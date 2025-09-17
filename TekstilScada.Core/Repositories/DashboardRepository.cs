using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TekstilScada.Models;
using TekstilScada.Core; // Bu satırı ekleyin
namespace TekstilScada.Repositories
{
    public class DashboardRepository
    {
        private readonly string _connectionString = AppConfig.ConnectionString;
        private readonly RecipeRepository _recipeRepository; // YENİ: Bağımlılık eklendi

        public DashboardRepository(RecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }
        public List<OeeData> GetOeeReport(DateTime startTime, DateTime endTime, int? machineId)
        {
            var oeeList = new List<OeeData>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
    SELECT 
        m.MachineName,
        b.BatchId,
        b.TotalProductionCount,
        b.DefectiveProductionCount,
        b.TotalDownTimeSeconds,
        b.RecipeName,
        b.actual_produced_quantity,
        TIME_TO_SEC(TIMEDIFF(b.EndTime, b.StartTime)) as PlannedTimeInSeconds
    FROM production_batches AS b
    JOIN machines AS m ON b.MachineId = m.Id
    WHERE 
        b.StartTime BETWEEN @StartTime AND @EndTime 
        AND b.EndTime IS NOT NULL 
        AND b.TotalProductionCount > 0 " +
                    (machineId.HasValue ? "AND b.MachineId = @MachineId " : "") +
                    "ORDER BY m.MachineName;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                if (machineId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@MachineId", machineId.Value);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        double plannedTime = reader.IsDBNull(reader.GetOrdinal("PlannedTimeInSeconds")) ? 0 : reader.GetDouble("PlannedTimeInSeconds");
                        double downTime = reader.IsDBNull(reader.GetOrdinal("TotalDownTimeSeconds")) ? 0 : reader.GetDouble("TotalDownTimeSeconds");
                        double runTime = plannedTime > downTime ? plannedTime - downTime : 0;

                        int defectiveCount = reader.IsDBNull(reader.GetOrdinal("DefectiveProductionCount")) ? 0 : reader.GetInt32("DefectiveProductionCount");
                        string recipeName = reader.GetString("RecipeName");
                        int actualQuantity = reader.IsDBNull(reader.GetOrdinal("actual_produced_quantity")) ? 0 : reader.GetInt32("actual_produced_quantity");

                        // --- OEE HESAPLAMA DÜZELTMESİ ---
                        // Toplam üretilen adet olarak PLC'den gelen sayaç yerine operatörün girdiği gerçek miktarı kullanıyoruz.
                        int totalPiecesProduced = actualQuantity;
                        int goodCount = totalPiecesProduced - defectiveCount;

                        // 1. Availability (Kullanılabilirlik)
                        double availability = (plannedTime > 0) ? (runTime / plannedTime) * 100 : 0;

                        // 2. Performance (Performans) - Düzeltilmiş Hesaplama
                        double performance = 0;
                        if (runTime > 0 && totalPiecesProduced > 0 && !string.IsNullOrEmpty(recipeName))
                        {
                            var recipe = _recipeRepository.GetRecipeByName(recipeName);
                            if (recipe != null)
                            {
                                double totalTheoreticalTimeSeconds = RecipeAnalysis.CalculateTotalTheoreticalTimeSeconds(recipe);
                                if (totalTheoreticalTimeSeconds > 0)
                                {
                                    // Performans = (Toplam Teorik Süre / Fiili Çalışma Süresi) * 100
                                    // Bu formül, birim başına süreyi ve toplam adedi birleştirerek daha basit bir hal alır.
                                    performance = (totalTheoreticalTimeSeconds / runTime) * 100;
                                }
                            }
                        }

                        // 3. Quality (Kalite) - Düzeltilmiş Hesaplama
                        double quality = (totalPiecesProduced > 0) ? ((double)goodCount / totalPiecesProduced) * 100 : 0;

