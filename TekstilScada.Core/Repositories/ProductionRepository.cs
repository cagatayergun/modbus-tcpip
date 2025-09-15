// Repositories/ProductionRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using TekstilScada.Models; // BU SATIR EKLENDİ
using System.Linq;
using System.Data;
using TekstilScada.Core; // Bu satırı ekleyin
namespace TekstilScada.Repositories
{
    public class ReportFilters
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? MachineId { get; set; }
        public string BatchNo { get; set; }
        public string RecipeName { get; set; }
        public string SiparisNo { get; set; }
        public string MusteriNo { get; set; }
        public string OperatorName { get; set; }
      
    }
    public class ProductionRepository
    {
        private readonly string _connectionString = AppConfig.ConnectionString;

        

        // ... Bu dosyanın geri kalan tüm metotları aynı kalacak ...
        // (GetProductionReport, StartNewBatch, EndBatch vb.)
        public List<ProductionReportItem> GetProductionReport(ReportFilters filters)
        {
            var reportItems = new List<ProductionReportItem>();
            var queryBuilder = new StringBuilder();

            queryBuilder.Append(@"
                SELECT 
                    m.Id as MachineId,
                    m.MachineName,
                    b.BatchId,
                    b.StartTime,
                    b.EndTime,
                    TIMEDIFF(b.EndTime, b.StartTime) as CycleTime,
                    b.RecipeName,
                    b.OperatorName,
                    b.MusteriNo,
                    b.MachineAlarmDurationSeconds, 
                     b.OperatorPauseDurationSeconds,
                    b.SiparisNo,
                     b.TheoreticalCycleTimeSeconds
                FROM production_batches AS b
                JOIN machines AS m ON b.MachineId = m.Id
            ");

            var whereClauses = new List<string>();
            whereClauses.Add("b.StartTime BETWEEN @StartTime AND @EndTime");
            if (filters.MachineId.HasValue && filters.MachineId > 0) whereClauses.Add("b.MachineId = @MachineId");
            if (!string.IsNullOrEmpty(filters.BatchNo)) whereClauses.Add("b.BatchId LIKE @BatchNo");
            if (!string.IsNullOrEmpty(filters.RecipeName)) whereClauses.Add("b.RecipeName LIKE @RecipeName");
            if (!string.IsNullOrEmpty(filters.SiparisNo)) whereClauses.Add("b.SiparisNo LIKE @SiparisNo");
            if (!string.IsNullOrEmpty(filters.MusteriNo)) whereClauses.Add("b.MusteriNo LIKE @MusteriNo");
            if (!string.IsNullOrEmpty(filters.OperatorName)) whereClauses.Add("b.OperatorName LIKE @OperatorName");

            queryBuilder.Append(" WHERE " + string.Join(" AND ", whereClauses));
            queryBuilder.Append(" ORDER BY b.StartTime DESC;");

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new MySqlCommand(queryBuilder.ToString(), connection);

                cmd.Parameters.AddWithValue("@StartTime", filters.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", filters.EndTime);
                if (filters.MachineId.HasValue && filters.MachineId > 0) cmd.Parameters.AddWithValue("@MachineId", filters.MachineId.Value);
                if (!string.IsNullOrEmpty(filters.BatchNo)) cmd.Parameters.AddWithValue("@BatchNo", $"%{filters.BatchNo}%");
                if (!string.IsNullOrEmpty(filters.RecipeName)) cmd.Parameters.AddWithValue("@RecipeName", $"%{filters.RecipeName}%");
                if (!string.IsNullOrEmpty(filters.SiparisNo)) cmd.Parameters.AddWithValue("@SiparisNo", $"%{filters.SiparisNo}%");
                if (!string.IsNullOrEmpty(filters.MusteriNo)) cmd.Parameters.AddWithValue("@MusteriNo", $"%{filters.MusteriNo}%");
                if (!string.IsNullOrEmpty(filters.OperatorName)) cmd.Parameters.AddWithValue("@OperatorName", $"%{filters.OperatorName}%");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reportItems.Add(new ProductionReportItem
                        {
                            MachineId = reader.GetInt32("MachineId"),
                            MachineName = reader.GetString("MachineName"),
                            BatchId = reader.GetString("BatchId"),
                            StartTime = reader.GetDateTime("StartTime"),
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? DateTime.MinValue : reader.GetDateTime("EndTime"),
                            CycleTime = reader.IsDBNull(reader.GetOrdinal("CycleTime"))
                                ? "Devam Ediyor"
                                : reader.GetTimeSpan(reader.GetOrdinal("CycleTime")).ToString(@"hh\:mm\:ss"),
                            RecipeName = reader.IsDBNull(reader.GetOrdinal("RecipeName")) ? "" : reader.GetString("RecipeName"),
                            OperatorName = reader.IsDBNull(reader.GetOrdinal("OperatorName")) ? "" : reader.GetString("OperatorName"),
                            MusteriNo = reader.IsDBNull(reader.GetOrdinal("MusteriNo")) ? "" : reader.GetString("MusteriNo"),
                            SiparisNo = reader.IsDBNull(reader.GetOrdinal("SiparisNo")) ? "" : reader.GetString("SiparisNo"),
                              MachineAlarmDurationSeconds = reader.IsDBNull(reader.GetOrdinal("MachineAlarmDurationSeconds")) ? 0 : reader.GetInt32("MachineAlarmDurationSeconds"),
                            OperatorPauseDurationSeconds = reader.IsDBNull(reader.GetOrdinal("OperatorPauseDurationSeconds")) ? 0 : reader.GetInt32("OperatorPauseDurationSeconds"),
                            TheoreticalCycleTimeSeconds = reader.IsDBNull(reader.GetOrdinal("TheoreticalCycleTimeSeconds")) ? 0 : reader.GetInt32("TheoreticalCycleTimeSeconds")
                        });
                    }
                }
            }
            return reportItems;
        }

        public void StartNewBatch(FullMachineStatus status)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO production_batches (MachineId, BatchId, RecipeName, OperatorName, MusteriNo, SiparisNo, StartTime) VALUES (@MachineId, @BatchId, @RecipeName, @OperatorName, @MusteriNo, @SiparisNo, @StartTime) ON DUPLICATE KEY UPDATE StartTime=@StartTime, EndTime=NULL;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", status.MachineId);
                cmd.Parameters.AddWithValue("@BatchId", status.BatchNumarasi);
                cmd.Parameters.AddWithValue("@RecipeName", status.RecipeName);
                cmd.Parameters.AddWithValue("@OperatorName", status.OperatorIsmi);
                cmd.Parameters.AddWithValue("@MusteriNo", status.MusteriNumarasi);
                cmd.Parameters.AddWithValue("@SiparisNo", status.SiparisNumarasi);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }

        public void EndBatch(int machineId, string batchId, FullMachineStatus finalStatus, int machineAlarmSeconds, int operatorPauseSeconds, int actualProducedQuantity, int calculatedTotalDowntimeSeconds, double theoreticalCycleTimeSeconds)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
            UPDATE production_batches SET 
                EndTime = @EndTime,
                TotalProductionCount = @TotalProductionCount,
                DefectiveProductionCount = @DefectiveProductionCount,
                TotalDownTimeSeconds = @TotalDownTimeSeconds, 
                MachineAlarmDurationSeconds = @MachineAlarmDuration,
                OperatorPauseDurationSeconds = @OperatorPauseDuration,
                actual_produced_quantity = @actual_produced_quantity,
                TheoreticalCycleTimeSeconds = @TheoreticalCycleTimeSeconds
            WHERE 
                MachineId = @MachineId AND BatchId = @BatchId AND EndTime IS NULL;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@EndTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@TotalProductionCount", finalStatus.TotalProductionCount);
                cmd.Parameters.AddWithValue("@DefectiveProductionCount", finalStatus.DefectiveProductionCount);
                cmd.Parameters.AddWithValue("@TotalDownTimeSeconds", calculatedTotalDowntimeSeconds);
                cmd.Parameters.AddWithValue("@MachineAlarmDuration", machineAlarmSeconds);
                cmd.Parameters.AddWithValue("@OperatorPauseDuration", operatorPauseSeconds);
                cmd.Parameters.AddWithValue("@actual_produced_quantity", actualProducedQuantity);
                cmd.Parameters.AddWithValue("@TheoreticalCycleTimeSeconds", theoreticalCycleTimeSeconds);

                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBatchSummary(int machineId, string batchId, BatchSummaryData summary)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                UPDATE production_batches 
                SET 
                    TotalWater = @TotalWater, 
                    TotalElectricity = @TotalElectricity, 
                    TotalSteam = @TotalSteam 
                WHERE 
                    MachineId = @MachineId AND BatchId = @BatchId;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TotalWater", summary.TotalWater);
                cmd.Parameters.AddWithValue("@TotalElectricity", summary.TotalElectricity);
                cmd.Parameters.AddWithValue("@TotalSteam", summary.TotalSteam);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.ExecuteNonQuery();
            }
        }

        public void LogAllStepDetails(int machineId, string batchId, List<ProductionStepDetail> steps)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                foreach (var step in steps)
                {
                    string query = @"
                        INSERT INTO production_step_logs 
                        (MachineId, BatchId, StepNumber, TheoreticalTime, WorkingTime, StopTime, DeflectionTime, LogTimestamp) 
                        VALUES 
                        (@MachineId, @BatchId, @StepNumber, @TheoreticalTime, @WorkingTime, @StopTime, @DeflectionTime, @LogTimestamp);";

                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MachineId", machineId);
                    cmd.Parameters.AddWithValue("@BatchId", batchId);
                    cmd.Parameters.AddWithValue("@StepNumber", step.StepNumber);
                    cmd.Parameters.AddWithValue("@TheoreticalTime", step.TheoreticalTime);
                    cmd.Parameters.AddWithValue("@WorkingTime", step.WorkingTime);
                    cmd.Parameters.AddWithValue("@StopTime", step.StopTime);
                    cmd.Parameters.AddWithValue("@DeflectionTime", step.DeflectionTime);
                    cmd.Parameters.AddWithValue("@LogTimestamp", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ProductionStepDetail> GetProductionStepDetails(string batchId, int machineId)
        {
            var details = new List<ProductionStepDetail>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM production_step_logs WHERE MachineId = @MachineId AND BatchId = @BatchId ORDER BY StepNumber;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@BatchId", batchId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        details.Add(new ProductionStepDetail
                        {
                            StepNumber = reader.GetInt32("StepNumber"),
                            StepName = reader.IsDBNull(reader.GetOrdinal("StepName")) ? "" : reader.GetString("StepName"),
                            TheoreticalTime = reader.IsDBNull(reader.GetOrdinal("TheoreticalTime")) ? "" : reader.GetString("TheoreticalTime"),
                            WorkingTime = reader.IsDBNull(reader.GetOrdinal("WorkingTime")) ? "" : reader.GetString("WorkingTime"),
                            StopTime = reader.IsDBNull(reader.GetOrdinal("StopTime")) ? "" : reader.GetString("StopTime"),
                            DeflectionTime = reader.IsDBNull(reader.GetOrdinal("DeflectionTime")) ? "" : reader.GetString("DeflectionTime")
                        });
                    }
                }
            }
            return details;
        }

        public void LogChemicalConsumption(int machineId, string batchId, List<ChemicalConsumptionData> consumptionData)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                foreach (var data in consumptionData)
                {
                    string query = @"
                        INSERT INTO production_chemical_logs 
                        (MachineId, BatchId, StepNumber, ChemicalName, AmountLiters) 
                        VALUES 
                        (@MachineId, @BatchId, @StepNumber, @ChemicalName, @AmountLiters);";

                    var cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MachineId", machineId);
                    cmd.Parameters.AddWithValue("@BatchId", batchId);
                    cmd.Parameters.AddWithValue("@StepNumber", data.StepNumber);
                    cmd.Parameters.AddWithValue("@ChemicalName", data.ChemicalName);
                    cmd.Parameters.AddWithValue("@AmountLiters", data.AmountLiters);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<ChemicalConsumptionData> GetChemicalConsumptionForBatch(string batchId, int machineId)
        {
            var consumptionList = new List<ChemicalConsumptionData>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT StepNumber, ChemicalName, AmountLiters FROM production_chemical_logs WHERE MachineId = @MachineId AND BatchId = @BatchId ORDER BY StepNumber;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@BatchId", batchId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        consumptionList.Add(new ChemicalConsumptionData
                        {
                            StepNumber = reader.GetInt32("StepNumber"),
                            ChemicalName = reader.GetString("ChemicalName"),
                            AmountLiters = reader.GetInt16("AmountLiters")
                        });
                    }
                }
            }
            return consumptionList;
        }
        // YENİ: Seçilen makine listesi ve tarihe göre üretim verilerini çeken metot
        public DataTable GetGeneralProductionReport(DateTime startTime, DateTime endTime, List<string> machineNames)
        {
            var dt = new DataTable();
            if (machineNames == null || !machineNames.Any())
            {
                return dt; // Makine seçilmemişse boş tablo döndür
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT 
                        m.MachineName,
                        b.BatchId,
                        b.EndTime,
                        b.TotalWater,
                        b.TotalElectricity,
                        b.TotalSteam
                    FROM production_batches b
                    JOIN machines m ON b.MachineId = m.Id
                    WHERE b.EndTime BETWEEN @StartTime AND @EndTime 
                    AND m.MachineName IN ({0})
                    ORDER BY m.MachineName, b.EndTime;";

                var machineParams = machineNames.Select((s, i) => "@machine" + i).ToArray();
                var formattedQuery = string.Format(query, string.Join(", ", machineParams));

                var cmd = new MySqlCommand(formattedQuery, connection);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                for (int i = 0; i < machineNames.Count; i++)
                {
                    cmd.Parameters.AddWithValue(machineParams[i], machineNames[i]);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }
            return dt;
        }
        // YENİ: Belirli bir periyot için toplam tüketimleri hesaplayan metot
        public ConsumptionTotals GetConsumptionTotalsForPeriod(DateTime startTime, DateTime endTime)
        {
            var totals = new ConsumptionTotals();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    SELECT 
                        SUM(TotalWater) as Water, 
                        SUM(TotalElectricity) as Electricity, 
                        SUM(TotalSteam) as Steam 
                    FROM production_batches 
                    WHERE EndTime BETWEEN @StartTime AND @EndTime;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        totals.TotalWater = reader["Water"] == DBNull.Value ? 0 : reader.GetDecimal("Water");
                        totals.TotalElectricity = reader["Electricity"] == DBNull.Value ? 0 : reader.GetDecimal("Electricity");
                        totals.TotalSteam = reader["Steam"] == DBNull.Value ? 0 : reader.GetDecimal("Steam");
                    }
                }
            }
            return totals;
        }
        public (DateTime? StartTime, DateTime? EndTime) GetBatchTimestamps(string batchId, int machineId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT StartTime, EndTime FROM production_batches WHERE BatchId = @BatchId AND MachineId = @MachineId;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var startTime = reader.GetDateTime("StartTime");
                        var endTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime("EndTime");
                        return (startTime, endTime);
                    }
                }
            }
            return (null, null);
        }
        // ProductionRepository.cs dosyasının içine bu yeni metodu ekleyin

        public void LogSingleStepDetail(ProductionStepDetail stepDetail, int machineId, string batchId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
            INSERT INTO production_step_logs 
            (MachineId, BatchId, StepNumber, StepName, TheoreticalTime, WorkingTime, StopTime, DeflectionTime, LogTimestamp) 
            VALUES 
            (@MachineId, @BatchId, @StepNumber, @StepName, @TheoreticalTime, @WorkingTime, @StopTime, @DeflectionTime, @LogTimestamp);";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@BatchId", batchId);
                cmd.Parameters.AddWithValue("@StepNumber", stepDetail.StepNumber);
                cmd.Parameters.AddWithValue("@StepName", stepDetail.StepName);
                cmd.Parameters.AddWithValue("@TheoreticalTime", stepDetail.TheoreticalTime);
                cmd.Parameters.AddWithValue("@WorkingTime", stepDetail.WorkingTime);
                cmd.Parameters.AddWithValue("@StopTime", stepDetail.StopTime);
                cmd.Parameters.AddWithValue("@DeflectionTime", stepDetail.DeflectionTime);
                cmd.Parameters.AddWithValue("@LogTimestamp", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
        }
        public void SaveBatchRecipe(int machineId, string batchId, ScadaRecipe recipe)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Reçetenin kendisini production_batches tablosuna güncelleyin (opsiyonel)
                        string updateQuery = "UPDATE production_batches SET RecipeName = @RecipeName WHERE MachineId = @MachineId AND BatchId = @BatchId;";
                        var updateCmd = new MySqlCommand(updateQuery, connection, transaction);
                        updateCmd.Parameters.AddWithValue("@RecipeName", recipe.RecipeName);
                        updateCmd.Parameters.AddWithValue("@MachineId", machineId);
                        updateCmd.Parameters.AddWithValue("@BatchId", batchId);
                        updateCmd.ExecuteNonQuery();

                        // Yeni tabloya reçete adımlarını kaydet
                        string stepQuery = "INSERT INTO batch_recipe_steps (MachineId, BatchId, StepNumber, Word0, Word1, Word2, Word3, Word4, Word5, Word6, Word7, Word8, Word9, Word10, Word11, Word12, Word13, Word14, Word15, Word16, Word17, Word18, Word19, Word20, Word24) " +
                                           "VALUES (@MachineId, @BatchId, @StepNumber, @Word0, @Word1, @Word2, @Word3, @Word4, @Word5, @Word6, @Word7, @Word8, @Word9, @Word10, @Word11, @Word12, @Word13, @Word14, @Word15, @Word16, @Word17, @Word18, @Word19, @Word20, @Word24);";

                        foreach (var step in recipe.Steps)
                        {
                            var stepCmd = new MySqlCommand(stepQuery, connection, transaction);
                            stepCmd.Parameters.AddWithValue("@MachineId", machineId);
                            stepCmd.Parameters.AddWithValue("@BatchId", batchId);
                            stepCmd.Parameters.AddWithValue("@StepNumber", step.StepNumber);
                            for (int i = 0; i <= 24; i++)
                            {
                                if (i >= 21 && i <= 23) continue;
                                stepCmd.Parameters.AddWithValue($"@Word{i}", step.StepDataWords[i]);
                            }
                            stepCmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public ScadaRecipe GetBatchRecipe(int machineId, string batchId)
        {
            var recipe = new ScadaRecipe();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM batch_recipe_steps WHERE MachineId = @MachineId AND BatchId = @BatchId ORDER BY StepNumber;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@BatchId", batchId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var step = new ScadaRecipeStep
                        {
                            StepNumber = reader.GetInt32("StepNumber")
                        };

                        // Sayısal Word değerlerini oku
                        for (int i = 0; i <= 20; i++)
                        {
                            step.StepDataWords[i] = reader.GetInt16($"Word{i}");
                        }

                        // String Word değerlerini oku
                      

                        // Son sayısal Word değerini oku
                        step.StepDataWords[24] = reader.GetInt16("Word24");

                        recipe.Steps.Add(step);
                    }
                }
            }
            return recipe;
        }
    }
}
