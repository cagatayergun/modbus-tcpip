// TekstilScada.WebApp/Services/CustomAuthStateProvider.cs

using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace TekstilScada.WebApp.Services
{
    // API'den dönen Token modelini WebAPI'ye uyumlu hale getirelim (RefreshToken eklendi)
    public class LoginResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; } // YENİ: WebAPI'den gelen Refresh Token
        public string Message { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }

    // Refresh isteği için basit model
    public class RefreshRequestModel
    {
        public string RefreshToken { get; set; }
    }

    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ILogger<CustomAuthStateProvider> _logger; // <-- YENİ: Logger ekleyin
        public CustomAuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage,
                                       ILogger<CustomAuthStateProvider> logger)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _logger = logger; // <-- YENİ
        }

        // -----------------------------------------------------
        // 1. YARDIMCI METOTLAR (Süre kontrolü ve Yenileme isteği)
        // -----------------------------------------------------

        // Token süresinin dolup dolmadığını kontrol eder. 30 saniye marjı eklenmiştir.
        private bool IsTokenExpired(string token)
        {
            try
            {
                var payload = token.Split('.')[1];
                payload = payload.Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }
                var jsonBytes = Convert.FromBase64String(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs.TryGetValue("exp", out object expValue))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expValue.ToString())).UtcDateTime;

                    // Token 30 saniye içinde dolacaksa yenilemeyi tetikle
                    return expirationTime <= DateTime.UtcNow.AddSeconds(30);
                }
                return true;
            }
            catch
            {
                return true; // Ayrıştırma hatası, token geçersiz sayılır
            }
        }

        // Refresh Token ile yeni token seti almayı dener
        private async Task<bool> RefreshTokenAsync(string refreshToken)
        {
            var refreshPayload = new RefreshRequestModel { RefreshToken = refreshToken };
            var jsonContent = JsonSerializer.Serialize(refreshPayload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            try
            {
                // 🔴 KRİTİK DÜZELTME: Refresh isteği gönderilmeden önce eski Authorization başlığı TEMİZLENMELİDİR.
                _httpClient.DefaultRequestHeaders.Authorization = null; // BU SATIR EKLENMELİDİR.

                // WebAPI'deki /api/auth/refresh ucuna istek gönder
                var response = await _httpClient.PostAsync("api/auth/refresh", httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    // 🔴 Console.WriteLine yerine ILogger kullanın
                    _logger.LogWarning("DEBUG AUTH: Refresh API BAŞARISIZ. Status Code: {StatusCode}. Content: {Content}",
                                       response.StatusCode,
                                       await response.Content.ReadAsStringAsync());
                    return false;
                }

                var refreshResult = await response.Content.ReadFromJsonAsync<LoginResponseModel>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (refreshResult == null || string.IsNullOrEmpty(refreshResult.Token))
                {
                    return false;
                }

                // YENİ TOKENLARI KAYDET VE HTTP CLIENT'I GÜNCELLE
                await _localStorage.SetItemAsync("authToken", refreshResult.Token);
                await _localStorage.SetItemAsync("refreshToken", refreshResult.RefreshToken);

                // Başarılı yenilemeden sonra yeni token'ı tekrar Authorization başlığına ekleyin
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", refreshResult.Token);

                return true; // Yenileme başarılı
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token Yenileme Hatası");
                return false;
            }
        }

        // -----------------------------------------------------
        // 2. KRİTİK METOT: GetAuthenticationStateAsync
        // -----------------------------------------------------

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 1. Token'ları okumayı dene
            string token = null;
            string refreshToken = null;
            try
            {
                token = await _localStorage.GetItemAsync<string>("authToken");
                refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
            }
            catch (Exception ex)
            {
                // Bu (JS Interop) hatası artık olmuyor gibi görünüyor, ancak kalsın.
                _logger.LogError(ex, "DEBUG AUTH: Local Storage'dan token okurken HATA (JS Interop?)");
                return new AuthenticationState(_anonymous);
            }

            // 2. Token var mı kontrol et
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogInformation("DEBUG AUTH: Token veya RefreshToken Local Storage'da bulunamadı. Anonim dönülüyor.");
                return new AuthenticationState(_anonymous);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            _logger.LogInformation("DEBUG AUTH: Tokenlar Local Storage'da bulundu. Geçerlilik kontrol ediliyor.");

            bool needsRefresh = false;
            ClaimsPrincipal claimsPrincipal;

            // 3. ÖNCE TOKEN'I AYRIŞTIRMAYI DENE
            try
            {
                var userClaims = ParseClaimsFromJwt(token);
                claimsPrincipal = new ClaimsPrincipal(userClaims);

                // 3.1. Ayrıştırma başarılıysa, SÜRESİNİ kontrol et
                if (IsTokenExpired(token))
                {
                    _logger.LogInformation("DEBUG AUTH: Access Token süresi dolmuş. Yenileme deneniyor.");
                    needsRefresh = true;
                }
                else
                {
                    _logger.LogInformation("DEBUG AUTH: Token geçerli ve süresi dolmamış.");
                }
            }
            // 4. AYRIŞTIRMA BAŞARISIZ OLURSA (BOZUK TOKEN)
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "DEBUG AUTH: JWT Claims Parse Hatası. Token'ın bozuk olduğu varsayıldı. Yenileme zorlanıyor.");
                needsRefresh = true; // Token bozuk, yenilemeyi zorla
                claimsPrincipal = _anonymous; // Şimdilik anonim ata
            }

            // 5. YENİLEME GEREKİYORSA (Süresi dolduğu için VEYA bozuk olduğu için)
            if (needsRefresh)
            {
                if (await RefreshTokenAsync(refreshToken))
                {
                    _logger.LogInformation("DEBUG AUTH: Refresh Token başarılı! Yeni token ile devam ediliyor.");
                    token = await _localStorage.GetItemAsync<string>("authToken");

                    // Yenileme sonrası yeni token'ı tekrar ayrıştır
                    try
                    {
                        var newUserClaims = ParseClaimsFromJwt(token);
                        claimsPrincipal = new ClaimsPrincipal(newUserClaims);
                    }
                    catch (Exception ex)
                    {
                        // Yeni alınan token bile bozuksa, ciddi bir sorun var. Logout yap.
                        _logger.LogError(ex, "DEBUG AUTH: YENİ ALINAN REFRESH TOKEN BİLE BOZUK. Logout tetikleniyor.");
                        await LogoutAsync();
                        return new AuthenticationState(_anonymous);
                    }
                }
                else
                {
                    // Yenileme başarısız oldu (refresh token da geçersiz)
                    _logger.LogWarning("DEBUG AUTH: Refresh Token BAŞARISIZ OLDU. Logout tetikleniyor.");
                    await LogoutAsync();
                    return new AuthenticationState(_anonymous);
                }
            }

            // 6. Sonuç
            _logger.LogInformation("DEBUG AUTH: Oturum geçerli. ClaimsPrincipal dönülüyor.");
            return new AuthenticationState(claimsPrincipal);
        }

        // -----------------------------------------------------
        // 3. Login ve Logout Metotları
        // -----------------------------------------------------

        public async Task<bool> LoginAsync(string username, string password)
        {
            // ... (Mevcut HTTP POST kodunuz) ...
            var loginPayload = new { Username = username, Password = password };
            var serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonContent = JsonSerializer.Serialize(loginPayload, serializerOptions);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", httpContent);

            if (!response.IsSuccessStatusCode) return false;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loginResult == null || string.IsNullOrEmpty(loginResult.Token)) return false;

            // --- Başarılı Giriş ---
            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            await _localStorage.SetItemAsync("refreshToken", loginResult.RefreshToken); // YENİ: Refresh Token kaydedildi

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

            var userClaims = ParseClaimsFromJwt(loginResult.Token);
            var claimsPrincipal = new ClaimsPrincipal(userClaims);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken"); // YENİ: Refresh Token temizlendi
            _httpClient.DefaultRequestHeaders.Authorization = null;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        // ... (ParseClaimsFromJwt metodu aynı kalır) ...
        private static ClaimsIdentity ParseClaimsFromJwt(string jwt)
        {
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