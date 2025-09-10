using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;
using System.Collections.Generic;

namespace TekstilScada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Tüm kullanıcıları listeler
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Kullanıcılar getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        // Bir kullanıcıyı kullanıcı adına göre getirir
        // Not: Mevcut UserRepository'de GetUserById metodu bulunmuyor. Bunun yerine GetUserByUsername kullanılabilir.
        [HttpGet("{username}")]
        public IActionResult GetUserByUsername(string username)
        {
            try
            {
                var user = _userRepository.GetUserByUsername(username);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Kullanıcı getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        // Yeni bir kullanıcı ekler veya mevcut bir kullanıcıyı günceller
        // Not: Mevcut UserRepository'de SaveUser yerine AddUser ve UpdateUser metotları kullanılıyor.
        // Bu yüzden bu uç nokta SaveUser gibi davranacak şekilde ayarlandı.
        [HttpPost]
        public IActionResult SaveOrUpdateUser([FromBody] User user, [FromQuery] string? password, [FromQuery] List<int> roleIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (user.Id == 0)
                {
                    // Yeni kullanıcı ekleme
                    _userRepository.AddUser(user, password, roleIds);
                }
                else
                {
                    // Mevcut kullanıcıyı güncelleme
                    _userRepository.UpdateUser(user, roleIds, password);
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Kullanıcı kaydedilirken bir hata oluştu: {ex.Message}");
            }
        }

        // Bir kullanıcıyı siler
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userRepository.DeleteUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Kullanıcı silinirken bir hata oluştu: {ex.Message}");
            }
        }

        // Tüm rolleri getirir
        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = _userRepository.GetAllRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Roller getirilirken bir hata oluştu: {ex.Message}");
            }
        }
    }
}