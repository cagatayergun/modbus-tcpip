using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Services;
using Microsoft.AspNetCore.Authorization; // Bu satırı ekle

namespace TekstilScada.Api.Controllers
{
    [ApiController]
    [Route("api/Machine")] // Adresi elle, kesin olarak yazdık.
  
    public class MachineController : ControllerBase
    {
        private readonly PlcPollingService _pollingService;

        public MachineController(PlcPollingService pollingService)
        {
            _pollingService = pollingService;
        }

        [HttpGet("GetAllMachineStatus")]
        public IActionResult GetAllMachineStatus() // Metodun adını da değiştirmek iyi bir pratiktir.
        {
            // Canlı makine verilerini al ve döndür
            var liveData = _pollingService.MachineDataCache.Values;
            return Ok(liveData);
        }
    }
}