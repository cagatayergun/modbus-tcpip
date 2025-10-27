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
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private bool _hasCheckedLocalStorage = false;

        public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        // --- ADIM 1: PRERENDERING GÜVENLİĞİ ---
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_hasCheckedLocalStorage)
            {
                return new AuthenticationState(_currentUser);
            }
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }

        // --- ADIM 2: GERÇEK KONTROL (MainLayout'tan çağrılır) ---
        public async Task InitializeAuthenticationStateAsync()
        {
            if (_hasCheckedLocalStorage) return;

            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                _currentUser = _anonymous;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
                _currentUser = new ClaimsPrincipal(ParseClaimsFromJwt(token));
            }

            _hasCheckedLocalStorage = true;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // --- ADIM 3: GİRİŞ İŞLEMİ (Hata Ayıklama Eklendi) ---
        // --- ADIM 3: GİRİŞ İŞLEMİ (DÜZELTİLDİ: JSON Serileştirme Kontrolü) ---
        public async Task<bool> LoginAsync(string username, string password)
        {
            // ...

            // 1. Payload oluşturma
            // 1. Payload oluşturma (Burası C# olduğu için PascalCase kalmalı, bu doğru)
            var loginPayload = new { Username = username, Password = password };

            // 2. JSON'u API'nin beklediği 'camelCase' formatında serileştir.
            var serializerOptions = new JsonSerializerOptions
            {
                // DÜZELTME: API'nin Program.cs'teki ayarıyla eşleşmesi için 'CamelCase' kullan
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var jsonContent = JsonSerializer.Serialize(loginPayload, serializerOptions);

            // 3. StringContent kullanarak JSON'u HTTP isteğine dönüştürün.
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // 4. İsteği gönder
            var response = await _httpClient.PostAsync("api/auth/login", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                // HATA DURUMU: 400, 401, 500 vb. durum kodları
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[API HATA] HTTP Status: {response.StatusCode}. Yanıt İçeriği: {errorContent}");

                // Login.razor'da gösterilen hata mesajını tetikler
                return false;
            }

            // BAŞARILI HTTP DURUMU (200 OK)
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[API BAŞARILI] Gelen JSON: {jsonResponse}");

            LoginResponseModel loginResult;
            try
            {
                // Gelen JSON'u okurken, API'nin yanıt formatına karşı büyük/küçük harf duyarsızlığı ile okumaya devam edin.
                loginResult = JsonSerializer.Deserialize<LoginResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JSON HATA] API yanıtı LoginResponseModel'e dönüştürülemedi: {ex.Message}");
                return false;
            }

            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                Console.WriteLine($"[TOKEN HATA] API'den token gelmedi.");
                return false;
            }

            // --- BAŞARILI GİRİŞ VE TOKEN İŞLEME ---
            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            _currentUser = new ClaimsPrincipal(ParseClaimsFromJwt(loginResult.Token));
            _hasCheckedLocalStorage = true;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            _currentUser = _anonymous;
            _hasCheckedLocalStorage = true;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        // ... (ParseClaimsFromJwt metodu aynı kalır) ...
        private static ClaimsIdentity ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            // 1. Base64Url formatını standart Base64 formatına çevir
            payload = payload.Replace('-', '+').Replace('_', '/');

            // 2. Eksik olan '=' dolgu karakterlerini ekle
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            // 3. Artık standart Base64'e dönen string'i çöz
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
