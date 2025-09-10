using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Services;
using Microsoft.AspNetCore.Authorization; // Bu satırı ekle

namespace TekstilScada.Api.Controllers
{
    [Authorize] // Bu etiketi ekle
    [Route("api/[controller]")]
    [ApiController]
    public class MachineController : ControllerBase
    {
        private readonly PlcPollingService _pollingService;

        public MachineController(PlcPollingService pollingService)
        {
            _pollingService = pollingService;
        }

        [HttpGet("live-status")]
        public IActionResult GetLiveStatus()
        {
            // Canlı makine verilerini al ve döndür
            var liveData = _pollingService.MachineDataCache.Values;
            return Ok(liveData);
        }
    }
}