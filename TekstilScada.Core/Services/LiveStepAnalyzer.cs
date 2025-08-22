// TekstilScada.Core/Services/LiveStepAnalyzer.cs
using System;
using System.Collections.Generic;
using System.Linq;
using TekstilScada.Models;
using TekstilScada.Repositories; // ProductionRepository için eklendi

namespace TekstilScada.Core.Services
{
    public class LiveStepAnalyzer
    {
        private readonly ScadaRecipe _recipe;
        private readonly ProductionRepository _productionRepository; // YENİ: Bağımlılık eklendi
        private int _currentStepNumber = 0;
        private DateTime _currentStepStartTime;
        private DateTime? _currentPauseStartTime;
        private double _currentStepPauseSeconds = 0;

        // --- Durum Yönetimi İçin Değişkenler ---
        private int _pendingStepNumber = 0; // Onaylanmayı bekleyen yeni adım numarası
        private DateTime? _pendingStepTimestamp = null; // Adım değişikliğinin tespit edildiği zaman
        private const int StepReadDelaySeconds = 3; // Adım ismini okumadan önce beklenecek süre (saniye)

        public List<ProductionStepDetail> AnalyzedSteps { get; }
        public ScadaRecipe Recipe { get; private set; }
        public DateTime CurrentStepStartTime { get; private set; }


        // GÜNCELLENDİ: Yeni constructor, ProductionRepository alacak şekilde.
        public LiveStepAnalyzer(ScadaRecipe recipe, ProductionRepository productionRepository)
        {
            _recipe = recipe;
            this.Recipe = recipe;
            _productionRepository = productionRepository;
            AnalyzedSteps = new List<ProductionStepDetail>();
            CurrentStepStartTime = DateTime.Now;
        }

        // ✅ TAMAMEN GÜNCELLENMİŞ METOT
        public bool ProcessData(FullMachineStatus status)
        {
            bool hasStepChanged = false;

            // Duraklatma (Pause) mantığı
            if (status.IsPaused && !_currentPauseStartTime.HasValue)
            {
                _currentPauseStartTime = DateTime.Now;
            }
            else if (!status.IsPaused && _currentPauseStartTime.HasValue)
            {
                _currentStepPauseSeconds += (DateTime.Now - _currentPauseStartTime.Value).TotalSeconds;
                _currentPauseStartTime = null;
            }

            // --- 1. AŞAMA: ONAY BEKLEYEN ADIMI İŞLEME ---
            // Eğer onay bekleyen bir adım varsa ve 3 saniyelik gecikme süresi dolmuşsa...
            if (_pendingStepNumber != 0 && _pendingStepTimestamp.HasValue && (DateTime.Now - _pendingStepTimestamp.Value).TotalSeconds >= StepReadDelaySeconds)
            {
                // Gecikme süresi doldu. Adım tipini şimdi okumak güvenli.
                // PLC'den gelen adım tipini kontrol et. Eğer 0 ise (boş/tanımsız adım), bu adımı kaydetme.
                if (status.AktifAdimTipiWordu != 0 && status.AktifAdimNo == _pendingStepNumber)
                {
                    // Adım tipi geçerli. Yeni adımı başlat.
                    StartNewStep(status);
                    hasStepChanged = true; // Arayüzün güncellenmesi için true döndür
                }

                // Bekleme durumunu sıfırla. Adım ister kaydedilsin ister edilmesin, bu durum bitti.
                _pendingStepNumber = 0;
                _pendingStepTimestamp = null;
            }

            // --- 2. AŞAMA: YENİ ADIM DEĞİŞİKLİĞİNİ TESPİT ETME ---
            // PLC'den gelen adım numarası bizim takip ettiğimizden farklıysa ve beklemede olan bir adım yoksa...
            if (status.AktifAdimNo != _currentStepNumber && _pendingStepNumber == 0)
            {
                // Mevcut adımı bitir (sürelerini hesapla ve kaydet).
                if (_currentStepNumber > 0)
                {
                    FinalizeStep(_currentStepNumber, status.BatchNumarasi, status.MachineId);
                }

                // Atlanan adımları işle
                HandleSkippedSteps(status, _currentStepNumber + 1, status.AktifAdimNo);

                // Yeni adımı hemen başlatma, "onay bekliyor" olarak işaretle.
                _pendingStepNumber = status.AktifAdimNo;
                _pendingStepTimestamp = DateTime.Now;

                // Takip ettiğimiz adım numarasını şimdi güncelle ki bu blok tekrar tekrar tetiklenmesin.
                _currentStepNumber = status.AktifAdimNo;
            }

            return hasStepChanged;
        }

