// File: TekstilScada.Core/Core/ExcelExportHelper.cs

using ClosedXML.Excel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TekstilScada.Core.Models;
using TekstilScada.Models;

namespace TekstilScada.Core.Core
{
    public static class ExcelExportHelper
    {
        public static byte[] ExportProductionReportToExcel(List<ProductionReportItem> reportItems)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Üretim Raporu");
                int currentRow = 1;

                // Başlıkları yaz
                worksheet.Cell(currentRow, 1).Value = "Makine Adı";
                worksheet.Cell(currentRow, 2).Value = "Reçete Adı";
                worksheet.Cell(currentRow, 3).Value = "Batch No";
                worksheet.Cell(currentRow, 4).Value = "Operatör";
                worksheet.Cell(currentRow, 5).Value = "Başlangıç";
                worksheet.Cell(currentRow, 6).Value = "Bitiş";
                worksheet.Cell(currentRow, 7).Value = "Süre";
                worksheet.Cell(currentRow, 8).Value = "Üretilen Miktar";
                worksheet.Cell(currentRow, 9).Value = "Toplam Su (L)";
                worksheet.Cell(currentRow, 10).Value = "Toplam Elektrik (kW)";

                // Başlıkları kalın yap
                worksheet.Range(1, 1, 1, 10).Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightGray);

                // Verileri yaz
                foreach (var item in reportItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.MachineName;
                    worksheet.Cell(currentRow, 2).Value = item.RecipeName;
                    worksheet.Cell(currentRow, 3).Value = item.BatchId;
                    worksheet.Cell(currentRow, 4).Value = item.OperatorName;
                    worksheet.Cell(currentRow, 5).Value = item.StartTime;
                    worksheet.Cell(currentRow, 6).Value = item.EndTime;
                    worksheet.Cell(currentRow, 7).Value = item.CycleTime;
                    worksheet.Cell(currentRow, 8).Value = item.TotalSteam;
                    worksheet.Cell(currentRow, 9).Value = item.TotalWater;
                    worksheet.Cell(currentRow, 10).Value = item.TotalElectricity;
                }

                // Sütun genişliklerini ayarla
                worksheet.Columns().AdjustToContents();

                // Tarih formatını ayarla
                worksheet.Column(5).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm");
                worksheet.Column(6).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm");

                // Sayı formatlarını ayarla
                worksheet.Column(8).Style.NumberFormat.SetFormat("#,##0");
                worksheet.Column(9).Style.NumberFormat.SetFormat("#,##0");
                worksheet.Column(10).Style.NumberFormat.SetFormat("#,##0.00");


