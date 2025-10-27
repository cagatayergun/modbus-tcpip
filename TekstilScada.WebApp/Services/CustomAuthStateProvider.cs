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
        public object UserInfo { get; set; } // User yerine daha genel bir tip kullanıldı
        public string Message { get; set; } // API'den gelen hata mesajını yakalamak için eklendi
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
        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginPayload = new { Username = username, Password = password };

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // PascalCase API için uyumlu
            };

            // API'ye istek atıyoruz
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginPayload, serializerOptions);

            if (!response.IsSuccessStatusCode)
            {
                // HATA DURUMU: 401, 500 vb. durum kodları
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
                // JSON'u okurken büyük/küçük harf duyarsızlığı eklendi
                loginResult = JsonSerializer.Deserialize<LoginResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JSON HATA] API yanıtı LoginResponseModel'e dönüştürülemedi: {ex.Message}");
                return false;
            }

            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            {
                // API 200 OK dönse bile, TOKEN gelmediyse (en olası Web API hatası)
                Console.WriteLine($"[TOKEN HATA] API'den token gelmedi. Muhtemel neden: AuthController'da JWT üretimi başarısız.");
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
            var jsonBytes = Convert.FromBase64String(
                payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4)
            );

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
