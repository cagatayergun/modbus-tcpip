// Services/IPlcManager.cs
using HslCommunication;
using System.Collections.Generic;
using System.Threading.Tasks;
using TekstilScada.Models;
using TekstilScada.Models; // BU SATIR EKLENDİ
namespace TekstilScada.Services
{
    /// <summary>
    /// Farklı PLC tipleriyle iletişimi soyutlayan arayüz.
    /// Tüm PLC yönetici sınıfları bu arayüzü uygulamalıdır.
    /// </summary>
    public interface IPlcManager
    {
        string IpAddress { get; }
        Task<OperateResult<ScadaRecipe>> ReadFullRecipeDataAsync(); // Bu satırı ekleyin
        Task<OperateResult<Dictionary<int, string>>> ReadRecipeNamesFromPlcAsync(); // YENİ EKLENEN SATIR
        OperateResult Connect();
        OperateResult Disconnect();
        Task<OperateResult> WriteRecipeNameAsync(int recipeNumber, string recipeName);
        // Canlı veri okuma
        OperateResult<FullMachineStatus> ReadLiveStatusData();

        // Reçete işlemleri
        
        Task<OperateResult<short[]>> ReadRecipeFromPlcAsync();
        Task<OperateResult> WriteRecipeToPlcAsync(ScadaRecipe recipe, int? recipeSlot = null);
        // Operatör işlemleri
        Task<OperateResult<List<PlcOperator>>> ReadPlcOperatorsAsync();
        Task<OperateResult> WritePlcOperatorAsync(PlcOperator plcOperator);
        Task<OperateResult<PlcOperator>> ReadSinglePlcOperatorAsync(int slotIndex);

        // Üretim sonu rapor verilerini okuma
        Task<OperateResult<BatchSummaryData>> ReadBatchSummaryDataAsync();
       // Task<OperateResult<List<ChemicalConsumptionData>>> ReadChemicalConsumptionDataAsync();
      //  Task<OperateResult<List<ProductionStepDetail>>> ReadStepAnalysisDataAsync();
        Task<OperateResult> AcknowledgeAlarm();
        Task<OperateResult> ResetOeeCountersAsync();
        Task<OperateResult> IncrementProductionCounterAsync();
    }
}