                // Bellek akışına yaz ve byte dizisi olarak döndür
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
            public static byte[] ExportAlarmReportToExcel(List<AlarmReportItem> reportItems)
        {
            if (reportItems == null || !reportItems.Any())
            {
                return new byte[0];
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Alarm Raporu");
                int currentRow = 1;

                // Başlıklar
                worksheet.Cell(currentRow, 1).Value = "Makine Adı";
                worksheet.Cell(currentRow, 2).Value = "Alarm No";
                worksheet.Cell(currentRow, 3).Value = "Alarm Açıklaması";
                worksheet.Cell(currentRow, 4).Value = "Başlangıç Zamanı";
                worksheet.Cell(currentRow, 5).Value = "Bitiş Zamanı";
                worksheet.Cell(currentRow, 6).Value = "Süre";

                // Biçimlendir
                worksheet.Range(1, 1, 1, 6).Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightCoral);

                // Verileri yaz
                foreach (var item in reportItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.MachineName;
                    worksheet.Cell(currentRow, 2).Value = item.AlarmNumber;
                    worksheet.Cell(currentRow, 3).Value = item.AlarmText;
                    worksheet.Cell(currentRow, 4).Value = item.StartTime;
                    worksheet.Cell(currentRow, 5).Value = item.EndTime;
                    worksheet.Cell(currentRow, 6).Value = item.Duration;

                    // Aktif alarmları vurgula
                    if (item.Duration == "Aktif")
                    {
                        worksheet.Row(currentRow).Style.Fill.SetBackgroundColor(XLColor.Red);
                    }
                }

                worksheet.Columns().AdjustToContents();
                // Zaman formatlarını ayarla
                worksheet.Column(4).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm:ss");
                worksheet.Column(5).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm:ss");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public static byte[] ExportOeeReportToExcel(List<OeeData> reportItems)
        {
            if (reportItems == null || !reportItems.Any())
            {
                return new byte[0];
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("OEE Raporu");
                int currentRow = 1;

                // Başlıklar
                worksheet.Cell(currentRow, 1).Value = "Makine Adı";
                worksheet.Cell(currentRow, 2).Value = "Batch No";
                worksheet.Cell(currentRow, 3).Value = "Kullanılabilirlik (A)";
                worksheet.Cell(currentRow, 4).Value = "Performans (P)";
                worksheet.Cell(currentRow, 5).Value = "Kalite (Q)";
                worksheet.Cell(currentRow, 6).Value = "OEE (A x P x Q)";
                worksheet.Cell(currentRow, 7).Value = "Başlangıç Zamanı";
                worksheet.Cell(currentRow, 8).Value = "Bitiş Zamanı";

                // Biçimlendir
                worksheet.Range(1, 1, 1, 8).Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightYellow);

                // Verileri yaz
                foreach (var item in reportItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.MachineName;
                    worksheet.Cell(currentRow, 2).Value = item.BatchId;
                    worksheet.Cell(currentRow, 3).Value = item.Availability;
                    worksheet.Cell(currentRow, 4).Value = item.Performance;
                    worksheet.Cell(currentRow, 5).Value = item.Quality;
                    worksheet.Cell(currentRow, 6).Value = item.OEE;
                 

                    // OEE değerine göre satır rengi
                    if (item.OEE < 60)
                    {
                        worksheet.Row(currentRow).Style.Fill.SetBackgroundColor(XLColor.Red);
                    }
                    else if (item.OEE < 85)
                    {
                        worksheet.Row(currentRow).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                    }
                    else
                    {
                        worksheet.Row(currentRow).Style.Fill.SetBackgroundColor(XLColor.LightGreen);
                    }
                }

                worksheet.Columns().AdjustToContents();
                // Yüzdelik değerler için format
                worksheet.Column(3).Style.NumberFormat.SetFormat("0.00%");
                worksheet.Column(4).Style.NumberFormat.SetFormat("0.00%");
                worksheet.Column(5).Style.NumberFormat.SetFormat("0.00%");
                worksheet.Column(6).Style.NumberFormat.SetFormat("0.00%");

                // Zaman formatlarını ayarla
                worksheet.Column(7).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm");
                worksheet.Column(8).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public static byte[] ExportManualConsumptionReportToExcel(ManualConsumptionSummary summary)
        {
            // Bu rapor bir nesne bekler.
            if (summary == null)
            {
                return new byte[0];
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Manuel Tüketim Özeti");

                // Başlık
                worksheet.Cell("A1").Value = $"Manuel Tüketim Özeti - {summary.Makine}";
                worksheet.Range("A1:B1").Merge().Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightSkyBlue);

                // Özet Detayları
                worksheet.Cell("A3").Value = "Rapor Aralığı:";
                worksheet.Cell("B3").Value = summary.RaporAraligi;

                worksheet.Cell("A4").Value = "Toplam Manuel Çalışma Süresi:";
                worksheet.Cell("B4").Value = summary.ToplamManuelSure;

                worksheet.Cell("A5").Value = "Ortalama Sıcaklık (°C):";
                worksheet.Cell("B5").Value = summary.OrtalamaSicaklik;
                worksheet.Cell("B5").Style.NumberFormat.SetFormat("0.0");

                worksheet.Cell("A6").Value = "Ortalama Devir (Rpm):";
                worksheet.Cell("B6").Value = summary.OrtalamaDevir;
                worksheet.Cell("B6").Style.NumberFormat.SetFormat("0");

                // Tüketim Başlıkları
                worksheet.Cell("A8").Value = "Tüketim Değerleri";
                worksheet.Range("A8:B8").Merge().Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightGreen);

                worksheet.Cell("A9").Value = "Toplam Su Tüketimi (Litre):";
                worksheet.Cell("B9").Value = summary.ToplamSuTuketimi_Litre;
                worksheet.Cell("B9").Style.NumberFormat.SetFormat("#,##0");

                worksheet.Cell("A10").Value = "Toplam Elektrik Tüketimi (kW):";
                worksheet.Cell("B10").Value = summary.ToplamElektrikTuketimi_kW;
                worksheet.Cell("B10").Style.NumberFormat.SetFormat("#,##0.00");

                worksheet.Cell("A11").Value = "Toplam Buhar Tüketimi (kg):";
                worksheet.Cell("B11").Value = summary.ToplamBuharTuketimi_kg;
                worksheet.Cell("B11").Style.NumberFormat.SetFormat("#,##0.00");

                // Sütun formatları
                worksheet.Column("A").Style.Font.SetBold(true);
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public static byte[] ExportGeneralDetailedConsumptionReportToExcel(List<ProductionReportItem> reportItems, string selectedConsumptionType)
        {
            if (reportItems == null || !reportItems.Any())
            {
                return new byte[0];
            }

            // Yardımcı fonksiyon: Seçilen tüketim tipinin adını ve birimini döndürür
            string GetConsumptionUnitName(string type)
            {
                return type switch
                {
                    "TotalWater" => "Su (Litre)",
                    "TotalElectricity" => "Elektrik (kW)",
                    "TotalSteam" => "Buhar (kg)",
                    _ => "Tüketim Değeri"
                };
            }

            // Excel'de gösterilecek sütun başlığı
            string consumptionColumnName = GetConsumptionUnitName(selectedConsumptionType);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Detaylı Tüketim");
                int currentRow = 1;

                // Başlıklar
                worksheet.Cell(currentRow, 1).Value = "Makine Adı";
                worksheet.Cell(currentRow, 2).Value = "Batch No";
                worksheet.Cell(currentRow, 3).Value = "Bitiş Zamanı";
                worksheet.Cell(currentRow, 4).Value = consumptionColumnName;

                // Biçimlendir
                worksheet.Range(1, 1, 1, 4).Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightCyan);

                // Verileri yaz
                foreach (var item in reportItems)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.MachineName;
                    worksheet.Cell(currentRow, 2).Value = item.BatchId;
                    worksheet.Cell(currentRow, 3).Value = item.EndTime;

                    // Seçilen tüketim tipine göre değeri al
                    decimal consumptionValue = selectedConsumptionType switch
                    {
                        "TotalWater" => item.TotalWater,
                        "TotalElectricity" => item.TotalElectricity,
                        "TotalSteam" => item.TotalSteam,
                        _ => 0
                    };

                    worksheet.Cell(currentRow, 4).Value = consumptionValue;

                    // Sayı formatlarını ayarla
                    worksheet.Column(4).Style.NumberFormat.SetFormat("#,##0.00");
                }

                worksheet.Columns().AdjustToContents();
                // Zaman formatını ayarla
                worksheet.Column(3).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public static byte[] ExportActionLogsReportToExcel(List<ActionLogEntry> logs)
        {
            if (logs == null || !logs.Any())
            {
                return new byte[0];
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Eylem Kayıtları");
                int currentRow = 1;

                // Başlıklar
                worksheet.Cell(currentRow, 1).Value = "Zaman Damgası";
                worksheet.Cell(currentRow, 2).Value = "Kullanıcı Adı";
                worksheet.Cell(currentRow, 3).Value = "Eylem Tipi";
                worksheet.Cell(currentRow, 4).Value = "Detaylar";

                // Biçimlendir
                worksheet.Range(1, 1, 1, 4).Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightCyan);

                // Verileri yaz
                foreach (var log in logs)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = log.Timestamp;
                    worksheet.Cell(currentRow, 2).Value = log.Username;
                    worksheet.Cell(currentRow, 3).Value = log.ActionType;
                    worksheet.Cell(currentRow, 4).Value = log.Details;
                }

                worksheet.Columns().AdjustToContents();
                // Zaman formatını ayarla
                worksheet.Column(1).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm:ss");

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        public class TrendDataPoint
        {
            public DateTime Timestamp { get; set; }
            public double Temperature { get; set; }
            public double Rpm { get; set; }
            public double WaterLevel { get; set; }
        }

        public class ProductionStepDetailDto : ProductionStepDetail
        {
            public double TheoreticalDurationSeconds { get; set; } = 0;
            public double Temperature { get; set; } = 0;
            public string StepDescription => StepName;
        }

        public class AlarmDetailDto
        {
            public DateTime AlarmTime { get; set; } = DateTime.MinValue;
            public string AlarmType { get; set; } = string.Empty;
            public string AlarmDescription { get; set; } = string.Empty;
            public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        }

        public class ProductionDetailDto
        {
            public ProductionReportItem Header { get; set; } = new();
            public List<ProductionStepDetailDto> Steps { get; set; } = new();
            public List<AlarmDetailDto> Alarms { get; set; } = new();
            public List<TrendDataPoint> LogData { get; set; } = new();
            public List<TrendDataPoint> TheoreticalData { get; set; } = new();
        }
        public static byte[] ExportProductionDetailToExcel(ProductionDetailDto detailData)
        {
            if (detailData == null || detailData.Header == null)
            {
                return new byte[0];
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Üretim Detayı");
                int currentRow = 1;

                // --- BÖLÜM 1: GENEL BİLGİLER (Header) ---
                worksheet.Cell(currentRow, 1).Value = "Genel Bilgiler";
                worksheet.Range(currentRow, 1, currentRow, 2).Merge().Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightBlue);
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "Makine Adı:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.MachineName; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Batch ID:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.BatchId; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Reçete Adı:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.RecipeName; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Operatör:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.OperatorName; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Başlangıç Zamanı:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.StartTime; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Bitiş Zamanı:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.EndTime; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Toplam Süre:"; worksheet.Cell(currentRow, 2).Value = detailData.Header.CycleTime; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Makine Alarm Süresi (sn):"; worksheet.Cell(currentRow, 2).Value = detailData.Header.MachineAlarmDurationSeconds; currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Operatör Duraklama Süresi (sn):"; worksheet.Cell(currentRow, 2).Value = detailData.Header.OperatorPauseDurationSeconds; currentRow++;

                worksheet.Range("A1:A9").Style.Font.SetBold(true);

                currentRow += 2;

                // --- BÖLÜM 2: ADIM DETAYLARI (Steps) ---
                worksheet.Cell(currentRow, 1).Value = "Adım Detayları";
                worksheet.Range(currentRow, 1, currentRow, 5).Merge().Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightGreen);
                currentRow++;

                // Başlıklar
                worksheet.Cell(currentRow, 1).Value = "Adım No";
                worksheet.Cell(currentRow, 2).Value = "Açıklama";
                worksheet.Cell(currentRow, 3).Value = "Teorik Süre";
                worksheet.Cell(currentRow, 4).Value = "Gerçekleşen Süre";
                worksheet.Cell(currentRow, 5).Value = "Sıcaklık (°C)";
                worksheet.Row(currentRow).Style.Font.SetBold(true);
                currentRow++;

                if (detailData.Steps != null)
                {
                    foreach (var step in detailData.Steps)
                    {
                        worksheet.Cell(currentRow, 1).Value = step.StepNumber;
                        worksheet.Cell(currentRow, 2).Value = step.StepDescription;
                        worksheet.Cell(currentRow, 3).Value = TimeSpan.FromSeconds(step.TheoreticalDurationSeconds).ToString(@"hh\:mm\:ss");
                        worksheet.Cell(currentRow, 4).Value = step.WorkingTime;
                        worksheet.Cell(currentRow, 5).Value = step.Temperature;
                        currentRow++;
                    }
                }

                currentRow += 2;

                // --- BÖLÜM 3: ALARM DETAYLARI (Alarms) ---
                worksheet.Cell(currentRow, 1).Value = "Alarm Detayları";
                worksheet.Range(currentRow, 1, currentRow, 4).Merge().Style.Font.SetBold(true).Fill.SetBackgroundColor(XLColor.LightCoral);
                currentRow++;

                // Başlıklar
                worksheet.Cell(currentRow, 1).Value = "Tarih";
                worksheet.Cell(currentRow, 2).Value = "Tip";
                worksheet.Cell(currentRow, 3).Value = "Açıklama";
                worksheet.Cell(currentRow, 4).Value = "Süre";
                worksheet.Row(currentRow).Style.Font.SetBold(true);
                currentRow++;

                if (detailData.Alarms != null)
                {
                    foreach (var alarm in detailData.Alarms)
                    {
                        worksheet.Cell(currentRow, 1).Value = alarm.AlarmTime;
                        worksheet.Cell(currentRow, 2).Value = alarm.AlarmType;
                        worksheet.Cell(currentRow, 3).Value = alarm.AlarmDescription;
                        worksheet.Cell(currentRow, 4).Value = alarm.Duration.ToString(@"hh\:mm\:ss");
                        currentRow++;
                    }
                }

                worksheet.Columns().AdjustToContents();

                // Zaman formatlarını ayarla
                worksheet.Column(5).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm:ss");
                worksheet.Column(1).Style.DateFormat.SetFormat("dd.MM.yyyy HH:mm:ss"); // Alarm Tarihi için

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
        // Diğer raporlar (Alarm, OEE vb.) için de benzer metotlar eklenebilir.
    }
}