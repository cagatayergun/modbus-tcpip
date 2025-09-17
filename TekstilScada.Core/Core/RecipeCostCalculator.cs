using System.Collections.Generic;
using System.Linq;
using TekstilScada.Models;

namespace TekstilScada.Core
{
    public static class RecipeCostCalculator
    {
        private const decimal MOTOR_POWER_KW = 15.0m;
        private const decimal STEAM_KG_PER_MINUTE_AT_HIGH_TEMP = 5.0m;

        // UPDATED: The method now also returns the currency symbol
        public static (decimal TotalCost, string CurrencySymbol, string Breakdown) Calculate(ScadaRecipe recipe, List<CostParameter> costParams)
        {
            if (recipe == null || !recipe.Steps.Any() || !costParams.Any())
            {
                return (0, "TRY", "No data.");
            }

            var waterParam = costParams.FirstOrDefault(p => p.ParameterName == "Water");
            var electricityParam = costParams.FirstOrDefault(p => p.ParameterName == "Electricity");
            var steamParam = costParams.FirstOrDefault(p => p.ParameterName == "Steam");

            if (waterParam == null || electricityParam == null || steamParam == null)
            {
                return (0, "TRY", "Missing cost parameters.");
            }

            decimal totalWaterLiters = 0;
            decimal totalOperatingMinutes = 0;
            decimal totalHeatingMinutes = 0;

            foreach (var step in recipe.Steps)
            {
                short controlWord = step.StepDataWords[24];
                if ((controlWord & 1) != 0) totalWaterLiters += step.StepDataWords[1];
                if ((controlWord & 4) != 0) totalOperatingMinutes += step.StepDataWords[18];
                if ((controlWord & 32) != 0) totalOperatingMinutes += step.StepDataWords[9];
                if ((controlWord & 2) != 0) totalHeatingMinutes += step.StepDataWords[4];
            }

            // UPDATED: Costs are now calculated using a MULTIPLIER
            decimal waterCost = totalWaterLiters * waterParam.CostValue * waterParam.Multiplier;
            decimal electricityCost = (totalOperatingMinutes / 60.0m) * MOTOR_POWER_KW * electricityParam.CostValue * electricityParam.Multiplier;
            decimal steamCost = totalHeatingMinutes * STEAM_KG_PER_MINUTE_AT_HIGH_TEMP * steamParam.CostValue * steamParam.Multiplier;

            decimal totalCost = waterCost + electricityCost + steamCost;
            string currencySymbol = waterParam.CurrencySymbol ?? "TRY";

            // Create a detailed breakdown text
            string breakdown = $"Water: {waterCost:F2} {currencySymbol}\nElectricity: {electricityCost:F2} {currencySymbol}\nSteam: {steamCost:F2} {currencySymbol}";

            return (totalCost, currencySymbol, breakdown);
        }
    }
}