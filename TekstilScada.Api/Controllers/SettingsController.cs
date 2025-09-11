using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TekstilScada.Models;
using TekstilScada.Repositories;
using Microsoft.AspNetCore.Authorization; // Bu satırı ekle
namespace TekstilScada.Api.Controllers
{
   // [Authorize] // Bu etiketi ekle
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly MachineRepository _machineRepository;
        private readonly AlarmRepository _alarmRepository;
        private readonly CostRepository _costRepository;
        private readonly PlcOperatorRepository _plcOperatorRepository;

        public SettingsController(
            MachineRepository machineRepository,
            AlarmRepository alarmRepository,
            CostRepository costRepository,
            PlcOperatorRepository plcOperatorRepository)
        {
            _machineRepository = machineRepository;
            _alarmRepository = alarmRepository;
            _costRepository = costRepository;
            _plcOperatorRepository = plcOperatorRepository;
        }

        // --- Makine Ayarları Uç Noktaları ---
        [HttpGet("machines")]
        public IActionResult GetMachines()
        {
            var machines = _machineRepository.GetAllMachines();
            return Ok(machines);
        }

        [HttpPost("machines")]
        public IActionResult SaveMachine([FromBody] Machine machine)
        {
            if (machine.Id > 0)
            {
                _machineRepository.UpdateMachine(machine);
            }
            else
            {
                _machineRepository.AddMachine(machine);
            }
            return Ok(machine);
        }

        [HttpDelete("machines/{id}")]
        public IActionResult DeleteMachine(int id)
        {
            _machineRepository.DeleteMachine(id);
            return NoContent();
        }

        // --- Alarm Tanımları Uç Noktaları ---
        [HttpGet("alarms")]
        public IActionResult GetAlarmDefinitions()
        {
            var alarms = _alarmRepository.GetAllAlarmDefinitions();
            return Ok(alarms);
        }

        [HttpPost("alarms")]
        public IActionResult SaveAlarmDefinition([FromBody] AlarmDefinition alarm)
        {
            if (alarm.Id > 0)
            {
                _alarmRepository.UpdateAlarmDefinition(alarm);
            }
            else
            {
                _alarmRepository.AddAlarmDefinition(alarm);
            }
            return Ok(alarm);
        }

        [HttpDelete("alarms/{id}")]
        public IActionResult DeleteAlarmDefinition(int id)
        {
            _alarmRepository.DeleteAlarmDefinition(id);
            return NoContent();
        }

        // --- Maliyet Parametreleri Uç Noktaları ---
        [HttpGet("costs")]
        public IActionResult GetCostParameters()
        {
            var costs = _costRepository.GetAllParameters();
            return Ok(costs);
        }

        [HttpPost("costs")]
        public IActionResult UpdateCostParameters([FromBody] List<CostParameter> costs)
        {
            _costRepository.UpdateParameters(costs);
            return Ok();
        }

        // Not: Mevcut CostRepository sınıfında bir "Delete" metodu bulunmamaktadır.
        // Bu nedenle maliyet parametrelerini Web API üzerinden silemezsiniz.

        // --- PLC Operatör Ayarları Uç Noktaları ---
        [HttpGet("plc-operators")]
        public IActionResult GetPlcOperators()
        {
            var operators = _plcOperatorRepository.GetAll();
            return Ok(operators);
        }

        [HttpPost("plc-operators")]
        public IActionResult SavePlcOperator([FromBody] PlcOperator op)
        {
            _plcOperatorRepository.SaveOrUpdate(op);
            return Ok(op);
        }

        [HttpDelete("plc-operators/{id}")]
        public IActionResult DeletePlcOperator(int id)
        {
            _plcOperatorRepository.Delete(id);
            return NoContent();
        }
    }
}