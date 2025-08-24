// Repositories/ProcessLogRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using TekstilScada.Core; // Bu satırı ekleyin
using TekstilScada.Models;
namespace TekstilScada.Repositories
{
    public class ProcessLogRepository
    {
        private readonly string _connectionString = AppConfig.ConnectionString;

        public void LogData(FullMachineStatus status)
        {
            // Batch numarası yoksa veya reçete modunda değilse loglama yapma
            if (string.IsNullOrEmpty(status.BatchNumarasi) || !status.IsInRecipeMode)
            {
                return;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO process_data_log (MachineId, BatchId, LogTimestamp, LiveTemperature, LiveWaterLevel, LiveRpm) VALUES (@MachineId, @BatchId, @LogTimestamp, @LiveTemperature, @LiveWaterLevel, @LiveRpm);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", status.MachineId);
                cmd.Parameters.AddWithValue("@BatchId", status.BatchNumarasi);
                cmd.Parameters.AddWithValue("@LogTimestamp", DateTime.Now);
                cmd.Parameters.AddWithValue("@LiveTemperature", status.AnlikSicaklik); // Varsayımsal olarak anlık sıcaklık
                cmd.Parameters.AddWithValue("@LiveWaterLevel", status.AnlikSuSeviyesi);
                cmd.Parameters.AddWithValue("@LiveRpm", status.AnlikDevirRpm);
                cmd.ExecuteNonQuery();
            }
        }
        // YENİ: Manuel mod verilerini loglayan metot
        public void LogManualData(FullMachineStatus status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO manual_mode_log (MachineId, LogTimestamp, LiveTemperature,TotalWater, LiveWaterLevel, LiveRpm,LiveElectricity,LiveSteam) VALUES (@MachineId, @LogTimestamp, @LiveTemperature,@TotalWater, @LiveWaterLevel, @LiveRpm,@LiveElectricity,@LiveSteam);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", status.MachineId);
                cmd.Parameters.AddWithValue("@LogTimestamp", DateTime.Now);
                cmd.Parameters.AddWithValue("@LiveTemperature", status.AnlikSicaklik);
                cmd.Parameters.AddWithValue("@TotalWater", status.SuMiktari);
                

                cmd.Parameters.AddWithValue("@LiveWaterLevel", status.AnlikSuSeviyesi);
                cmd.Parameters.AddWithValue("@LiveRpm", status.AnlikDevirRpm);
                cmd.Parameters.AddWithValue("@LiveElectricity", status.ElektrikHarcama);
                cmd.Parameters.AddWithValue("@LiveSteam", status.BuharHarcama);
                cmd.ExecuteNonQuery();
            }
        }
        public List<ProcessDataPoint> GetLogsForBatch(int machineId, string batchId, DateTime? startTime = null, DateTime? endTime = null)
        {
            var dataPoints = new List<ProcessDataPoint>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var queryBuilder = new StringBuilder("SELECT LogTimestamp, LiveTemperature, LiveWaterLevel, LiveRpm FROM process_data_log WHERE MachineId = @MachineId ");

                if (!string.IsNullOrEmpty(batchId))
                {
                    queryBuilder.Append("AND BatchId = @BatchId ");
                }
                if (startTime.HasValue)
                {
                    queryBuilder.Append("AND LogTimestamp >= @StartTime ");
                }
                if (endTime.HasValue)
                {
                    queryBuilder.Append("AND LogTimestamp <= @EndTime ");
                }
                queryBuilder.Append("ORDER BY LogTimestamp;");

                var cmd = new MySqlCommand(queryBuilder.ToString(), connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);

                if (!string.IsNullOrEmpty(batchId)) cmd.Parameters.AddWithValue("@BatchId", batchId);
                if (startTime.HasValue) cmd.Parameters.AddWithValue("@StartTime", startTime.Value);
                if (endTime.HasValue) cmd.Parameters.AddWithValue("@EndTime", endTime.Value);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataPoints.Add(new ProcessDataPoint
                        {
                            Timestamp = reader.GetDateTime("LogTimestamp"),
                            Temperature = reader.GetDecimal("LiveTemperature"),
                            WaterLevel = reader.GetDecimal("LiveWaterLevel"),
                            Rpm = reader.GetInt32("LiveRpm")
                        });
                    }
                }
            }
            return dataPoints;
        }
        public List<ProcessDataPoint> GetLogsForDateRange(int machineId, DateTime startTime, DateTime endTime)
        {
            var dataPoints = new List<ProcessDataPoint>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT LogTimestamp, LiveTemperature, LiveWaterLevel, LiveRpm FROM process_data_log WHERE MachineId = @MachineId AND LogTimestamp BETWEEN @StartTime AND @EndTime ORDER BY LogTimestamp;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataPoints.Add(new ProcessDataPoint
                        {
                            Timestamp = reader.GetDateTime("LogTimestamp"),
                            Temperature = reader.GetDecimal("LiveTemperature"),
                            WaterLevel = reader.GetDecimal("LiveWaterLevel"),
                            Rpm = reader.GetInt32("LiveRpm")
                        });
                    }
                }
            }
            return dataPoints;
        }
        public List<ProcessDataManuel> GetManualLogs1(int machineId, DateTime startTime, DateTime endTime)
        {
            
            var dataPoints1 = new List<ProcessDataManuel>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT LogTimestamp, LiveTemperature,TotalWater, LiveWaterLevel, LiveRpm,LiveElectricity,LiveSteam FROM manual_mode_log WHERE MachineId = @MachineId AND LogTimestamp BETWEEN @StartTime AND @EndTime ORDER BY LogTimestamp;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);




                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataPoints1.Add(new ProcessDataManuel
                        {
                            Timestamp = reader.GetDateTime("LogTimestamp"),
                            totalwatermanuel = reader.GetDecimal("TotalWater"),
                            totalelectritymanuel = reader.GetDecimal("LiveElectricity"),
                            totalsteammanuel = reader.GetDecimal("LiveSteam")
                        });

                     
                    }
                }
            }
            return dataPoints1;
        }
        public List<ProcessDataPoint> GetManualLogs(int machineId, DateTime startTime, DateTime endTime)
        {
            var dataPoints = new List<ProcessDataPoint>();
          //  var dataPoints1 = new List<ProcessDataManuel>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT LogTimestamp, LiveTemperature,TotalWater, LiveWaterLevel, LiveRpm,LiveElectricity,LiveSteam FROM manual_mode_log WHERE MachineId = @MachineId AND LogTimestamp BETWEEN @StartTime AND @EndTime ORDER BY LogTimestamp;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);




                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       

                        dataPoints.Add(new ProcessDataPoint
                        {
                            Timestamp = reader.GetDateTime("LogTimestamp"),
                            Temperature = reader.GetDecimal("LiveTemperature"),
                            WaterLevel = reader.GetDecimal("LiveWaterLevel"),
                            Rpm = reader.GetInt32("LiveRpm")
                            
                        });
                    }
                }
            }
            return dataPoints;
        }
        public class ProcessDataManuel
        {
            public int MachineId { get; set; }
            public DateTime Timestamp { get; set; }
            public decimal totalwatermanuel { get; set; }
            public decimal totalelectritymanuel { get; set; }
            public decimal totalsteammanuel { get; set; }
        }
        // GÜNCELLENDİ: Manuel logları ve batch sonu verilerini birleştirerek özet oluşturan metot
        // GÜNCELLENDİ: Manuel logları ve batch sonu verilerini birleştirerek özet oluşturan metot
        public ManualConsumptionSummary GetManualConsumptionSummary(int machineId, string machineName, DateTime startTime, DateTime endTime)
        {
            var status = new FullMachineStatus();
            var dataPoints = GetManualLogs(machineId, startTime, endTime);
            if (!dataPoints.Any())
            {
                return null; // Veri yoksa boş döndür
            }
            var dataPoints1 = GetManualLogs1(machineId, startTime, endTime);
            if (!dataPoints1.Any())
            {
                return null; // Veri yoksa boş döndür
            }

            // Her bir değer türü için tepe noktalarını bulan ve toplayan yardımcı metot
            decimal SumPeaks(List<ProcessDataManuel> data, Func<ProcessDataManuel, decimal> selector)
            {
                decimal sumOfPeaks = 0;
                if (data.Count == 0) return 0;

                // Listenin sonuna her zaman son noktayı ekleyelim, çünkü o da bir tepe noktası olabilir.
                // Bunu yapmazsak, eğer artış devam ederken liste biterse, son tepe noktasını kaçırırız.
                var fullData = new List<ProcessDataManuel>(data);

                // Döngü içinde önceki değeri tutmak için bir değişken
                decimal previousValue = 0;

                foreach (var point in fullData)
                {
                    var currentValue = selector(point);
                    // Eğer mevcut değer bir önceki değerden büyükse (yükseliş trendindeysek)
                    if (currentValue > previousValue)
                    {
                        // Değerin bir sonraki noktada düşüp düşmediğini kontrol et
                        // Bu, o noktanın bir "tepe" olduğunu gösterir.
                        // Basitlik için sadece currentValue'i previousValue'a atıyoruz ve döngüyü devam ettiriyoruz.
                        // Çünkü asıl tepe noktası, düşüşe geçtiği anda tespit edilecek.
                        // Bu algoritmada, her düşüşün başlangıcındaki en yüksek değeri yakalamak hedeflenmiştir.

                        // Daha basit bir algoritma kullanalım:
                        // Sadece yükselen trendlerin son noktasını alalım.
                    }
                    else if (currentValue < previousValue)
                    {
                        // Yükseliş bitti, previousValue bir tepe noktasıydı.
                        sumOfPeaks += previousValue;
                        // Yeni bir trend başladı.
                    }
                    previousValue = currentValue;
                }

                // Döngü bittiğinde, son kalan tepe noktasını da ekleyelim.
                sumOfPeaks += previousValue;

                return sumOfPeaks;
            }


            var summary = new ManualConsumptionSummary
            {
                Makine = machineName,
                RaporAraligi = $"{startTime:dd.MM.yy HH:mm} - {endTime:dd.MM.yy HH:mm}",
                ToplamManuelSure = TimeSpan.FromSeconds(dataPoints.Count * 5).ToString(@"hh\:mm\:ss"),
                OrtalamaSicaklik = dataPoints.Average(p => (double)p.Temperature / 10.0),
                OrtalamaDevir = dataPoints.Average(p => p.Rpm),

                // Tepe noktalarını bulup toplama işlemleri
                ToplamSuTuketimi_Litre = SumPeaks(dataPoints1, p => p.totalwatermanuel),
                ToplamElektrikTuketimi_kW = SumPeaks(dataPoints1, p => p.totalelectritymanuel),
                ToplamBuharTuketimi_kg = SumPeaks(dataPoints1, p => p.totalsteammanuel)
            };

            return summary;
        }
        public List<ProcessDataPoint> GetLogsForDateRange(DateTime startTime, DateTime endTime, List<int> machineIds)
        {
            var dataPoints = new List<ProcessDataPoint>();
            // Eğer seçili makine yoksa, boş liste döndür.
            if (machineIds == null || !machineIds.Any())
            {
                return dataPoints;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var queryBuilder = new StringBuilder();
                queryBuilder.Append("SELECT MachineId, LogTimestamp, LiveTemperature, LiveWaterLevel, LiveRpm FROM process_data_log ");
                queryBuilder.Append("WHERE LogTimestamp BETWEEN @StartTime AND @EndTime ");

                // Seçilen makine ID'lerini sorguya parametre olarak ekle
                queryBuilder.Append("AND MachineId IN (");
                var machineParams = new List<string>();
                for (int i = 0; i < machineIds.Count; i++)
                {
                    var paramName = $"@MachineId{i}";
                    machineParams.Add(paramName);
                }
                queryBuilder.Append(string.Join(",", machineParams));
                queryBuilder.Append(") ORDER BY LogTimestamp;");

                var cmd = new MySqlCommand(queryBuilder.ToString(), connection);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                for (int i = 0; i < machineIds.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@MachineId{i}", machineIds[i]);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataPoints.Add(new ProcessDataPoint
                        {
                            // MachineId'yi de modele eklemek ileride faydalı olabilir,
                            // şimdilik bu şekilde bırakıyoruz.
                            MachineId = reader.GetInt32("MachineId"),
                            Timestamp = reader.GetDateTime("LogTimestamp"),
                            Temperature = reader.GetDecimal("LiveTemperature"),
                            WaterLevel = reader.GetDecimal("LiveWaterLevel"),
                            Rpm = reader.GetInt32("LiveRpm")
                        });
                    }
                }
            }
            return dataPoints;
        }
    }

    
    // Grafik için veri noktası modeli
    public class ProcessDataPoint
    {
        public int MachineId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Temperature { get; set; }
        public decimal WaterLevel { get; set; }
        public int Rpm { get; set; }
    }
}
