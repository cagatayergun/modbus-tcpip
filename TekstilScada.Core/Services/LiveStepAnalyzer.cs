// File: TekstilScada.Core/Services/LiveStepAnalyzer.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.Core.Services
{
    public class LiveStepAnalyzer
    {
        private readonly ScadaRecipe _recipe;
        private readonly ProductionRepository _productionRepository;
        private int _currentStepNumber = 0;
        private DateTime _currentStepStartTime;
        private DateTime? _currentPauseStartTime;
        private double _currentStepPauseSeconds = 0;

        private int _pendingStepNumber = 0;
        private DateTime? _pendingStepTimestamp = null;
        private const int StepReadDelaySeconds = 2;

        public List<ProductionStepDetail> AnalyzedSteps { get; }
        public ScadaRecipe Recipe { get; private set; }
        public DateTime CurrentStepStartTime { get; private set; }

        public LiveStepAnalyzer(ScadaRecipe recipe, ProductionRepository productionRepository)
        {
            _recipe = recipe;
            this.Recipe = recipe;
            _productionRepository = productionRepository;
            AnalyzedSteps = new List<ProductionStepDetail>();
            CurrentStepStartTime = DateTime.Now;
        }

        public bool ProcessData(FullMachineStatus status)
        {

            bool hasStepChanged = false;

            // The pause/resume logic will remain the same
            if (status.IsPaused && !_currentPauseStartTime.HasValue)
            {
                _currentPauseStartTime = DateTime.Now;
            }
            else if (!status.IsPaused && _currentPauseStartTime.HasValue)
            {
                _currentStepPauseSeconds += (DateTime.Now - _currentPauseStartTime.Value).TotalSeconds;
                _currentPauseStartTime = null;
            }

            // When a step change is detected
            if (status.AktifAdimNo != _pendingStepNumber)
            {
                // Move the pending step to the new step and start the waiting period.
                _pendingStepNumber = status.AktifAdimNo;
                _pendingStepTimestamp = DateTime.Now;

            }

            // If the new step continues stably for 3 seconds, start the new step.
            if (status.AktifAdimTipiWordu != 0 && _pendingStepNumber != 0 && _pendingStepNumber == status.AktifAdimNo && (DateTime.Now - _pendingStepTimestamp.Value).TotalSeconds >= StepReadDelaySeconds)
            {


                // Only call FinalizeStep and StartNewStep when a new step begins
                if (_currentStepNumber != status.AktifAdimNo)
                {
                    // Immediately finalize and save the previous step.
                    if (_currentStepNumber > 0)
                    {

                        FinalizeStep(_currentStepNumber, status.BatchNumarasi, status.MachineId);
                    }

                    // Handle skipped steps
                    HandleSkippedSteps(status, _currentStepNumber + 1, status.AktifAdimNo);

                    // Start the new step
                    StartNewStep(status);

                    // Assign _currentStepNumber to the last started step to ensure FinalizeStep works correctly in the next loop.
                    _currentStepNumber = status.AktifAdimNo;

                    hasStepChanged = true;
                }

                // Reset the waiting state.
                _pendingStepNumber = 0;
                _pendingStepTimestamp = null;
            }
            else if (_pendingStepNumber != status.AktifAdimNo)
            {
                // If the step changed between PLC reading cycles, reset the waiting state.
                _pendingStepNumber = 0;
                _pendingStepTimestamp = null;
            }

            return hasStepChanged;
        }

        public void FinalizeStep(int stepNumber, string batchId, int machineId)
        {


            var stepToFinalize = AnalyzedSteps.LastOrDefault(s => s.StepNumber == stepNumber && s.WorkingTime == "Processing...");
            if (stepToFinalize == null)
            {

                return;
            }

            TimeSpan workingTime = DateTime.Now - CurrentStepStartTime;
            stepToFinalize.WorkingTime = workingTime.ToString(@"hh\:mm\:ss");

            if (_currentPauseStartTime.HasValue)
            {
                _currentStepPauseSeconds += (DateTime.Now - _currentPauseStartTime.Value).TotalSeconds;
                _currentPauseStartTime = null;
            }
            stepToFinalize.StopTime = TimeSpan.FromSeconds(_currentStepPauseSeconds).ToString(@"hh\:mm\:ss");

            TimeSpan theoreticalTime;
            TimeSpan.TryParse(stepToFinalize.TheoreticalTime, out theoreticalTime);

            TimeSpan actualWorkTime = workingTime - TimeSpan.FromSeconds(_currentStepPauseSeconds);
            TimeSpan deflection = actualWorkTime - theoreticalTime;

            string sign = deflection.TotalSeconds >= 0 ? "+" : "";
            stepToFinalize.DeflectionTime = $"{sign}{deflection:hh\\:mm\\:ss}";



            //_productionRepository.LogSingleStepDetail(stepToFinalize, machineId, batchId);
        }

        public void StartNewStep(FullMachineStatus status)
        {

            CurrentStepStartTime = DateTime.Now;
            _currentPauseStartTime = null;
            _currentStepPauseSeconds = 0;

            var recipeStep = _recipe.Steps.FirstOrDefault(s => s.StepNumber == status.AktifAdimNo);

            AnalyzedSteps.Add(new ProductionStepDetail
            {
                StepNumber = status.AktifAdimNo,
                StepName = GetStepTypeName(status.AktifAdimTipiWordu),
                TheoreticalTime = CalculateTheoreticalTime(status.AktifAdimDataWords),
                WorkingTime = "Processing...",
                StopTime = "00:00:00",
                DeflectionTime = ""
            });

            if ((status.AktifAdimTipiWordu & 8) != 0)
            {
                try
                {
                    var dozajParams = new DozajParams(status.AktifAdimDataWords);
                    if (!string.IsNullOrEmpty(dozajParams.Kimyasal) && dozajParams.DozajLitre > 0)
                    {
                        var consumptionData = new List<ChemicalConsumptionData>
                        {
                            new ChemicalConsumptionData
                            {
                                StepNumber = status.AktifAdimNo,
                                ChemicalName = dozajParams.Kimyasal,
                                AmountLiters = dozajParams.DozajLitre
                            }
                        };
                        _productionRepository.LogChemicalConsumption(status.MachineId, status.BatchNumarasi, consumptionData);

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void HandleSkippedSteps(FullMachineStatus status, int fromStep, int toStep)
        {
            for (int i = fromStep; i < toStep; i++)
            {
                var recipeStep = _recipe.Steps.FirstOrDefault(s => s.StepNumber == i);
                if (recipeStep != null && recipeStep.StepDataWords[24] != 0)
                {
                    string skippedStepName = GetStepTypeName(recipeStep.StepDataWords[24]) + " (Skipped)";

                    AnalyzedSteps.Add(new ProductionStepDetail
                    {
                        StepNumber = i,
                        StepName = skippedStepName,
                        TheoreticalTime = CalculateTheoreticalTime(recipeStep),
                        WorkingTime = "00:00:00",
                        StopTime = "00:00:00",
                        DeflectionTime = ""
                    });
                }
            }
        }

        public ProductionStepDetail GetLastCompletedStep()
        {
            return AnalyzedSteps.LastOrDefault(s => s.WorkingTime != "Processing...");
        }

        private const double SECONDS_PER_LITER = 0.5;
        private string CalculateTheoreticalTime(ScadaRecipeStep step)
        {
            var parallelDurations = new List<double>();
            short controlWord = step.StepDataWords[24];
            if ((controlWord & 1) != 0) parallelDurations.Add(new SuAlmaParams(step.StepDataWords).MiktarLitre * SECONDS_PER_LITER);
            if ((controlWord & 8) != 0)
            {
                var dozajParams = new DozajParams(step.StepDataWords);
                double dozajSuresi = 0;
                if (dozajParams.AnaTankMakSu || dozajParams.AnaTankTemizSu) { dozajSuresi += 60; }
                dozajSuresi += dozajParams.CozmeSure;
                if (dozajParams.Tank1Dozaj) { dozajSuresi += dozajParams.DozajSure; }
                parallelDurations.Add(dozajSuresi);
            }
            if ((controlWord & 2) != 0) parallelDurations.Add(new IsitmaParams(step.StepDataWords).Sure * 60);
            if ((controlWord & 4) != 0) parallelDurations.Add(new CalismaParams(step.StepDataWords).CalismaSuresi * 60);
            if ((controlWord & 16) != 0) parallelDurations.Add(120);
            if ((controlWord & 32) != 0) parallelDurations.Add(new SikmaParams(step.StepDataWords).SikmaSure * 60);
            double maxDurationSeconds = parallelDurations.Any() ? parallelDurations.Max() : 0;
            return TimeSpan.FromSeconds(maxDurationSeconds).ToString(@"hh\:mm\:ss");
        }

        private string GetStepTypeName(short controlWord)
        {
            if (controlWord == 0) return "Undefined Step";

            var stepTypes = new List<string>();
            if ((controlWord & 1) != 0) stepTypes.Add("Water Intake");
            if ((controlWord & 2) != 0) stepTypes.Add("Heating");
            if ((controlWord & 4) != 0) stepTypes.Add("Working");
            if ((controlWord & 8) != 0) stepTypes.Add("Dosing");
            if ((controlWord & 16) != 0) stepTypes.Add("Discharge");
            if ((controlWord & 32) != 0) stepTypes.Add("Squeezing");
            return stepTypes.Any() ? string.Join(" + ", stepTypes) : "Waiting...";
        }

        private string CalculateTheoreticalTime(short[] stepDataWords)
        {
            var parallelDurations = new List<double>();
            short controlWord = stepDataWords[24];

            if ((controlWord & 1) != 0) parallelDurations.Add(new SuAlmaParams(stepDataWords).MiktarLitre * SECONDS_PER_LITER);
            if ((controlWord & 8) != 0)
            {
                var dozajParams = new DozajParams(stepDataWords);
                double dozajSuresi = 0;
                if (dozajParams.AnaTankMakSu || dozajParams.AnaTankTemizSu) { dozajSuresi += 60; }
                dozajSuresi += dozajParams.CozmeSure;
                if (dozajParams.Tank1Dozaj) { dozajSuresi += dozajParams.DozajSure; }
                parallelDurations.Add(dozajSuresi);
            }
            if ((controlWord & 2) != 0) parallelDurations.Add(new IsitmaParams(stepDataWords).Sure * 60);
            if ((controlWord & 4) != 0) parallelDurations.Add(new CalismaParams(stepDataWords).CalismaSuresi * 60);
            if ((controlWord & 16) != 0) parallelDurations.Add(120);
            if ((controlWord & 32) != 0) parallelDurations.Add(new SikmaParams(stepDataWords).SikmaSure * 60);

            double maxDurationSeconds = parallelDurations.Any() ? parallelDurations.Max() : 0;
            return TimeSpan.FromSeconds(maxDurationSeconds).ToString(@"hh\:mm\:ss");
        }
    }
}