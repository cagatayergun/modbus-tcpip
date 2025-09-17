// Services/AuthService.cs
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.Services
{
    /// <summary>
    /// O an giriş yapmış olan kullanıcının bilgilerini global olarak tutar.
    /// </summary>
    public static class CurrentUser
    {
        private static User _user; // Yeni bir özel alan tanımlayın

        public static User User
        {
            get => _user;
            set
            {
                _user = value;
                // Hata ayıklama için bu satırı ekleyin
               
            }
        }
        public static bool IsLoggedIn => User != null;

        public static void Login(User user)
        {
            User = user;
        }

        public static void Logout()
        {
            User = null;
        }

        public static bool HasRole(string roleName)
        {
            if (!IsLoggedIn) return false;
            // Roller null değilse kontrol et
            return User.Roles != null && User.Roles.Exists(r => r.RoleName.Equals(roleName, System.StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Kullanıcı giriş ve çıkış işlemlerini yönetir.
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService()
        {
            _userRepository = new UserRepository();
        }

        public bool Login(string username, string password)
        {
            // 1. Kullanıcı adı ve şifre veritabanında geçerli mi diye kontrol et.
            bool isValid = _userRepository.ValidateUser(username, password);

            if (isValid)
            {
                // 2. DÜZELTME: GetUserById yerine GetUserByUsername metodu kullanılıyor.
                User user = _userRepository.GetUserByUsername(username);
                if (user != null)
                {
                    // 3. Kullanıcı bilgilerini global oturuma kaydet.
                    CurrentUser.Login(user);
                    return true;
                }
            }

            // Başarısız giriş durumunda oturumu temizle.
            CurrentUser.Logout();
            return false;
        }
      
    }
}
