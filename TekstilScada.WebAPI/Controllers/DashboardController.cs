// TekstilScada.WebAPI/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using TekstilScada.Repositories;
using TekstilScada.Models;
using System;
using System.Collections.Generic;
using System.Globalization; // YENİ: DateTime.Parse için eklendi
using TekstilScada.WebAPI.Controllers; // YENİ: ReportFiltersDto için eklendi (Namespace'ler farklıysa gereklidir)

namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        // DÜZELTME: HTTP GET yerine HTTP POST kullanılıyor ve filtreler gövdeden alınıyor.
        [HttpPost("oee-report")]
        public ActionResult<IEnumerable<OeeData>> GetOeeReport([FromBody] ReportFiltersDto filtersDto)
        {
            // Null kontrolü, 400 hatasını önlemek için WebApp'ten gelen verinin kontrolünü sağlar.
            if (filtersDto.StartTime == null || filtersDto.EndTime == null)
            {
                return BadRequest("Başlangıç ve Bitiş tarihleri zorunludur.");
            }

            try
            {
                var startTime = DateTime.Parse(filtersDto.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                var endTime = DateTime.Parse(filtersDto.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                // EndTime'ı repoda kullanacağımız '<' operatörüne hazırlıyoruz.
                var effectiveEndTime = endTime.Date.AddDays(1);

                var reportData = _dashboardRepository.GetOeeReport(startTime.Date, effectiveEndTime, filtersDto.MachineId);
                return Ok(reportData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"OEE Raporu oluşturulurken bir hata oluştu: {ex.Message}");
            }
        }
    }
}