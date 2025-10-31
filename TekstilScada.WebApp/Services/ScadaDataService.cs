// Dosya: TekstilScada.WebApp/Services/ScadaDataService.cs (SON KARARLI SÜRÜM)

using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TekstilScada.Models;
using TekstilScada.Repositories;
using System.Text.Json;
// DTO'lar, global namespace'de kalmalı4
// 1. TrendDataPoint (CS0234 hatasını çözmek için)
public class TrendDataPoint
{
    public DateTime Timestamp { get; set; }
    public double Temperature { get; set; }
    public double Rpm { get; set; }
    public double WaterLevel { get; set; }
}

// 2. Zenginleştirilmiş ProductionStepDetail DTO (CS1061 hatalarını çözer)
public class ProductionStepDetailDto : TekstilScada.Models.ProductionStepDetail
{
    public double TheoreticalDurationSeconds { get; set; } = 0; // Hesaplanan teorik süre
    public double Temperature { get; set; } = 0; // Adıma ait sıcaklık (eğri için değil, o anki)
    public string StepDescription => StepName; // UI için takma ad
}

// 3. Zenginleştirilmiş AlarmDetail DTO (CS1061 hatalarını çözer)
public class AlarmDetailDto
{
    public DateTime AlarmTime { get; set; } = DateTime.MinValue;
    public string AlarmType { get; set; } = string.Empty;
    public string AlarmDescription { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
}

// 4. Ana API Yanıt Modeli
public class ProductionDetailDto
{
    public TekstilScada.Models.ProductionReportItem Header { get; set; } = new();
    public List<ProductionStepDetailDto> Steps { get; set; } = new();
    public List<AlarmDetailDto> Alarms { get; set; } = new();
    public List<TrendDataPoint> LogData { get; set; } = new();
    public List<TrendDataPoint> TheoreticalData { get; set; } = new();
}
public class GeneralDetailedConsumptionFilters
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<int>? MachineIds { get; set; }
}
public class ActionLogFilters
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Username { get; set; }
    public string? Details { get; set; }
}
public class HourlyConsumptionData
{
    public double Saat { get; set; }
    public double ToplamElektrik { get; set; }
    public double ToplamSu { get; set; }
    public double ToplamBuhar { get; set; }
}

public class HourlyOeeData
{
    public double Saat { get; set; }
    public double AverageOEE { get; set; }
}
public class ReportFilters1
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int? MachineId { get; set; } // Tek makine filtresi için
   // public List<int>? MachineIds { get; set; } // KRİTİK DÜZELTME: Sparkline için gerekli alan
}
// TekstilScada.Models.TopAlarmData'nın kullanıldığı varsayılmıştır.
public class JsReadyTrendDataPoint
{
    public DateTime Timestamp { get; set; }
    public double TimestampOADate { get; set; } // KRİTİK EKLENTİ
    public double Temperature { get; set; } // JS tarafında ondalık sorunu yaşamamak için
    public double Rpm { get; set; }
    public double WaterLevel { get; set; }
}
public class StepTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class SaveLayoutRequest
{
    public string LayoutName { get; set; }
    public string MachineSubType { get; set; }
    public int StepTypeId { get; set; }
    public string LayoutJson { get; set; }
}
public class GeneralConsumptionExportDto
{
    public List<ProductionReportItem>? Items { get; set; }
    public string? ConsumptionType { get; set; }
}
namespace TekstilScada.WebApp.Services
{
    // KRİTİK GÜNCELLEME: IAsyncDisposable arayüzünü uyguluyoruz
    public class ScadaDataService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;
        public ConcurrentDictionary<int, FullMachineStatus> MachineData { get; private set; } = new();
        // Dashboard için grup bilgisi önbelleği
        public ConcurrentDictionary<int, Machine> MachineDetailsCache { get; private set; } = new();

        public event Action? OnDataUpdated;

