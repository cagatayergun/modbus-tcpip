// TekstilScada.WebApp/Services/CustomAuthStateProvider.cs

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using TekstilScada.Models; // Bu using TekstilScada.Core.Models ise düzeltilmeli
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.Json;

namespace TekstilScada.WebApp.Services
{
    // API'den dönen Token modelini varsayalım
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public string Username { get; set; } // Bu alanı ekleyin
        public List<string> Roles { get; set; } // Bu alanı ekleyin
    }

    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        // *** DEĞİŞİKLİK 1: Bu iki değişkeni (currentUser ve hasChecked) SİLEBİLİRSİN ***
        // private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        // private bool _hasCheckedLocalStorage = false;
        // (LoginAsync ve LogoutAsync içinde _currentUser ve _hasCheckedLocalStorage kullanan satırları da silmen gerekecek)


        public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        // --- ADIM 1: GetAuthenticationStateAsync METODUNU TAMAMEN GÜNCELLE ---
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                // Token yoksa, anonim kullanıcı döndür
                return new AuthenticationState(_anonymous);
            }

            // Token varsa, geçerli mi diye kontrol et
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                var userClaims = ParseClaimsFromJwt(token);
                var claimsPrincipal = new ClaimsPrincipal(userClaims);

                // Geçerli token ile kullanıcı bilgisi döndür
                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                // Token parse edilemedi (örn. süresi dolmuş veya geçersiz)
                Console.WriteLine($"JWT Parse Hatası: {ex.Message}");
                await _localStorage.RemoveItemAsync("authToken"); // Bozuk token'ı temizle
                _httpClient.DefaultRequestHeaders.Authorization = null;

                // Hata durumunda anonim kullanıcı döndür
                return new AuthenticationState(_anonymous);
            }
        }

        // --- ADIM 2: BU METODA ARTIK İHTİYAÇ YOK ---
        /*
        public async Task InitializeAuthenticationStateAsync()
        {
           // BU METODUN TAMAMINI SİLEBİLİRSİN
           // VEYA MainLayout.razor içinden buna yapılan çağrıyı kaldırdığından emin ol.
        }
        */

        // --- ADIM 3: LoginAsync METODUNU GÜNCELLE ---
        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginPayload = new { Username = username, Password = password };
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var jsonContent = JsonSerializer.Serialize(loginPayload, serializerOptions);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                return false; // Hata durumları
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            LoginResponseModel loginResult;
            try
            {
                loginResult = JsonSerializer.Deserialize<LoginResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception)
            {
                return false; // JSON parse hatası
            }

            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                return false; // Token gelmedi
            }

            // --- Başarılı Giriş ---
            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            // *** DEĞİŞİKLİK 2: Login olunca durumu Blazor'a bildir ***
            var userClaims = ParseClaimsFromJwt(loginResult.Token);
            var claimsPrincipal = new ClaimsPrincipal(userClaims);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

            return true;
        }

        // --- ADIM 4: LogoutAsync METODUNU GÜNCELLE ---
        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // *** DEĞİŞİKLİK 3: Çıkış yapınca durumu Blazor'a bildir ***
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        // ... (ParseClaimsFromJwt metodu aynı kalır) ...
        private static ClaimsIdentity ParseClaimsFromJwt(string jwt)
        {
            // (Bu metotta değişiklik yok)
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }
            var jsonBytes = Convert.FromBase64String(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Name, out object username);
            if (username != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, username.ToString()));
            }
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);
            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());
                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }
            }
            return new ClaimsIdentity(claims, "jwt");
        }
    }
}