// TekstilScada.Core/Services/LiveStepAnalyzer.cs
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
            Debug.WriteLine($"proses data içine girdi");
            bool hasStepChanged = false;

            // Duraklatma/Devam etme mantığı aynı kalacak
            if (status.IsPaused && !_currentPauseStartTime.HasValue)
            {
                _currentPauseStartTime = DateTime.Now;
            }
            else if (!status.IsPaused && _currentPauseStartTime.HasValue)
            {
                _currentStepPauseSeconds += (DateTime.Now - _currentPauseStartTime.Value).TotalSeconds;
                _currentPauseStartTime = null;
            }

            // Adım değişikliği tespit edildiğinde
            if (status.AktifAdimNo != _pendingStepNumber)
            {
                // Beklemedeki adımı yeni adıma taşı ve bekleme süresini başlat.
                _pendingStepNumber = status.AktifAdimNo;
                _pendingStepTimestamp = DateTime.Now;
                Debug.WriteLine($"[LiveStepAnalyzer] Yeni adım değişikliği tespit edildi: {status.AktifAdimNo}. Onay için bekleniyor...");
            }

            // Yeni adım, 3 saniye boyunca stabil bir şekilde devam ederse, yeni adımı başlat.
            if (status.AktifAdimTipiWordu !=0 && _pendingStepNumber != 0 && _pendingStepNumber == status.AktifAdimNo && (DateTime.Now - _pendingStepTimestamp.Value).TotalSeconds >= StepReadDelaySeconds)
            {
                Debug.WriteLine($"[LiveStepAnalyzer] Adım değişikliği onaylandı. Adım: {status.AktifAdimNo}");

                // Sadece yeni bir adım başladığında FinalizeStep ve StartNewStep'i çağır
                if (_currentStepNumber != status.AktifAdimNo)
                {
                    // Önceki adımı hemen sonlandır ve kaydet.
                    if (_currentStepNumber > 0)
                    {
                        Debug.WriteLine($"[LiveStepAnalyzer] Önceki adım ({_currentStepNumber}) sonlandırılıyor.");
                        FinalizeStep(_currentStepNumber, status.BatchNumarasi, status.MachineId);
                    }

                    // Atlanan adımları işle
                    HandleSkippedSteps(status, _currentStepNumber + 1, status.AktifAdimNo);

                    // Yeni adımı başlat
                    StartNewStep(status);

                    // _currentStepNumber'ı en son başlatılan adıma atayarak bir sonraki döngüde FinalizeStep'in doğru çalışmasını sağla.
                    _currentStepNumber = status.AktifAdimNo;

                    hasStepChanged = true;
                }

                // Bekleme durumunu sıfırla.
                _pendingStepNumber = 0;
                _pendingStepTimestamp = null;
            }
            else if (_pendingStepNumber != status.AktifAdimNo)
            {
                // Eğer PLC okuma döngüleri arasında adım değiştiyse, bekleme durumunu sıfırla.
                _pendingStepNumber = 0;
                _pendingStepTimestamp = null;
            }

            return hasStepChanged;
        }

        public void FinalizeStep(int stepNumber, string batchId, int machineId)
        {
            Debug.WriteLine($"[FinalizeStep] Metot çağrıldı. Adım: {stepNumber}, Batch: {batchId}");

            var stepToFinalize = AnalyzedSteps.LastOrDefault(s => s.StepNumber == stepNumber && s.WorkingTime == "İşleniyor...");
            if (stepToFinalize == null)
            {
                Debug.WriteLine($"[FinalizeStep] Hata: Sonlandırılacak adım bulunamadı. Adım: {stepNumber}");
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

            Debug.WriteLine($"[FinalizeStep] Kaydedilecek veri: Adım={stepToFinalize.StepNumber}, Çalışma={stepToFinalize.WorkingTime}, Sapma={stepToFinalize.DeflectionTime}");

            //_productionRepository.LogSingleStepDetail(stepToFinalize, machineId, batchId);
        }

        public void StartNewStep(FullMachineStatus status)
        {
            Debug.WriteLine($"[StartNewStep] Yeni adım başlatıldı: {status.AktifAdimNo}");
            CurrentStepStartTime = DateTime.Now;
            _currentPauseStartTime = null;
            _currentStepPauseSeconds = 0;

            var recipeStep = _recipe.Steps.FirstOrDefault(s => s.StepNumber == status.AktifAdimNo);

            AnalyzedSteps.Add(new ProductionStepDetail
            {
                StepNumber = status.AktifAdimNo,
                StepName = GetStepTypeName(status.AktifAdimTipiWordu),
                TheoreticalTime = CalculateTheoreticalTime(status.AktifAdimDataWords),
                WorkingTime = "İşleniyor...",
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
                        Debug.WriteLine($"[StartNewStep] Kimyasal tüketimi kaydedildi: {dozajParams.Kimyasal}, {dozajParams.DozajLitre} Litre");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[StartNewStep] Kimyasal tüketimi kaydedilirken hata oluştu: {ex.Message}");
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
                    string skippedStepName = GetStepTypeName(recipeStep.StepDataWords[24]) + " (Atlandı)";

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
            return AnalyzedSteps.LastOrDefault(s => s.WorkingTime != "İşleniyor...");
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
            if (controlWord == 0) return "Tanımsız Adım";

            var stepTypes = new List<string>();
            if ((controlWord & 1) != 0) stepTypes.Add("Su Alma");
            if ((controlWord & 2) != 0) stepTypes.Add("Isıtma");
            if ((controlWord & 4) != 0) stepTypes.Add("Çalışma");
            if ((controlWord & 8) != 0) stepTypes.Add("Dozaj");
            if ((controlWord & 16) != 0) stepTypes.Add("Boşaltma");
            if ((controlWord & 32) != 0) stepTypes.Add("Sıkma");
            return stepTypes.Any() ? string.Join(" + ", stepTypes) : "Bekliyor...";
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