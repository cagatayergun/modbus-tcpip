// Dosya: TekstilScada.WebAPI/Controllers/FtpController.cs

using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Services;

namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FtpController : ControllerBase
    {
        private readonly MachineRepository _machineRepository;

        public FtpController(MachineRepository machineRepository)
        {
            _machineRepository = machineRepository;
        }

        [HttpGet("list/{machineId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetHmiRecipes(int machineId)
        {
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            if (machine == null) return NotFound("Makine bulunamadı.");
            if (string.IsNullOrEmpty(machine.IpAddress)) return BadRequest("Makine için FTP adresi tanımlanmamış.");

            try
            {
                var ftpService = new FtpService(machine.IpAddress, machine.FtpUsername, machine.FtpPassword);
                var files = await ftpService.ListDirectoryAsync("/");
                var recipeFiles = files
                    .Where(f => f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(f => f)
                    .ToList();
                return Ok(recipeFiles);
            }
            catch (Exception ex)
            {
                // Hata olduğunda programı çökertmek yerine 500 kodu ve hata mesajını döndür.
                return StatusCode(500, $"FTP sunucusuna bağlanılamadı: {ex.Message}");
            }
        }
    }
}