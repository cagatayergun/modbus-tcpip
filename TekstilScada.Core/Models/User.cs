// Models/User.cs
using System.Collections.Generic;
using System.Data;

namespace TekstilScada.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public List<Role> Roles { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; } // Refresh Token'ın geçerlilik süresi
        public User()
        {
            Roles = new List<Role>();
        }
    }
}