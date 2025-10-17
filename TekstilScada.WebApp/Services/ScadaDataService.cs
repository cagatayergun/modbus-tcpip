// Dosya: TekstilScada.WebApp/Services/ScadaDataService.cs (SON KARARLI SÜRÜM)

using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using TekstilScada.Models;
using TekstilScada.Repositories;
using System;
using System.Threading.Tasks;

// DTO'lar, global namespace'de kalmalı
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

// TekstilScada.Models.TopAlarmData'nın kullanıldığı varsayılmıştır.

namespace TekstilScada.WebApp.Services
{
    // KRİTİK GÜNCELLEME: IAsyncDisposable arayüzünü uyguluyoruz
    public class ScadaDataService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly HttpClient _httpClient;

        public ConcurrentDictionary<int, FullMachineStatus> MachineData { get; private set; } = new();
        // Dashboard için grup bilgisi önbelleği
        public ConcurrentDictionary<int, Machine> MachineDetailsCache { get; private set; } = new();

        public event Action? OnDataUpdated;

        public ScadaDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // KRİTİK GÜNCELLEME: InitializeAsync metodunda agresif temizlik
        public async Task InitializeAsync()
        {
            // 1. Zaten bağlıysa hiçbir şey yapma.
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                return;
            }

            // 2. Bir bağlantı varsa ancak sağlıklı değilse, onu agresifçe temizle ve sıfırla.
            if (_hubConnection != null)
            {
                try
                {
                    // Bağlantıyı durdur ve kaynakları serbest bırak.
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                }
                catch { /* Hataları yut */ }

                _hubConnection = null; // CRITICAL: Referansı sıfırla
            }

            // 3. Yeni ve temiz bir HubConnection örneği oluştur.
            var hubUrl = new Uri(_httpClient.BaseAddress!, "/scadaHub");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            // 4. Event handler'ı ayarla.
            _hubConnection.On<FullMachineStatus>("ReceiveMachineUpdate", (status) =>
            {
                MachineData[status.MachineId] = status;
                OnDataUpdated?.Invoke();
            });

            // 5. Bağlantıyı başlat.
            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR bağlantısı başarıyla kuruldu/yeniden kuruldu.");
            }
            catch (Exception ex)
            {
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
    }
}