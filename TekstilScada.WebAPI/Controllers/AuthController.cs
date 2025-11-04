using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography; // YENİ: Refresh Token üretimi için eklendi
using TekstilScada.Models; // DÜZELTİLDİ: TekstilScada.Core.Models yerine TekstilScada.Models kullanıldı
using System.Text.Json.Serialization;

using TekstilScada.Repositories;

namespace TekstilScada.WebAPI.Controllers
{
    // YENİ MODEL: Login yanıtı için (Blazor'da LoginResponseModel olarak kullanılacak)
    public class AuthResponseModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }

    // YENİ MODEL: Refresh isteği için
    public class RefreshModel
    {
        public string RefreshToken { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserRepository userRepository, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        // YARDIMCI METOT: Refresh Token üretimi
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        // YARDIMCI METOT: JWT üretimi
        private string GenerateJwtToken(User user)
        {
            // JWT anahtarlarının kontrolü
            string jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key appsettings'te eksik.");
            string jwtIssuer = _configuration["Jwt:Issuer"] ?? "TekstilScadaAPI";
            string jwtAudience = _configuration["Jwt:Audience"] ?? "TekstilScadaClient";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (user.Roles != null)
            {
                claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.RoleName)));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15), // Erişim token'ı (Access Token) 15 dakika
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                _logger.LogWarning("[AUTH HATA] Geçersiz istek: Kullanıcı adı veya şifre eksik.");
                return BadRequest("Kullanıcı adı ve şifre gereklidir.");
            }

            try
            {
                // 1. Kullanıcıyı doğrula ve nesnesini al
                bool isValid = _userRepository.ValidateUser(model.Username, model.Password);

                if (!isValid) return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre" });

                var user = _userRepository.GetUserByUsername(model.Username);
                if (user == null) return Unauthorized(new { message = "Kullanıcı bilgileri bulunamadı." });

                // 2. JWT ve Refresh Token Üretimi
                var tokenString = GenerateJwtToken(user);
                var refreshTokenString = GenerateRefreshToken();

                // 3. Refresh Token'ı veritabanına kaydet (UserRepository'ye eklendi)
                _userRepository.UpdateRefreshToken(user.Id, refreshTokenString);

                _logger.LogInformation($"[AUTH SUCCESS] Kullanıcı '{model.Username}' giriş yaptı ve tokenlar üretildi.");

                return Ok(new AuthResponseModel
                {
                    Token = tokenString,
                    RefreshToken = refreshTokenString,
                    Message = "Giriş başarılı",
                    Username = user.Username,
                    Roles = user.Roles?.Select(r => r.RoleName)
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"[AUTH CRITICAL ERROR] Giriş sırasında beklenmedik bir hata oluştu. Kullanıcı: {model.Username}");
                return StatusCode(500, new
                {
                    message = "Sunucu hatası: Giriş yapılamadı.",
                    detailedError = ex.Message
                });
            }
        }

        // YENİ UÇ NOKTA: Token yenileme
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.RefreshToken))
            {
                return BadRequest("Refresh token gereklidir.");
            }

            try
            {
                // 1. Refresh token'ı veritabanında bul (UserRepository'ye eklendi)
                var user = _userRepository.GetUserByRefreshToken(model.RefreshToken);

                if (user == null)
                {
                    _logger.LogWarning($"[REFRESH FAILED] Geçersiz veya süresi dolmuş refresh token kullanıldı.");
                    // İstemcinin zorla logout olması için Unauthorized döndürüyoruz.
                    return Unauthorized(new { message = "Oturum süresi doldu. Lütfen tekrar giriş yapın." });
                }

                // 2. Yeni Access Token ve Refresh Token üret
                var newAccessToken = GenerateJwtToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // 3. Yeni Refresh Token'ı veritabanına kaydet
                _userRepository.UpdateRefreshToken(user.Id, newRefreshToken);

                _logger.LogInformation($"[REFRESH SUCCESS] Kullanıcı '{user.Username}' için token başarıyla yenilendi.");

                return Ok(new AuthResponseModel
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    Message = "Token başarıyla yenilendi.",
                    Username = user.Username,
                    Roles = user.Roles?.Select(r => r.RoleName)
                });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"[REFRESH CRITICAL ERROR] Token yenileme sırasında beklenmedik bir hata oluştu.");
                return StatusCode(500, new { message = "Sunucu hatası: Token yenilenemedi." });
            }
        }
    }
}