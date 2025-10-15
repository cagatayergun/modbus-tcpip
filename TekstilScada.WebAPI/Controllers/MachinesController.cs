// Dosya: TekstilScada.WebAPI/Controllers/MachinesController.cs
using Microsoft.AspNetCore.Mvc;
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.Services;

namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachinesController : ControllerBase
    {
        private readonly MachineRepository _machineRepository;
        private readonly PlcPollingService _pollingService;

        public MachinesController(MachineRepository machineRepository, PlcPollingService pollingService)
        {
            _machineRepository = machineRepository;
            _pollingService = pollingService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Machine>> GetAllMachines()
        {
            return Ok(_machineRepository.GetAllMachines()); // Tüm makineleri getir
        }

        [HttpGet("{id}/status")]
        public ActionResult<FullMachineStatus> GetMachineStatus(int id)
        {
            // 1. Canlı durumu Polling Cache'ten al.
            if (!_pollingService.MachineDataCache.TryGetValue(id, out var status))
            {
                return NotFound();
            }

            // 2. MachineRepository'den makinenin statik detaylarını (MachineSubType) al.
            var machineDetails = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == id);


            // 3. FullMachineStatus objesine MachineSubType bilgisini ekle.
            // Bu bilgi, Dashboard'daki gruplama için kullanılır.
            if (machineDetails != null)
            {
                // Machine objesindeki MachineSubType bilgisini FullMachineStatus objesinin MakineTipi alanına ata.
                status.MakineTipi = machineDetails.MachineSubType;
            }

            return Ok(status);
        }

        // === YENİ METOTLAR ===
        [HttpPost]
        public IActionResult AddMachine([FromBody] Machine machine)
        {
            try
            {
                _machineRepository.AddMachine(machine);
                // NOT: Gerçek bir uygulamada yeni eklenen makine için polling servisini yeniden başlatmak gerekir.
                return CreatedAtAction(nameof(GetAllMachines), new { id = machine.Id }, machine);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMachine(int id, [FromBody] Machine machine)
        {
            if (id != machine.Id) return BadRequest();
            try
            {
                _machineRepository.UpdateMachine(machine);
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMachine(int id)
        {
            try
            {
                _machineRepository.DeleteMachine(id);
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}