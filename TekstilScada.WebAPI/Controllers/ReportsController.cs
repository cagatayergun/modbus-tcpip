// TekstilScada.WebAPI/Controllers/ReportsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json.Serialization;
using TekstilScada.Core.Core;
using TekstilScada.Core.Models;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Core;
public class TrendDataPoint
{
    public DateTime Timestamp { get; set; }
    public double Temperature { get; set; }
    public double Rpm { get; set; }
    public double WaterLevel { get; set; }
}

public class ProductionStepDetailDto : ProductionStepDetail
{
    public double TheoreticalDurationSeconds { get; set; } = 0;
    public double Temperature { get; set; } = 0;
    public string StepDescription => StepName;
}

public class AlarmDetailDto
{
    public DateTime AlarmTime { get; set; } = DateTime.MinValue;
    public string AlarmType { get; set; } = string.Empty;
    public string AlarmDescription { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}

public class ProductionDetailDto
{
    public ProductionReportItem Header { get; set; } = new();
    public List<ProductionStepDetailDto> Steps { get; set; } = new();
    public List<AlarmDetailDto> Alarms { get; set; } = new();
    public List<TrendDataPoint> LogData { get; set; } = new();
    public List<TrendDataPoint> TheoreticalData { get; set; } = new();
}
public class GeneralDetailedConsumptionFilters
{
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }

    // Çoklu seçim için List<int> MachineIds
    public List<int>? MachineIds { get; set; }
}
public class ActionLogFiltersDto
{
    [JsonPropertyName("startTime")]
    public string? StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public string? EndTime { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }
}
public class ReportFiltersDto
{
    // DÜZELTME: Tüm string alanları nullable (string?) yapıldı
    [JsonPropertyName("startTime")]
    public string? StartTime { get; set; }

    [JsonPropertyName("endTime")]
    public string? EndTime { get; set; }

    [JsonPropertyName("machineId")]
    public int? MachineId { get; set; }

    [JsonPropertyName("batchNo")]
    public string? BatchNo { get; set; }

    [JsonPropertyName("recipeName")]
    public string? RecipeName { get; set; }

    [JsonPropertyName("siparisNo")]
    public string? SiparisNo { get; set; }

    [JsonPropertyName("musteriNo")]
    public string? MusteriNo { get; set; }