        // ✅ BASİTLEŞTİRİLMİŞ METOT
        public void StartNewStep(FullMachineStatus status)
        {
            CurrentStepStartTime = DateTime.Now;
            _currentPauseStartTime = null;
            _currentStepPauseSeconds = 0;

            // Teorik zamanı hesaplamak için reçetedeki orijinal adımı bulalım.
            var recipeStep = _recipe.Steps.FirstOrDefault(s => s.StepNumber == status.AktifAdimNo);

            AnalyzedSteps.Add(new ProductionStepDetail
            {
                StepNumber = status.AktifAdimNo,
                // Adım adı, 3 saniyelik gecikme sonrası okunan güncel PLC verisinden alınıyor.
                StepName = GetStepTypeName(status.AktifAdimTipiWordu),
                TheoreticalTime = CalculateTheoreticalTime(status.AktifAdimDataWords),

                WorkingTime = "İşleniyor...",
                StopTime = "00:00:00",
                DeflectionTime = ""
            });

            // ✅ YENİ MANTIK: Kimyasal tüketimini kaydet
            if ((status.AktifAdimTipiWordu & 8) != 0) // Eğer adım tipi "Dozaj" ise
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
                        Console.WriteLine($"Makine {status.MachineId} için Dozaj adımı ({status.AktifAdimNo}) kaydedildi: {dozajParams.Kimyasal}, {dozajParams.DozajLitre} Litre");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kimyasal tüketimi kaydedilirken hata oluştu: {ex.Message}");
                }
            }
        }

        private void FinalizeStep(int stepNumber, string batchId, int machineId)
        {
            var stepToFinalize = AnalyzedSteps.LastOrDefault(s => s.StepNumber == stepNumber && s.WorkingTime == "İşleniyor...");
            if (stepToFinalize == null) return;

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

            // Adım tamamlandığında veritabanına logla
            _productionRepository.LogSingleStepDetail(stepToFinalize, machineId, batchId);
        }

        // Bu metotları olduğu gibi bırakabilirsiniz
        private void HandleSkippedSteps(FullMachineStatus status, int fromStep, int toStep)
        {
            for (int i = fromStep; i < toStep; i++)
            {
                var recipeStep = _recipe.Steps.FirstOrDefault(s => s.StepNumber == i);
                if (recipeStep != null && recipeStep.StepDataWords[24] != 0) // Atlanan adımın boş olmadığını kontrol et
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
            if ((controlWord & 16) != 0) parallelDurations.Add(120); // Boşaltma sabit 120 sn
            if ((controlWord & 32) != 0) parallelDurations.Add(new SikmaParams(step.StepDataWords).SikmaSure * 60);
            double maxDurationSeconds = parallelDurations.Any() ? parallelDurations.Max() : 0;
            return TimeSpan.FromSeconds(maxDurationSeconds).ToString(@"hh\:mm\:ss");
        }

        private string GetStepTypeName(short controlWord)
        {
            if (controlWord == 0) return "Tanımsız Adım"; // Artık bu durum oluşmamalı ama önlem olarak kalabilir

            var stepTypes = new List<string>();
            if ((controlWord & 1) != 0) stepTypes.Add("Su Alma");
            if ((controlWord & 2) != 0) stepTypes.Add("Isıtma");
            if ((controlWord & 4) != 0) stepTypes.Add("Çalışma");
            if ((controlWord & 8) != 0) stepTypes.Add("Dozaj");
            if ((controlWord & 16) != 0) stepTypes.Add("Boşaltma");
            if ((controlWord & 32) != 0) stepTypes.Add("Sıkma");
            return stepTypes.Any() ? string.Join(" + ", stepTypes) : "Bekliyor...";
        }
        // LiveStepAnalyzer.cs dosyasının içine bu YENİ metodu ekleyin.

        private string CalculateTheoreticalTime(short[] stepDataWords)
        {
            // Bu metot, ScadaRecipeStep nesnesi yerine doğrudan PLC'den gelen
            // ham word dizisini kullanarak hesaplama yapar.
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
            if ((controlWord & 16) != 0) parallelDurations.Add(120); // Boşaltma sabit 120 sn
            if ((controlWord & 32) != 0) parallelDurations.Add(new SikmaParams(stepDataWords).SikmaSure * 60);

            double maxDurationSeconds = parallelDurations.Any() ? parallelDurations.Max() : 0;
            return TimeSpan.FromSeconds(maxDurationSeconds).ToString(@"hh\:mm\:ss");
        }
    }
}