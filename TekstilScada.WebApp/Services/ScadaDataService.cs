// Dosya: TekstilScada.WebApp/Services/ScadaDataService.cs (TAM SÜRÜM)

using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using TekstilScada.Models;
using TekstilScada.Repositories;
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
namespace TekstilScada.WebApp.Services
{
    public class ScadaDataService
    {
        private HubConnection? _hubConnection;
        private readonly HttpClient _httpClient;

        public ConcurrentDictionary<int, FullMachineStatus> MachineData { get; private set; } = new();
        public event Action? OnDataUpdated;

        public ScadaDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
                Console.WriteLine($"SignalR bağlantı hatası: {ex.Message}");
            }
        }

        public async Task<List<Machine>?> GetMachinesAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<List<Machine>>("api/machines"); }
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

            // --- HATA DETAYINI YAKALAMAK İÇİN BU BLOK EKLENDİ ---
            if (!response.IsSuccessStatusCode)
            {
                // Eğer istek başarılı değilse (400 hatası gibi), sunucunun gönderdiği
                // detaylı hata mesajını oku ve konsola yaz.
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası: {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");

                // Hata durumunda boş bir liste döndürerek arayüzün çökmesini engelle.
                return new List<ProductionReportItem>();
            }
            // --- BLOK SONU ---

            return await response.Content.ReadFromJsonAsync<List<ProductionReportItem>>();
        }

        // === EKSİK OLAN METOT BURADA ===
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
        // YENİ METOT: Alarm Raporu Getirme
        public async Task<List<AlarmReportItem>?> GetAlarmReportAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/alarms", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Alarm Raporu): {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");
                return new List<AlarmReportItem>();
            }

            return await response.Content.ReadFromJsonAsync<List<AlarmReportItem>>();
        }
        // YENİ METOT: OEE Raporu Getirme
        public async Task<List<OeeData>?> GetOeeReportAsync(ReportFilters filters)
        {
            // DÜZELTME: Metot adı değişikliğine uyum sağlamak için URL güncellendi
            var response = await _httpClient.PostAsJsonAsync("api/dashboard/oee-report", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (OEE Raporu): {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");
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
                Console.WriteLine($"Hata Detayı: {errorContent}");
                return null;
            }

            // API'den gelen veriyi List<object> (yani dinamik olarak) alıyoruz.
            // Bu, client'ta doğru modele dönüştürülecektir.
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
        // YENİ METOT: Manuel Tüketim Raporu Getirme
        public async Task<ManualConsumptionSummary?> GetManualConsumptionReportAsync(ReportFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/manual-consumption", filters);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Veri bulunamadığında null döndürürüz.
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Manuel Tüketim): {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");
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
                Console.WriteLine($"Hata Detayı: {errorContent}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ConsumptionTotals>();
        }
        // YENİ METOT: Genel Detaylı Tüketim Raporu Getirme
        public async Task<List<ProductionReportItem>?> GetGeneralDetailedConsumptionReportAsync(GeneralDetailedConsumptionFilters filters)
        {
            var response = await _httpClient.PostAsJsonAsync("api/reports/general-detailed", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Genel Detaylı Tüketim): {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<ProductionReportItem>>();
        }
        public async Task<List<TekstilScada.Core.Models.ActionLogEntry>?> GetActionLogsAsync(ActionLogFilters filters)
        {
            // TekstilScada.Core.Models.ActionLogEntry modelini kullanıyoruz.
            var response = await _httpClient.PostAsJsonAsync("api/reports/action-logs", filters);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası (Eylem Kayıtları): {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");
                return new List<TekstilScada.Core.Models.ActionLogEntry>();
            }

            return await response.Content.ReadFromJsonAsync<List<TekstilScada.Core.Models.ActionLogEntry>>();
        }


    }
}