        public ScadaDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        // KRİTİK GÜNCELLEME: InitializeAsync metodunda agresif temizlik
        public async Task InitializeAsync()
        {
            var hubUrl = new Uri(_httpClient.BaseAddress!, "/scadaHub");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<FullMachineStatus>("ReceiveMachineUpdate", (status) =>
            {
                MachineData[status.MachineId] = status;
                OnDataUpdated?.Invoke();
            });
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                _hubConnection = null;
                Console.WriteLine($"SignalR bağlantı hatası: {ex.Message}");
            }
        }

        // KRİTİK METOT: IAsyncDisposable uygulaması (Çöküşleri ve refresh hatalarını çözer)
        public async ValueTask DisposeAsync()
        {
            var hub = _hubConnection;
            _hubConnection = null; // CRITICAL: Referansı hemen null yap.

            if (hub is not null)
            {
                try
                {
                    // Bağlantıyı durdur ve kaynakları serbest bırak.
                    await hub.StopAsync();
                    await hub.DisposeAsync();
                    Console.WriteLine("SignalR bağlantısı güvenle kapatıldı ve atıldı.");
                }
                catch { /* Hataları yut */ }
            }
        }

        // --- Diğer Metotlar (Aynı Kalır) ---

        public async Task<List<Machine>?> GetMachinesAsync()
        {
            try
            {
                var machines = await _httpClient.GetFromJsonAsync<List<Machine>>("api/machines");
                if (machines != null)
                {
                    MachineDetailsCache.Clear();
                    foreach (var m in machines)
                    {
                        MachineDetailsCache.TryAdd(m.Id, m);
                    }
                }
                return machines;
            }
            catch (Exception ex) { Console.WriteLine($"Makine listesi alınamadı: {ex.Message}"); return null; }
        }

        public async Task<Machine?> AddMachineAsync(Machine machine)
        {
            var response = await _httpClient.PostAsJsonAsync("api/machines", machine);
            return await response.Content.ReadFromJsonAsync<Machine>();
        }

        public async Task<bool> UpdateMachineAsync(Machine machine)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/machines/{machine.Id}", machine);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMachineAsync(int machineId)
        {
            var response = await _httpClient.DeleteAsync($"api/machines/{machineId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<User>?> GetUsersAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<List<User>>("api/users"); }
            catch (Exception ex) { Console.WriteLine($"Kullanıcılar alınamadı: {ex.Message}"); return null; }
        }

        public async Task<List<ScadaRecipe>?> GetRecipesAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<List<ScadaRecipe>>("api/recipes"); }
            catch (Exception ex) { Console.WriteLine($"Reçete listesi alınamadı: {ex.Message}"); return null; }
        }

        public async Task<ScadaRecipe?> GetRecipeDetailsAsync(int recipeId)
        {
            try { return await _httpClient.GetFromJsonAsync<ScadaRecipe>($"api/recipes/{recipeId}"); }
            catch (Exception ex) { Console.WriteLine($"Reçete detayı alınamadı: {ex.Message}"); return null; }
        }

        public async Task<ScadaRecipe?> SaveRecipeAsync(ScadaRecipe recipe)
        {
            var response = await _httpClient.PostAsJsonAsync("api/recipes", recipe);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<ScadaRecipe>() : null;
        }

        public async Task<bool> DeleteRecipeAsync(int recipeId)
        {
            var response = await _httpClient.DeleteAsync($"api/recipes/{recipeId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendRecipeToPlcAsync(int recipeId, int machineId)
        {
            var response = await _httpClient.PostAsync($"api/recipes/{recipeId}/send-to-plc/{machineId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<ScadaRecipe?> ReadRecipeFromPlcAsync(int machineId)
        {
            try { return await _httpClient.GetFromJsonAsync<ScadaRecipe>($"api/recipes/read-from-plc/{machineId}"); }
            catch { return null; }
        }

        public async Task<List<ProductionReportItem>?> GetProductionReportAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/production", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası: {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");
                return new List<ProductionReportItem>();
            }

            return await response.Content.ReadFromJsonAsync<List<ProductionReportItem>>();
        }

        public async Task<List<string>?> GetHmiRecipesAsync(int machineId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<string>>($"api/ftp/list/{machineId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HMI reçeteleri alınamadı: {ex.Message}");
                return new List<string> { $"Hata: {ex.Message}" };
            }
        }

        public async Task<List<AlarmReportItem>?> GetAlarmReportAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/alarms", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Alarm Raporu): {response.StatusCode}");
                return new List<AlarmReportItem>();
            }

            return await response.Content.ReadFromJsonAsync<List<AlarmReportItem>>();
        }

        public async Task<List<OeeData>?> GetOeeReportAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/dashboard/oee-report", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (OEE Raporu): {response.StatusCode}");
                return new List<OeeData>();
            }

            return await response.Content.ReadFromJsonAsync<List<OeeData>>();
        }

        public async Task<List<object>?> GetTrendDataAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/trend", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Trend Raporu): {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<object>>();
        }

        public async Task<List<ProductionReportItem>?> GetRecipeConsumptionHistoryAsync(int recipeId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ProductionReportItem>>($"api/recipes/{recipeId}/usage-history");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reçete kullanım geçmişi alınamadı: {ex.Message}");
                return null;
            }
        }

        public async Task<ManualConsumptionSummary?> GetManualConsumptionReportAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/manual-consumption", filters);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Manuel Tüketim): {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ManualConsumptionSummary>();
        }

        public async Task<ConsumptionTotals?> GetConsumptionTotalsAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/consumption-totals", filters);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Genel Tüketim): {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ConsumptionTotals>();
        }

        public async Task<List<ProductionReportItem>?> GetGeneralDetailedConsumptionReportAsync(GeneralDetailedConsumptionFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/general-detailed", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Genel Detaylı Tüketim): {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<ProductionReportItem>>();
        }

        public async Task<List<TekstilScada.Core.Models.ActionLogEntry>?> GetActionLogsAsync(ActionLogFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/action-logs", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Eylem Kayıtları): {response.StatusCode}");
                return new List<TekstilScada.Core.Models.ActionLogEntry>();
            }

            return await response.Content.ReadFromJsonAsync<List<TekstilScada.Core.Models.ActionLogEntry>>();
        }

        public async Task<List<HourlyConsumptionData>?> GetHourlyConsumptionAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<HourlyConsumptionData>>("api/dashboard/hourly-consumption");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Saatlik tüketim verileri alınamadı: {ex.Message}");
                return null;
            }
        }

        public async Task<List<HourlyOeeData>?> GetHourlyOeeAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<HourlyOeeData>>("api/dashboard/hourly-oee");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Saatlik OEE verileri alınamadı: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TopAlarmData>?> GetTopAlarmsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<TopAlarmData>>("api/dashboard/top-alarms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Popüler alarmlar alınamadı: {ex.Message}");
                return null;
            }
        }
      
        // Varsayım: ScadaDataService sınıfınızın içine eklenmiştir.
        // Varsayım: ScadaRecipe, Machine, HmiRecipeInfo ve API'dan beklenen dönüş tiplerine erişiminiz var.

        // WinForms: LoadHmiRecipes -> plcManager.ReadRecipeNamesFromPlcAsync() mantığına karşılık gelir
        // Beklenen API Dönüşü: Dictionary<int, string> { SlotNumber: RecipeName }
        public async Task<Dictionary<int, string>?> GetHmiRecipeNamesAsync(int machineId)
        {
            try
            {
                // Örnek API yolu: /api/ftp/hmi-recipe-names/{machineId}
                var response = await _httpClient.GetFromJsonAsync<Dictionary<int, string>>($"api/ftp/hmi-recipe-names/{machineId}");
                return response;
            }
            catch (HttpRequestException ex)
            {
                // Loglama veya hata yönetimi eklenebilir
                Console.WriteLine($"API çağrısı sırasında hata: {ex.Message}");
                return null;
            }
        }

        // WinForms: lstHmiRecipes_SelectedIndexChanged -> FtpService.DownloadFileAsync & RecipeCsvConverter.ToRecipe mantığına karşılık gelir
        // Beklenen API Dönüşü: ScadaRecipe (Adımları içerir)
        public async Task<ScadaRecipe?> GetHmiRecipePreviewAsync(int machineId, string remoteFileName)
        {
            try
            {
                // API'ya dosya adını sorgu parametresi veya gövde olarak gönderin
                // Örnek API yolu: /api/ftp/hmi-recipe-preview/{machineId}?fileName={remoteFileName}
                var response = await _httpClient.GetFromJsonAsync<ScadaRecipe>($"api/ftp/hmi-recipe-preview/{machineId}?fileName={remoteFileName}");
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API çağrısı sırasında hata: {ex.Message}");
                return null;
            }
        }

        // WinForms: btnSend_Click -> _transferService.QueueSequentiallyNamedSendJobs mantığına karşılık gelir
        public async Task<bool> QueueSequentiallyNamedSendJobsAsync(List<int> recipeIds, List<int> machineIds, int startNumber)
        {
            try
            {
                var payload = new
                {
                    RecipeIds = recipeIds,
                    MachineIds = machineIds,
                    StartNumber = startNumber
                };

                // Örnek API yolu: /api/ftp/queue-send-jobs
                var response = await _httpClient.PostAsJsonAsync("api/ftp/queue-send-jobs", payload);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API çağrısı sırasında hata: {ex.Message}");
                return false;
            }
        }

        // WinForms: btnReceive_Click -> _transferService.QueueReceiveJobs mantığına karşılık gelir
        public async Task<bool> QueueReceiveJobsAsync(List<string> fileNames, int machineId)
        {
            try
            {
                var payload = new
                {
                    FileNames = fileNames,
                    MachineId = machineId
                };

                // Örnek API yolu: /api/ftp/queue-receive-jobs
                var response = await _httpClient.PostAsJsonAsync("api/ftp/queue-receive-jobs", payload);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API çağrısı sırasında hata: {ex.Message}");
                return false;
            }
        }
        public async Task<ProductionDetailDto?> GetProductionDetailAsync(int machineId, string batchId)
        {
            try
            {
                var url = $"api/reports/production-detail/{machineId}/{batchId}";
                return await _httpClient.GetFromJsonAsync<ProductionDetailDto>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Üretim detayı alınamadı: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ExportProductionDetailAsync(int machineId, string batchId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/reports/export-production-detail/{machineId}/{batchId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excel dışa aktarımı başarısız: {ex.Message}");
                return false;
            }
        }
        public async Task<string> GetLayoutJsonAsync(string machineSubType, int stepId)
        {
            try
            {
                // API'nin iç implementasyonunun değişmesi, bu çağrıyı etkilemez.
                var response = await _httpClient.GetAsync($"api/Json/config/layout/{machineSubType}/{stepId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    
                    return string.Empty;
                }
                else
                {
                    // ... (mevcut hata yönetimi) ...
                    throw new Exception($"API call failed: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
               
                throw;
            }
        }

        // --- BİR REÇETE TASARIM EKRANI İÇİN GEREKLİ YENİ METOTLAR ---

        public async Task<List<StepTypeDto>> GetStepTypesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<StepTypeDto>>("api/Json/config/steptypes");
            }
            catch (Exception ex)
            {
           
                return new List<StepTypeDto>(); // Hata durumunda boş liste dön
            }
        }

        public async Task<List<string>> GetMachineSubTypesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<string>>("api/Json/config/machinesubtypes");
            }
            catch (Exception ex)
            {
                
                return new List<string> { "DEFAULT" }; // Hata durumunda sadece DEFAULT dön
            }
        }

        public async Task<bool> SaveLayoutAsync(SaveLayoutRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Json/config/savelayout", request);
                response.EnsureSuccessStatusCode(); // Hata varsa exception fırlatır
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
        public async Task<byte[]> ExportProductionReportAsync(List<ProductionReportItem> reportItems)
        {
            // _serializerOptions artık tanımlı
            var json = JsonSerializer.Serialize(reportItems, _serializerOptions);

            // KRİTİK DÜZELTME 2 (404/Not Found HATASI ÇÖZÜMÜ: api/ ön eki eklendi)
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/reports/export/production", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new Exception($"Rapor dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            // Başarılı olursa, response'un içeriğini doğrudan byte dizisi olarak döndür
            return await response.Content.ReadAsByteArrayAsync();
        }
        public async Task<byte[]> ExportAlarmReportAsync(List<AlarmReportItem> reportItems)
        {
            var json = JsonSerializer.Serialize(reportItems, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API endpoint'ini çağır
            var response = await _httpClient.PostAsync("api/reports/export/alarms", content); // YENİ ENDPOINT

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Alarm raporu dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        public async Task<byte[]> ExportOeeReportAsync(List<OeeData> reportItems)
        {
            var json = JsonSerializer.Serialize(reportItems, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API endpoint'ini çağır
            var response = await _httpClient.PostAsync("api/reports/export/oee", content); // YENİ ENDPOINT

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"OEE raporu dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        // YENİ METOT: Manuel Tüketim Raporunu Excel'e Aktarma API Çağrısı
        public async Task<byte[]> ExportManualConsumptionReportAsync(ManualConsumptionSummary summary)
        {
            // API, bir ManualConsumptionSummary nesnesini bekler.
            var json = JsonSerializer.Serialize(summary, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API endpoint'ini çağır
            var response = await _httpClient.PostAsync("api/reports/export/manual-consumption", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Manuel Tüketim raporu dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        public async Task<byte[]> ExportGeneralDetailedConsumptionReportAsync(GeneralConsumptionExportDto exportData)
        {
            // API, GeneralConsumptionExportDto nesnesini bekler.
            var json = JsonSerializer.Serialize(exportData, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API endpoint'ini çağır
            var response = await _httpClient.PostAsync("api/reports/export/general-detailed", content); // YENİ ENDPOINT

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Genel Tüketim raporu dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        public async Task<byte[]> ExportActionLogsReportAsync(List<TekstilScada.Core.Models.ActionLogEntry> logs)
        {
            // API, ActionLogEntry listesini bekler.
            var json = JsonSerializer.Serialize(logs, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API endpoint'ini çağır
            var response = await _httpClient.PostAsync("api/reports/export/action-logs", content); // YENİ ENDPOINT

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Eylem Kayıtları raporu dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
        // YENİ METOT: Üretim Detayını Excel'e Aktarma API Çağrısı (Byte dizisi döner)
        public async Task<byte[]> ExportProductionDetailFileAsync(int machineId, string batchId)
        {
            // API endpoint'ini çağır, GET request'i ile veriyi alıyoruz.
            var response = await _httpClient.GetAsync($"api/reports/export/production-detail/{machineId}/{batchId}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Üretim detayı dışa aktarma başarısız oldu: {response.ReasonPhrase}. Detay: {error}");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}