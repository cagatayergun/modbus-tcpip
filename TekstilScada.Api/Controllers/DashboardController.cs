using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
//using TekstilScada.Core.Repositories;
using TekstilScada.Core.Services;
using TekstilScada.Repositories;
using TekstilScada.Services;
using TekstilScada.Web.Models;

namespace TekstilScada.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;
        private readonly MachineRepository _machineRepository;
        private readonly AlarmRepository _alarmRepository;
        private readonly PlcPollingService _pollingService;

        public DashboardController(DashboardRepository dashboardRepository, MachineRepository machineRepository, AlarmRepository alarmRepository, PlcPollingService pollingService)
        {
            _dashboardRepository = dashboardRepository;
            _machineRepository = machineRepository;
            _alarmRepository = alarmRepository;
            _pollingService = pollingService;
        }

        [HttpGet("GetKpiData")]
        public async Task<ActionResult<KpiViewModel>> GetKpiData()
        {
            // Verileri PlcPollingService'in bellek içi önbelleğinden çekiyoruz.
            var allStatuses = _pollingService.MachineDataCache.Values;

            int totalMachines = allStatuses.Count;
            int runningMachines = allStatuses.Count(s => s.IsInRecipeMode && !s.IsPaused && !s.HasActiveAlarm);
            int alarmedMachines = allStatuses.Count(s => s.HasActiveAlarm);
            int stoppedMachines = totalMachines - runningMachines - alarmedMachines;

            var kpiData = new KpiViewModel
            {
                TotalMachineCount = totalMachines,
                RunningMachineCount = runningMachines,
                StoppedMachineCount = stoppedMachines,
                AlarmedMachineCount = alarmedMachines
            };

            return Ok(kpiData);
        }
    }
}
