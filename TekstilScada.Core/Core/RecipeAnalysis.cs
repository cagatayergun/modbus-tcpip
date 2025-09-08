using System;
using System.Collections.Generic;
using System.Linq;
using TekstilScada.Models;

namespace TekstilScada.Core
{
    /// <summary>
    /// Reçetelerin teorik sürelerini ve diğer analizlerini yapmak için yardımcı metotlar içerir.
    /// </summary>
    public static class RecipeAnalysis
    {
        // Bu değerler LiveStepAnalyzer ile tutarlı olmalıdır.
        private const double SECONDS_PER_LITER = 0.5;
        private const double DRAIN_SECONDS = 120.0;
        /// <summary>
        /// Bir reçetenin tüm adımlarının teorik sürelerini toplayarak toplam teorik süreyi saniye cinsinden hesaplar.
        /// </summary>
        /// <param name="recipe">Hesaplanacak reçete.</param>
        /// <returns>Toplam teorik süre (saniye).</returns>
        public static double CalculateTheoreticalTimeForSteps(IEnumerable<ScadaRecipeStep> steps)
        {
            if (steps == null || !steps.Any()) return 0;

            double totalSeconds = 0;
            foreach (var step in steps)
            {
                var parallelDurations = new List<double>();
                short controlWord = step.StepDataWords[24];

                if ((controlWord & 1) != 0) // Su Alma
                {
                    parallelDurations.Add(new SuAlmaParams(step.StepDataWords).MiktarLitre * SECONDS_PER_LITER);
                }
                if ((controlWord & 8) != 0) // Dozaj
                {
                    var dozajParams = new DozajParams(step.StepDataWords);
                    double dozajSuresi = 0;
                    if (dozajParams.AnaTankMakSu || dozajParams.AnaTankTemizSu) { dozajSuresi += 60; }
                    dozajSuresi += dozajParams.CozmeSure;
                    if (dozajParams.Tank1Dozaj) { dozajSuresi += dozajParams.DozajSure; }
                    parallelDurations.Add(dozajSuresi);
                }
                if ((controlWord & 2) != 0) // Isıtma
                {
                    parallelDurations.Add(new IsitmaParams(step.StepDataWords).Sure * 60);
                }
                if ((controlWord & 4) != 0) // Çalışma
                {
                    parallelDurations.Add(new CalismaParams(step.StepDataWords).CalismaSuresi * 60);
                }
                if ((controlWord & 16) != 0) // Boşaltma
                {
                    // Boşaltma için sabit 120 saniye ekliyoruz.
                    parallelDurations.Add(DRAIN_SECONDS);
                }
                if ((controlWord & 32) != 0) // Sıkma
                {
                    parallelDurations.Add(new SikmaParams(step.StepDataWords).SikmaSure * 60);
                }

                totalSeconds += parallelDurations.Any() ? parallelDurations.Max() : 0;
            }
            return totalSeconds;
        }

        // YENİ: Eski metot artık yeni esnek metodu çağırıyor, kod tekrarını önlüyoruz.
        public static double CalculateTotalTheoreticalTimeSeconds(ScadaRecipe recipe)
        {
            if (recipe == null || recipe.Steps == null) return 0;
            return CalculateTheoreticalTimeForSteps(recipe.Steps);
        }
        public static double CalculateTotalTheoreticalTimeForDryingMachine(ScadaRecipe recipe)
        {
            if (recipe == null || !recipe.Steps.Any()) return 0;
            var firstStep = recipe.Steps.First();
            if (firstStep == null) return 0;

            var paramsDrying = new KurutmaParams(firstStep.StepDataWords);

            // Zaman bazlı çalışma (sadece süre kontrolü aktifse)
            if ((paramsDrying.ControlWord & 2) != 0)
            {
                return (paramsDrying.DurationMinutes + paramsDrying.CoolingTimeMinutes) * 60;
            }
            // Nem bazlı çalışma (sadece nem kontrolü aktifse)
            else if ((paramsDrying.ControlWord & 1) != 0)
            {
                // Nem bazlı süre tahmini burada yapılır. Bu bilgi olmadan basit bir ortalama süre kullanılabilir.
                // Şimdilik sadece soğutma süresini ekleyelim, çünkü nem süresi dinamik olarak hesaplanamaz.
                return paramsDrying.CoolingTimeMinutes * 60;
            }
            // Hem nem hem de süre kontrolü aynı anda aktifse (hangisi önce biterse)
            else if ((paramsDrying.ControlWord & 3) == 3)
            {
                // Burada teorik süre, daha kısa olanın süresi olur. Ancak elimizde nem için teorik süre yok.
                // Bu durumda sadece süre bilgisini alıp soğutma süresini ekleyebiliriz.
                return (paramsDrying.DurationMinutes + paramsDrying.CoolingTimeMinutes) * 60;
            }

            return 0;
        }
    }
}