                        // OEE - Daha anlaşılır formül ile güncellendi
                        // Not: (A/100)*(P/100)*(Q/100)*100 ile (A*P*Q)/10000 aynı anlama gelir.
                        double oee = (availability * performance * quality) / 10000;

                        oeeList.Add(new OeeData
                        {
                            MachineName = reader.GetString("MachineName"),
                            BatchId = reader.GetString("BatchId"),
                            Availability = Math.Max(0, Math.Round(availability, 2)),
                            Performance = Math.Max(0, Math.Round(performance, 2)),
                            Quality = Math.Max(0, Math.Round(quality, 2)),
                            OEE = Math.Max(0, Math.Round(oee, 2))
                        });
                    }
                }
            }
            return oeeList;
        }
        public DataTable GetHourlyAverageOee(DateTime startDate)
        {
            var dt = new DataTable();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        HOUR(b.EndTime) AS Saat,
                        AVG(
                            CASE WHEN b.TotalProductionCount > 0 AND b.TheoreticalCycleTimeSeconds > 0 AND TIME_TO_SEC(TIMEDIFF(b.EndTime, b.StartTime)) > 0 THEN
                                (TIME_TO_SEC(TIMEDIFF(b.EndTime, b.StartTime)) - b.TotalDownTimeSeconds) / TIME_TO_SEC(TIMEDIFF(b.EndTime, b.StartTime)) *
                                (b.TheoreticalCycleTimeSeconds / TIME_TO_SEC(TIMEDIFF(b.EndTime, b.StartTime)) - b.TotalDownTimeSeconds) *
                                ( (b.TotalProductionCount - b.DefectiveProductionCount) / b.TotalProductionCount ) * 10000
                            ELSE
                                0
                            END
                        ) AS AverageOEE
                    FROM production_batches AS b
                    WHERE DATE(b.EndTime) = @SelectedDate AND b.EndTime IS NOT NULL
                    GROUP BY Saat
                    ORDER BY Saat;
                ";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SelectedDate", startDate.ToString("yyyy-MM-dd"));

                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }
        public DataTable GetHourlyFactoryConsumption(DateTime date)
        {
            var dt = new DataTable();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                // Bu sorgu, tamamlanmış batch'lerin verilerini saatlik olarak gruplayıp toplar.
                string query = @"
                    SELECT 
                        HOUR(EndTime) AS Saat,
                        SUM(TotalElectricity) AS ToplamElektrik,
                        SUM(TotalWater) AS ToplamSu,
                        SUM(TotalSteam) AS ToplamBuhar
                    FROM production_batches
                    WHERE DATE(EndTime) = @SelectedDate
                    GROUP BY HOUR(EndTime)
                    ORDER BY Saat;
                ";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SelectedDate", date.ToString("yyyy-MM-dd"));
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }

        // YENİ: Seçilen güne ait en çok tüketim yapan 5 makineyi getiren metot
        public DataTable GetTop5ConsumingMachines(DateTime date, string consumptionType)
        {
            var dt = new DataTable();
            string consumptionColumn = "";

            // Gelen parametreye göre SQL'deki sütun adını güvenli bir şekilde belirle
            switch (consumptionType.ToLower())
            {
                case "su":
                    consumptionColumn = "TotalWater";
                    break;
                case "buhar":
                    consumptionColumn = "TotalSteam";
                    break;
                default:
                    consumptionColumn = "TotalElectricity";
                    break;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = $@"
                    SELECT 
                        m.MachineName,
                        SUM({consumptionColumn}) AS ToplamTuketim
                    FROM production_batches b
                    JOIN machines m ON b.MachineId = m.Id
                    WHERE DATE(b.EndTime) = @SelectedDate
                    GROUP BY m.MachineName
                    ORDER BY ToplamTuketim DESC
                    LIMIT 5;
                ";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SelectedDate", date.ToString("yyyy-MM-dd"));
                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }
        // Bu metot şimdilik kullanılmıyor, olduğu gibi bırakabiliriz.
      
    }
}