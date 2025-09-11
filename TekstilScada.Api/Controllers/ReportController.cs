using Microsoft.AspNetCore.Mvc;
using TekstilScada.Repositories;
using static TekstilScada.Repositories.ProductionRepository;
using TekstilScada.Core.Models;
using System.Data; // DataTable için
using TekstilScada.Models;
using Microsoft.AspNetCore.Authorization; // Bu satırı ekle
namespace TekstilScada.Api.Controllers
{
    //[Authorize] // Bu etiketi ekle
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ProductionRepository _productionRepository;
        private readonly AlarmRepository _alarmRepository;
        private readonly DashboardRepository _dashboardRepository;
        private readonly ProcessLogRepository _processLogRepository;
        private readonly UserRepository _userRepository;
        private readonly MachineRepository _machineRepository; // EKLENDİ

        public ReportController(ProductionRepository productionRepository, AlarmRepository alarmRepository, DashboardRepository dashboardRepository, ProcessLogRepository processLogRepository, UserRepository userRepository, MachineRepository machineRepository) // DÜZELTME
        {
            _productionRepository = productionRepository;
            _alarmRepository = alarmRepository;
            _dashboardRepository = dashboardRepository;
            _processLogRepository = processLogRepository;
            _userRepository = userRepository;
            _machineRepository = machineRepository; // EKLENDİ
        }

        // Production Report
        [HttpGet("production-report")]
        public IActionResult GetProductionReport(
            [FromQuery] DateTime? startTime,
            [FromQuery] DateTime? endTime,
            [FromQuery] int? machineId,
            [FromQuery] string? batchNo,
            [FromQuery] string? recipeName,
            [FromQuery] string? siparisNo,
            [FromQuery] string? musteriNo,
            [FromQuery] string? operatorName)
        {
            var filters = new ReportFilters
            {
                StartTime = startTime ?? DateTime.Today.AddDays(-7),
                EndTime = endTime ?? DateTime.Now,
                MachineId = machineId,
                BatchNo = batchNo,
                RecipeName = recipeName,
                SiparisNo = siparisNo,
                MusteriNo = musteriNo,
                OperatorName = operatorName
            };

            var reportData = _productionRepository.GetProductionReport(filters);
            return Ok(reportData);
        }

        // Alarm Report
        [HttpGet("alarm-report")]
        public IActionResult GetAlarmReport([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] int? machineId)
        {
            if (!startTime.HasValue)
            {
                startTime = DateTime.Today.AddDays(-7);
            }
            if (!endTime.HasValue)
            {
                endTime = DateTime.Now;
            }

            var reportData = _alarmRepository.GetAlarmReport(startTime.Value, endTime.Value, machineId);
            return Ok(reportData);
        }

        // Genel Üretim Raporu
        [HttpGet("general-production-report")]
        public IActionResult GetGeneralProductionReport([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] List<string> machineNames)
        {
            var reportData = _productionRepository.GetGeneralProductionReport(startTime, endTime, machineNames);
            return Ok(reportData);
        }

        // OEE Report
        [HttpGet("oee-report")]
        public IActionResult GetOeeReport([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] int? machineId)
        {
            var reportData = _dashboardRepository.GetOeeReport(startTime, endTime, machineId);
            return Ok(reportData);
        }

        // Manuel Kullanım Raporu
        [HttpGet("manual-usage-report")]
        public IActionResult GetManualUsageReport([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] int machineId)
        {
            var machine = _machineRepository.GetAllMachines().FirstOrDefault(m => m.Id == machineId);
            if (machine == null)
            {
                return NotFound($"Machine with ID {machineId} not found.");
            }

            var reportData = _processLogRepository.GetManualConsumptionSummary(machine.Id, machine.MachineName, startTime, endTime);
            return Ok(reportData);
        }

        // Trend Analizi Raporu
        [HttpGet("trend-analysis-data")]
        public IActionResult GetTrendAnalysisData([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] List<int> machineIds)
        {
            if (machineIds == null || !machineIds.Any())
            {
                return BadRequest("En az bir makine ID'si belirtmelisiniz.");
            }

            var dataPoints = _processLogRepository.GetLogsForDateRange(startTime, endTime, machineIds);
            return Ok(dataPoints);
        }

        // Eylem Kayıtları Raporu
        [HttpGet("action-log-report")]
        public IActionResult GetActionLogReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? username, [FromQuery] string? details)
        {
            var logs = _userRepository.GetActionLogs(startDate, endDate, username, details);
            return Ok(logs);
        }
    }
}