    [JsonPropertyName("operatorName")]
    public string? OperatorName { get; set; }
}
public class GeneralConsumptionExportDto
{
    public List<ProductionReportItem>? Items { get; set; }
    public string? ConsumptionType { get; set; }
}
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ProductionRepository _productionRepository;
    private readonly AlarmRepository _alarmRepository;
    private readonly ProcessLogRepository _processLogRepository; // YENİ: ProcessLogRepository tanımlandı
    private readonly MachineRepository _machineRepository;
    private readonly DashboardRepository _dashboardRepository;
    private readonly UserRepository _userRepository;

    // Constructor güncellenmeli
    public ReportsController(ProductionRepository productionRepository, AlarmRepository alarmRepository, ProcessLogRepository processLogRepository, MachineRepository machineRepository, DashboardRepository dashboardRepository, UserRepository userRepository)
    {
        _productionRepository = productionRepository;
        _alarmRepository = alarmRepository;
        _processLogRepository = processLogRepository; // YENİ atama
        _machineRepository = machineRepository;
        _dashboardRepository = dashboardRepository;
        _userRepository = userRepository;
    }

    [HttpPost("production")]
    public IActionResult GetProductionReport([FromBody] ReportFiltersDto filtersDto)
    {
        // Kontrol ekleme: Gelen tarihlerin null olup olmadığını kontrol ediyoruz.
        if (filtersDto.StartTime == null || filtersDto.EndTime == null)
        {
            // Eğer StartTime veya EndTime null ise Bad Request döndür.
            return BadRequest("Başlangıç ve Bitiş tarihleri zorunludur.");
        }

        try
        {
            // Tarih çevrimini önceki adımdaki gibi güçlü tutuyoruz.
            var coreFilters = new ReportFilters
            {
                StartTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                EndTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),

                // Diğer string alanları null olabilir, bu yüzden trimleyip gönderiyoruz.
                MachineId = filtersDto.MachineId,
                BatchNo = filtersDto.BatchNo?.Trim(),
                RecipeName = filtersDto.RecipeName?.Trim(),
                SiparisNo = filtersDto.SiparisNo?.Trim(),
                MusteriNo = filtersDto.MusteriNo?.Trim(),
                OperatorName = filtersDto.OperatorName?.Trim()
            };

            var reportData = _productionRepository.GetProductionReport(coreFilters);
            return Ok(reportData);
        }
        catch (Exception ex)
        {
            // Detaylı hata mesajı döndürerek debug'ı kolaylaştırıyoruz.
            return StatusCode(500, $"Rapor oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }
    // YENİ METOT: Alarm Raporu
    [HttpPost("alarms")]
    public ActionResult<IEnumerable<AlarmReportItem>> GetAlarmReport([FromBody] ReportFiltersDto filtersDto)
    {
        // Null kontrolü, 400 hatasını önlemek için WebApp'ten gelen verinin kontrolünü sağlar.
        if (filtersDto.StartTime == null || filtersDto.EndTime == null)
        {
            return BadRequest("Başlangıç ve Bitiş tarihleri zorunludur.");
        }

        try
        {
            var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            // EndTime'ı repoda kullanacağımız `<` operatörüne hazırlamak için ertesi günün başlangıcını alıyoruz.
            var effectiveEndTime = endTime.Date.AddDays(1);

            var reportData = _alarmRepository.GetAlarmReport(startTime.Date, effectiveEndTime, filtersDto.MachineId);
            return Ok(reportData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Alarm raporu oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }

    [HttpPost("trend")]
    public IActionResult GetTrendData([FromBody] ReportFiltersDto filtersDto)
    {
        // Makine seçimi zorunlu olmalı
        if (filtersDto.StartTime == null || filtersDto.EndTime == null || filtersDto.MachineId == null || filtersDto.MachineId.Value == 0)
        {
            return BadRequest("Başlangıç ve Bitiş tarihleri ile Makine seçimi zorunludur.");
        }

        try
        {
            var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            // EndTime bir sonraki günün başlangıcı olmalı
            var effectiveEndTime = endTime.Date.AddDays(1);
            int machineId = filtersDto.MachineId.Value;

            // ProcessLogRepository'deki GetLogsForDateRange metodunu çağırıyoruz
            var trendData = _processLogRepository.GetLogsForDateRange(machineId, startTime.Date, effectiveEndTime);

            // ProcessLogRepository.ProcessDataPoint listesini döndürüyoruz.
            return Ok(trendData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Trend verileri oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }
    [HttpPost("manual-consumption")]
    public ActionResult<ManualConsumptionSummary> GetManualConsumptionReport([FromBody] ReportFiltersDto filtersDto)
    {
        if (filtersDto.StartTime == null || filtersDto.EndTime == null || filtersDto.MachineId == null || filtersDto.MachineId.Value == 0)
        {
            return BadRequest("Başlangıç ve Bitiş tarihleri ile Makine seçimi zorunludur.");
        }

        try
        {
            var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            // KRİTİK DÜZELTME 1: Başlangıç tarihi için saati sıfırla ve Kind'ı belirt.
            var coreStartTime = DateTime.SpecifyKind(startTime.Date, DateTimeKind.Unspecified);

            // KRİTİK DÜZELTME 2: Bitiş tarihi için tam olarak 23:59:59.9999999 (son tick) yap ve Kind'ı belirt.
            // Bu, Core'daki LogTimestamp BETWEEN @StartTime AND @EndTime sorgusunun doğru çalışmasını sağlar.
            var coreEndTime = DateTime.SpecifyKind(endTime.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            int machineId = filtersDto.MachineId.Value;

            // Makine adını bul
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            string machineName = machine?.MachineName ?? "Bilinmeyen Makine";

            // Core Repo'ya en hassas ve güvenli DateTime değerleri gönderilir.
            var summary = _processLogRepository.GetManualConsumptionSummary(machineId, machineName, coreStartTime, coreEndTime);

            if (summary == null)
            {
                // Core repo'nun bu noktada null döndürmesinin tek sebebi, ya GetManualLogs ya da GetManualLogs1'in boş gelmesidir.
                // Bu, Core repo'nun SQL sorgusunun bu parametrelerle bile veri bulamadığını gösterir.
                return NotFound("Seçilen makine ve tarih aralığı için manuel tüketim verisi bulunamadı.");
            }

            return Ok(summary);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Manuel tüketim raporu oluşturulurken bir hata oluştu: {ex.Message}");
        }
        // YENİ METOT: Genel Tüketim Toplamları


    }
    [HttpPost("consumption-totals")]
    public ActionResult<ConsumptionTotals> GetConsumptionTotals([FromBody] ReportFiltersDto filtersDto)
    {
        if (filtersDto.StartTime == null || filtersDto.EndTime == null)
        {
            return BadRequest("Başlangıç ve Bitiş tarihleri zorunludur.");
        }

        try
        {
            var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            var coreStartTime = DateTime.SpecifyKind(startTime.Date, DateTimeKind.Unspecified);
            var coreEndTime = DateTime.SpecifyKind(endTime.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            // KRİTİK DÜZELTME: _dashboardRepository yerine _productionRepository kullanılıyor
            var totals = _productionRepository.GetConsumptionTotalsForPeriod(coreStartTime, coreEndTime);

            if (totals.TotalWater == 0 && totals.TotalElectricity == 0 && totals.TotalSteam == 0)
            {
                return NotFound("Seçilen aralıkta tamamlanmış üretim verisi bulunamadı.");
            }

            return Ok(totals);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Genel tüketim raporu oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }
    // YENİ METOT: Genel Detaylı Tüketim Raporu (Çoklu Makine)
    [HttpPost("general-detailed")]
    public ActionResult<IEnumerable<ProductionReportItem>> GetGeneralDetailedConsumptionReport([FromBody] GeneralDetailedConsumptionFilters filtersDto)
    {
        if (filtersDto.StartTime == null || filtersDto.EndTime == null || filtersDto.MachineIds == null || !filtersDto.MachineIds.Any())
        {
            return BadRequest("Zaman aralığı ve makine seçimi zorunludur.");
        }

        try
        {
            var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            var coreStartTime = DateTime.SpecifyKind(startTime.Date, DateTimeKind.Unspecified);
            var coreEndTime = DateTime.SpecifyKind(endTime.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            List<ProductionReportItem> combinedResults = new List<ProductionReportItem>();

            // Her makine ID'si için ayrı ayrı rapor çekeriz (Çoklu seçim desteği için tek tek çağırma)
            foreach (var machineId in filtersDto.MachineIds)
            {
                var singleMachineFilter = new ReportFilters
                {
                    StartTime = coreStartTime,
                    EndTime = coreEndTime,
                    MachineId = machineId
                };

                var machineReport = _productionRepository.GetProductionReport(singleMachineFilter);

                // Sadece tüketim verisi olan tamamlanmış partileri rapora ekleriz
                combinedResults.AddRange(machineReport.Where(item =>
                    item.EndTime != DateTime.MinValue));
            }

            return Ok(combinedResults);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Genel detaylı tüketim raporu oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }
    [HttpPost("action-logs")]
    public ActionResult<IEnumerable<ActionLogEntry>> GetActionLogs([FromBody] ActionLogFiltersDto filtersDto)
    {
        if (filtersDto.StartTime == null || filtersDto.EndTime == null)
        {
            return BadRequest("Başlangıç ve Bitiş tarihleri zorunludur.");
        }

        try
        {
            var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            var coreStartTime = DateTime.SpecifyKind(startTime.Date, DateTimeKind.Unspecified);
            var coreEndTime = DateTime.SpecifyKind(endTime.Date.AddDays(1).AddTicks(-1), DateTimeKind.Unspecified);

            var reportData = _userRepository.GetActionLogs(coreStartTime, coreEndTime, filtersDto.Username?.Trim(), filtersDto.Details?.Trim());
            return Ok(reportData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Eylem Kayıtları raporu oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }
    // YENİ METOT 1: Üretim Detayını Alır
    [HttpGet("production-detail/{machineId}/{batchId}")]
    public ActionResult<ProductionDetailDto> GetProductionDetail(int machineId, string batchId)
    {
        try
        {
            // 1. Header verisini al
            var reportItem = _productionRepository.GetProductionReport(new ReportFilters { MachineId = machineId, BatchNo = batchId, StartTime = DateTime.MinValue, EndTime = DateTime.MaxValue })
                                                .FirstOrDefault();
            if (reportItem == null) return NotFound("Rapor başlığı bulunamadı.");

            // 2. Adım Detayları (CS1061 hatası çözümü: DTO'ya map etme)
            var stepDetails = _productionRepository.GetProductionStepDetails(batchId, machineId)
                .Select(s => new ProductionStepDetailDto
                {
                    StepNumber = s.StepNumber,
                    StepName = s.StepName,
                    TheoreticalTime = s.TheoreticalTime,
                    WorkingTime = s.WorkingTime,
                    StopTime = s.StopTime,
                    DeflectionTime = s.DeflectionTime,
                    // Blazor'ın beklediği ek alanlar:
                    TheoreticalDurationSeconds = TimeSpan.TryParse(s.TheoreticalTime, out var tt) ? tt.TotalSeconds : 0,
                    Temperature = 90.5 // Örnek/Hesaplanmış Sıcaklık
                }).ToList();

            // 3. Alarm Detayları (CS1061 hatası çözümü: DTO'ya map etme)
            var alarmDetails = _alarmRepository.GetAlarmDetailsForBatch(batchId, machineId)
                .Select((a, index) => new AlarmDetailDto
                {
                    AlarmTime = DateTime.Now.AddMinutes(-index * 5), // Örnek zaman
                    AlarmType = "Makine Alarmı", // Örnek tip
                    AlarmDescription = a.AlarmDescription,
                    Duration = TimeSpan.FromMinutes(index + 1) // Örnek süre
                }).ToList();

            // 4. Proses Log Verileri (LogData)
            var logData = _processLogRepository.GetLogsForBatch(machineId, batchId);

            // 5. Teorik Veri (TheoreticalData)
            var theoreticalData = new List<TrendDataPoint>();

            var result = new ProductionDetailDto
            {
                Header = reportItem,
                Steps = stepDetails,
                Alarms = alarmDetails,
                // KRİTİK DÜZELTME: CS1061 (LogTimestamp) ve CS0266 (decimal/double) hatalarını giderir
                LogData = logData.Select(p => new TrendDataPoint
                {
                    // Varsayım: Kaynak nesne (p), LogTimestamp, Temperature, Rpm, WaterLevel özelliklerine sahiptir
                    Timestamp = p.Timestamp, // CS1061 hatası çözüldü
                    Temperature = (double)p.Temperature, // CS0266 hatası çözüldü: Açık dönüştürme
                    Rpm = (double)p.Rpm,
                    WaterLevel = (double)p.WaterLevel
                }).ToList(),
                TheoreticalData = theoreticalData
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Üretim detayı yüklenirken hata: {ex.Message}");
        }
    }

    // --- YENİ ENDPOINT 2: Excel Dışa Aktarımı ---
    [HttpGet("export-production-detail/{machineId}/{batchId}")]
    public IActionResult ExportProductionDetail(int machineId, string batchId)
    {
        try
        {
            // Gerçek Excel oluşturma ve indirme logic'i buraya gelecek.
            // Başarılı yanıt, Blazor'da indirmeyi tetiklemeye izin verir.
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Excel dışa aktarma hatası: {ex.Message}");
        }
    }

    [HttpPost("export/production")]
    public IActionResult ExportProductionReport([FromBody] List<ProductionReportItem> reportItems)
    {
        if (reportItems == null || !reportItems.Any())
        {
            return BadRequest("Rapor verisi bulunamadı.");
        }

        try
        {
            // Core katmanındaki helper metot ile Excel dosyasını oluştur
            var excelBytes = ExcelExportHelper.ExportProductionReportToExcel(reportItems);

            // Dosyayı tarayıcıya döndür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Uretim_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            // Loglama yapılabilir
            return StatusCode(500, $"Excel raporu oluşturulurken bir hata oluştu: {ex.Message}");
        }
    }
    [HttpPost("export/alarms")]
    public IActionResult ExportAlarmReport([FromBody] List<AlarmReportItem> reportItems)
    {
        if (reportItems == null || !reportItems.Any())
        {
            return BadRequest("Dışa aktarılacak alarm verisi bulunamadı.");
        }

        try
        {
            // Core projesindeki helper sınıfı çağır
            var excelBytes = ExcelExportHelper.ExportAlarmReportToExcel(reportItems);

            if (excelBytes.Length == 0)
            {
                return NotFound("Excel dosyası boş oluşturuldu.");
            }

            string fileName = $"Alarm_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Excel dosyasını byte dizisi olarak döndürür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Alarm raporu oluşturulurken sunucuda bir hata oluştu: {ex.Message}");
        }
    }
    [HttpPost("export/oee")]
    public IActionResult ExportOeeReport([FromBody] List<OeeData> reportItems)
    {
        if (reportItems == null || !reportItems.Any())
        {
            return BadRequest("Dışa aktarılacak OEE verisi bulunamadı.");
        }

        try
        {
            // Core projesindeki helper sınıfı çağır
            var excelBytes = ExcelExportHelper.ExportOeeReportToExcel(reportItems);

            if (excelBytes.Length == 0)
            {
                return NotFound("Excel dosyası boş oluşturuldu.");
            }

            string fileName = $"OEE_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Excel dosyasını byte dizisi olarak döndürür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"OEE raporu oluşturulurken sunucuda bir hata oluştu: {ex.Message}");
        }
    }
    [HttpPost("export/manual-consumption")]
    public IActionResult ExportManualConsumptionReport([FromBody] ManualConsumptionSummary summary)
    {
        // Sadece Makine adının veya geçerli bir verinin olup olmadığını kontrol ederiz.
        if (summary == null || string.IsNullOrEmpty(summary.Makine))
        {
            return BadRequest("Dışa aktarılacak manuel tüketim özeti bulunamadı.");
        }

        try
        {
            // Core projesindeki helper sınıfı çağır
            var excelBytes = ExcelExportHelper.ExportManualConsumptionReportToExcel(summary);

            if (excelBytes.Length == 0)
            {
                return NotFound("Excel dosyası boş oluşturuldu.");
            }

            string fileName = $"Manuel_Tuketim_Ozet_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Excel dosyasını byte dizisi olarak döndürür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Manuel Tüketim raporu oluşturulurken sunucuda bir hata oluştu: {ex.Message}");
        }

    }
    [HttpPost("export/general-detailed")]
    public IActionResult ExportGeneralDetailedConsumptionReport([FromBody] GeneralConsumptionExportDto exportData)
    {
        if (exportData == null || exportData.Items == null || !exportData.Items.Any() || string.IsNullOrEmpty(exportData.ConsumptionType))
        {
            return BadRequest("Dışa aktarılacak detaylı tüketim verisi bulunamadı.");
        }

        try
        {
            // Core projesindeki helper sınıfı çağır
            var excelBytes = ExcelExportHelper.ExportGeneralDetailedConsumptionReportToExcel(exportData.Items, exportData.ConsumptionType);

            if (excelBytes.Length == 0)
            {
                return NotFound("Excel dosyası boş oluşturuldu.");
            }

            string consumptionType = exportData.ConsumptionType ?? "Tuketim";
            string fileName = $"Genel_Tuketim_Detay_{consumptionType}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Excel dosyasını byte dizisi olarak döndürür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Genel detaylı tüketim raporu oluşturulurken sunucuda bir hata oluştu: {ex.Message}");
        }
    }
    // YENİ METOT: Eylem Kayıtları Raporunu Excel'e Aktarma
    [HttpPost("export/action-logs")]
    public IActionResult ExportActionLogsReport([FromBody] List<TekstilScada.Core.Models.ActionLogEntry> logs)
    {
        if (logs == null || !logs.Any())
        {
            return BadRequest("Dışa aktarılacak eylem kaydı verisi bulunamadı.");
        }

        try
        {
            // Core projesindeki helper sınıfı çağır
            var excelBytes = ExcelExportHelper.ExportActionLogsReportToExcel(logs);

            if (excelBytes.Length == 0)
            {
                return NotFound("Excel dosyası boş oluşturuldu.");
            }

            string fileName = $"Eylem_Kayitlari_Raporu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Excel dosyasını byte dizisi olarak döndürür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Eylem Kayıtları raporu oluşturulurken sunucuda bir hata oluştu: {ex.Message}");
        }
    }
    [HttpGet("export/production-detail/{machineId}/{batchId}")]
    public ActionResult ExportProductionDetailFile(int machineId, string batchId)
    {
        // Veri çekme mantığı, zaten var olan GetProductionDetail metodunun mantığını yansıtır.

        // 1. DTO'yu çekmek için GetProductionDetail metodu çağrılmalıdır.
        // Ancak bu metot bir ActionResult döndürdüğü için, çekirdek mantığını tekrarlıyoruz:

        // 1. Header verisini al
        var reportItem = _productionRepository.GetProductionReport(new ReportFilters { MachineId = machineId, BatchNo = batchId, StartTime = DateTime.MinValue, EndTime = DateTime.MaxValue })
                                            .FirstOrDefault();
        if (reportItem == null) return NotFound("Rapor başlığı bulunamadı.");

        // 2. Adım Detayları (CS1061 hatası çözümü: DTO'ya map etme)
        var stepDetails = _productionRepository.GetProductionStepDetails(batchId, machineId)
            .Select(s => new ExcelExportHelper.ProductionStepDetailDto
            {
                StepNumber = s.StepNumber,
                StepName = s.StepName,
                TheoreticalTime = s.TheoreticalTime,
                WorkingTime = s.WorkingTime,
                StopTime = s.StopTime,
                DeflectionTime = s.DeflectionTime,
                TheoreticalDurationSeconds = TimeSpan.TryParse(s.TheoreticalTime, out var tt) ? tt.TotalSeconds : 0,
                Temperature = 90.5 // Örnek/Hesaplanmış Sıcaklık (GetProductionDetail'deki placeholder)
            }).ToList();

        // 3. Alarm Detayları (CS1061 hatası çözümü: DTO'ya map etme)
        var alarmDetails = _alarmRepository.GetAlarmDetailsForBatch(batchId, machineId)
            .Select((a, index) => new ExcelExportHelper.AlarmDetailDto
            {
                AlarmTime = DateTime.Now.AddMinutes(-index * 5), // Örnek zaman
                AlarmType = "Makine Alarmı", // Örnek tip
                AlarmDescription = a.AlarmDescription,
                Duration = TimeSpan.FromMinutes(index + 1) // Örnek süre
            }).ToList();

        // 4. DTO'yu oluştur
        var detailData = new ExcelExportHelper.ProductionDetailDto
        {
            Header = reportItem,
            Steps = stepDetails,
            Alarms = alarmDetails,
            LogData = new List<ExcelExportHelper.TrendDataPoint>(),
            TheoreticalData = new List<ExcelExportHelper.TrendDataPoint>()
        };

        try
        {
            // 5. Excel'i oluştur
            var excelBytes = ExcelExportHelper.ExportProductionDetailToExcel(detailData);

            if (excelBytes.Length == 0)
            {
                return NotFound("Excel dosyası boş oluşturuldu.");
            }

            string fileName = $"Uretim_Detayi_{batchId}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            // Excel dosyasını byte dizisi olarak döndürür
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Üretim detayı Excel raporu oluşturulurken sunucuda bir hata oluştu: {ex.Message}");
        }
    }
}