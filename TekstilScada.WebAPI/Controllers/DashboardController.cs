// TekstilScada.WebAPI/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization; // YENİ: DateTime.Parse için eklendi
using System.Data; // ZORUNLU: DataTable için
using System.Linq; // ZORUNLU: AsEnumerable() ve Select() için
using TekstilScada.Models;
using TekstilScada.Repositories;
using TekstilScada.WebAPI.Controllers; // YENİ: ReportFiltersDto için eklendi (Namespace'ler farklıysa gereklidir)
using static System.Convert;
public class HourlyConsumptionData
{
    public double Saat { get; set; }
    public double ToplamElektrik { get; set; }
    public double ToplamSu { get; set; }
    public double ToplamBuhar { get; set; }
}

public class HourlyOeeData
{
    public double Saat { get; set; }
    public double AverageOEE { get; set; }
}
namespace TekstilScada.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardRepository _dashboardRepository;
        private readonly AlarmRepository _alarmRepository; // YENİ: AlarmRepo eklendi

        public DashboardController(DashboardRepository dashboardRepository, AlarmRepository alarmRepository) // YENİ: AlarmRepo enjekte edildi
        {
            _dashboardRepository = dashboardRepository;
            _alarmRepository = alarmRepository; // YENİ: Atama yapıldı
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
        [HttpGet("hourly-consumption")]
        public ActionResult<IEnumerable<HourlyConsumptionData>> GetHourlyConsumption()
        {
            try
            {
                var hourlyData = _dashboardRepository.GetHourlyFactoryConsumption(DateTime.Today);

                var result = hourlyData.AsEnumerable().Select(row => new HourlyConsumptionData
                {
                    Saat = ToDouble(row.Field<object>("Saat") ?? 0),
                    ToplamElektrik = ToDouble(row.Field<object>("ToplamElektrik") ?? 0),
                    ToplamSu = ToDouble(row.Field<object>("ToplamSu") ?? 0),
                    ToplamBuhar = ToDouble(row.Field<object>("ToplamBuhar") ?? 0)
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Hatanın loglanması için 500 hatası döndürülüyor.
                return StatusCode(500, $"Saatlik tüketim verileri alınırken bir hata oluştu: {ex.Message}");
            }
        }

        // YENİ METOT: Saatlik Ortalama OEE Verilerini Getirir
        [HttpGet("hourly-oee")]
        public ActionResult<IEnumerable<HourlyOeeData>> GetHourlyAverageOee()
        {
            try
            {
                var hourlyData = _dashboardRepository.GetHourlyAverageOee(DateTime.Today);

                var result = hourlyData.AsEnumerable().Select(row => new HourlyOeeData
                {
                    // KRİTİK DÜZELTME: Güvenli dönüşüm için Convert.ToDouble kullanıldı.
                    Saat = ToDouble(row.Field<object>("Saat") ?? 0),
                    AverageOEE = ToDouble(row.Field<object>("AverageOEE") ?? 0)
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Saatlik OEE verileri alınırken bir hata oluştu: {ex.Message}");
            }
        }

        // YENİ METOT: En Sık Görülen Alarmları Getirir
        [HttpGet("top-alarms")]
        public ActionResult<IEnumerable<TopAlarmData>> GetTopAlarms()
        {
            try
            {
                // Windows Forms'taki gibi son 24 saatlik veri
                var topAlarms = _alarmRepository.GetTopAlarmsByFrequency(DateTime.Now.AddDays(-1), DateTime.Now);
                return Ok(topAlarms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Popüler alarmlar alınırken bir hata oluştu: {ex.Message}");
            }
        }
    }
}