using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.WebAPI.Controllers
{
    // Login isteğini karşılamak için bir model oluşturuyoruz
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

        public AuthController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Kullanıcı adı ve şifre gereklidir.");
            }

            bool isValid = _userRepository.ValidateUser(model.Username, model.Password);

            if (isValid)
            {
                // Başarılı giriş durumunda kullanıcı bilgilerini de döndürebiliriz.
                // Şimdilik sadece "OK" (başarılı) durum kodu gönderiyoruz.
                // İleride buradan bir token (anahtar) üreterek Flutter'a gönderebilirsiniz.
                return Ok(new { message = "Giriş başarılı" });
            }
            else
            {
                // 401 Unauthorized, "Giriş yetkisi yok" anlamına gelir.
                return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre" });
            }
        }
    }
}