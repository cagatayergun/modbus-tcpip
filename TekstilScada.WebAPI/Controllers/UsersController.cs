// Dosya: TekstilScada.WebAPI/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;

namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            return Ok(_userRepository.GetAllUsers());
        }
    }
}