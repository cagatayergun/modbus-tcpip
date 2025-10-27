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
using TekstilScada.Core.Models; // Model referansınız
using System.Text.Json.Serialization; // <-- 1. BU SATIRI EKLEYİN

using TekstilScada.Repositories;

namespace TekstilScada.WebAPI.Controllers
{
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
                // 1. Kullanıcıyı doğrula
                bool isValid = _userRepository.ValidateUser(model.Username, model.Password);

                if (!isValid)
                {
                    _logger.LogWarning($"[AUTH FAILED] Geçersiz giriş denemesi: Kullanıcı Adı: {model.Username}");
                    return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre" });
                }

                // 2. Kullanıcı nesnesini al
                var user = _userRepository.GetUserByUsername(model.Username);
                if (user == null)
                {
                    _logger.LogError($"[AUTH CRITICAL] Doğrulama başarılı ama kullanıcı nesnesi bulunamadı: {model.Username}");
                    return Unauthorized(new { message = "Kullanıcı bilgileri bulunamadı." });
                }

                // 3. JWT Üretimi
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

                // Rolleri taleplere ekle
                if (user.Roles != null)
                {
                    claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.RoleName)));
                }

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(7), // 7 gün geçerli token
                    signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation($"[AUTH SUCCESS] Kullanıcı '{model.Username}' giriş yaptı ve token üretildi.");

                return Ok(new
                {
                    Token = tokenString,
                    Message = "Giriş başarılı",
                    // Blazor client'ının kullanması için ek bilgiler
                    Username = user.Username,
                    Roles = user.Roles?.Select(r => r.RoleName)
                });
            }
            catch (Exception ex)
            {
                // KRİTİK HATA YAKALANDI: Genellikle LiteDB erişimi veya eksik JWT anahtarı.
                _logger.LogCritical(ex, $"[AUTH CRITICAL ERROR] Giriş sırasında beklenmedik bir hata oluştu. Kullanıcı: {model.Username}");

                // TEST AMAÇLI GEÇİCİ DEĞİŞİKLİK:
                return StatusCode(500, new
                {
                    message = "Sunucu hatası: Giriş yapılamadı.",
                    // ex.Message'ı yanıt gövdesine ekle
                    detailedError = ex.Message
                });
            }
        }
    }
}
