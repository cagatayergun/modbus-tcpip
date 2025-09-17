// Repositories/AlarmRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TekstilScada.Core; // Bu satırı ekleyin
using TekstilScada.Core.Models;
using TekstilScada.Models;


namespace TekstilScada.Repositories
{
    public class AlarmRepository
    {
        private readonly string _connectionString = AppConfig.ConnectionString;

        public List<AlarmDefinition> GetAllAlarmDefinitions()
        {
            var definitions = new List<AlarmDefinition>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, AlarmNumber, AlarmText, Severity, Category FROM alarm_definitions ORDER BY AlarmNumber;";
                var cmd = new MySqlCommand(query, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        definitions.Add(new AlarmDefinition
                        {
                            Id = reader.GetInt32("Id"),
                            AlarmNumber = reader.GetInt32("AlarmNumber"),
                            AlarmText = reader.GetString("AlarmText"),
                            Severity = reader.GetInt32("Severity"),
                            Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString("Category")
                        });
                    }
                }
            }
            return definitions;
        }

        public AlarmDefinition GetAlarmDefinitionByNumber(int alarmNumber)
        {
            AlarmDefinition definition = null;
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, AlarmNumber, AlarmText, Severity, Category FROM alarm_definitions WHERE AlarmNumber = @AlarmNumber;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AlarmNumber", alarmNumber);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        definition = new AlarmDefinition
                        {
                            Id = reader.GetInt32("Id"),
                            AlarmNumber = reader.GetInt32("AlarmNumber"),
                            AlarmText = reader.GetString("AlarmText"),
                            Severity = reader.GetInt32("Severity"),
                            Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? null : reader.GetString("Category")
                        };
                    }
                }
            }
            return definition;
        }
        public List<Alarm> GetAlarmsByBatchId(string batchId)
        {
            var alarms = new List<Alarm>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                // alarms tablosunda BatchId'ye göre sorgulama yapıyoruz.
                // Alarm tanımını (AlarmDefinition) da join ile sorguya ekliyoruz.
                string query = @"
            SELECT 
                a.Id, a.MachineId, a.AlarmDefinitionId, a.BatchId, a.StartTime, a.EndTime, 
                ad.Code as AlarmCode, ad.Message as AlarmMessage, ad.Severity
            FROM alarms a
            JOIN alarm_definitions ad ON a.AlarmDefinitionId = ad.Id
            WHERE a.BatchId = @BatchId
            ORDER BY a.StartTime DESC;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@BatchId", batchId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        alarms.Add(new Alarm
                        {
                            Id = reader.GetInt32("Id"),
                            MachineId = reader.GetInt32("MachineId"),
                            AlarmDefinitionId = reader.GetInt32("AlarmDefinitionId"),
                            BatchId = reader.GetString("BatchId"),
                            StartTime = reader.GetDateTime("StartTime"),
                            EndTime = reader.GetDateTime("EndTime"),
                            // Join'den gelen ek bilgiler
                            AlarmCode = reader.GetString("AlarmCode"),
                            AlarmMessage = reader.GetString("AlarmMessage"),
                            Severity = reader.GetString("Severity")
                        });
                    }
                }
            }
            return alarms;
        }
        public void AddAlarmDefinition(AlarmDefinition definition)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO alarm_definitions (AlarmNumber, AlarmText, Severity, Category) VALUES (@AlarmNumber, @AlarmText, @Severity, @Category);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AlarmNumber", definition.AlarmNumber);
                cmd.Parameters.AddWithValue("@AlarmText", definition.AlarmText);
                cmd.Parameters.AddWithValue("@Severity", definition.Severity);
                cmd.Parameters.AddWithValue("@Category", definition.Category);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateAlarmDefinition(AlarmDefinition definition)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "UPDATE alarm_definitions SET AlarmNumber = @AlarmNumber, AlarmText = @AlarmText, Severity = @Severity, Category = @Category WHERE Id = @Id;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", definition.Id);
                cmd.Parameters.AddWithValue("@AlarmNumber", definition.AlarmNumber);
                cmd.Parameters.AddWithValue("@AlarmText", definition.AlarmText);
                cmd.Parameters.AddWithValue("@Severity", definition.Severity);
                cmd.Parameters.AddWithValue("@Category", definition.Category);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAlarmDefinition(int definitionId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM alarm_definitions WHERE Id = @Id;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", definitionId);
                cmd.ExecuteNonQuery();
            }
        }

        public void WriteAlarmHistoryEvent(int machineId, int alarmDefinitionId, string eventType, int? userId = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO alarm_history (MachineId, AlarmDefinitionId, EventType, EventTimestamp, AcknowledgedByUserId) VALUES (@MachineId, @AlarmDefinitionId, @EventType, @EventTimestamp, @UserId);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@AlarmDefinitionId", alarmDefinitionId);
                cmd.Parameters.AddWithValue("@EventType", eventType);
                cmd.Parameters.AddWithValue("@EventTimestamp", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public List<AlarmReportItem> GetAlarmReport(DateTime startTime, DateTime endTime, int? machineId)
        {
            var reportItems = new List<AlarmReportItem>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        m.MachineName,
                        ad.AlarmNumber,
                        ad.AlarmText,
                        active_event.EventTimestamp AS StartTime,
                        (SELECT MIN(ia.EventTimestamp) 
                         FROM alarm_history ia 
                         WHERE ia.MachineId = active_event.MachineId 
                           AND ia.AlarmDefinitionId = active_event.AlarmDefinitionId 
                           AND ia.EventType = 'INACTIVE' 
                           AND ia.EventTimestamp > active_event.EventTimestamp) AS EndTime
                    FROM alarm_history AS active_event
                    JOIN machines AS m ON active_event.MachineId = m.Id
                    JOIN alarm_definitions AS ad ON active_event.AlarmDefinitionId = ad.Id
                    WHERE 
                        active_event.EventType = 'ACTIVE' AND
                        active_event.EventTimestamp BETWEEN @StartTime AND @EndTime " +
                    (machineId.HasValue ? "AND active_event.MachineId = @MachineId " : "") +
                    @"ORDER BY StartTime DESC;";

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
                        var item = new AlarmReportItem
                        {
                            MachineName = reader.GetString("MachineName"),
                            AlarmNumber = reader.GetInt32("AlarmNumber"),
                            AlarmText = reader.GetString("AlarmText"),
                            StartTime = reader.GetDateTime("StartTime"),
                            EndTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? (DateTime?)null : reader.GetDateTime("EndTime")
                        };

                        if (item.EndTime.HasValue)
                        {
                            TimeSpan duration = item.EndTime.Value - item.StartTime;
                            item.Duration = duration.ToString(@"hh\:mm\:ss");
                        }
                        else
                        {
                            item.Duration = "Active";
                        }
                        reportItems.Add(item);
                    }
                }
            }
            return reportItems;
        }

        // YENİ: Bir partide meydana gelen alarmları getiren metot.
        public List<AlarmDetail> GetAlarmDetailsForBatch(string batchId, int machineId)
        {
            var details = new List<AlarmDetail>();
            DateTime startTime, endTime;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                // Önce batch'in başlangıç ve bitiş zamanını bul
                string batchQuery = "SELECT StartTime, EndTime FROM production_batches WHERE BatchId = @BatchId AND MachineId = @MachineId;";
                var batchCmd = new MySqlCommand(batchQuery, connection);
                batchCmd.Parameters.AddWithValue("@BatchId", batchId);
                batchCmd.Parameters.AddWithValue("@MachineId", machineId);

                using (var reader = batchCmd.ExecuteReader())
                {
                    if (!reader.Read()) return details; // Batch bulunamadı
                    startTime = reader.GetDateTime("StartTime");
                    endTime = reader.IsDBNull(reader.GetOrdinal("EndTime")) ? DateTime.Now : reader.GetDateTime("EndTime");
                }

                // Şimdi bu zaman aralığındaki alarmları çek
                string alarmQuery = @"
                    SELECT ad.AlarmText 
                    FROM alarm_history ah
                    JOIN alarm_definitions ad ON ah.AlarmDefinitionId = ad.Id
                    WHERE ah.MachineId = @MachineId 
                      AND ah.EventType = 'ACTIVE' 
                      AND ah.EventTimestamp BETWEEN @StartTime AND @EndTime
                    ORDER BY ah.EventTimestamp;";

                var alarmCmd = new MySqlCommand(alarmQuery, connection);
                alarmCmd.Parameters.AddWithValue("@MachineId", machineId);
                alarmCmd.Parameters.AddWithValue("@StartTime", startTime);
                alarmCmd.Parameters.AddWithValue("@EndTime", endTime);

                using (var reader = alarmCmd.ExecuteReader())
                {
                    int stepCounter = 1; // Adım no'yu şimdilik varsayımsal olarak artırıyoruz
                    while (reader.Read())
                    {
                        details.Add(new AlarmDetail
                        {
                            StepNumber = stepCounter++,
                            AlarmDescription = reader.GetString("AlarmText")
                        });
                    }
                }
            }
            return details;
        }
        public List<TopAlarmData> GetTopAlarmsByFrequency(DateTime startTime, DateTime endTime, int limit = 5)
        {
            var topAlarms = new List<TopAlarmData>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                ad.AlarmText, 
                COUNT(ah.Id) as AlarmCount
            FROM alarm_history ah
            JOIN alarm_definitions ad ON ah.AlarmDefinitionId = ad.Id
            WHERE 
                ah.EventType = 'ACTIVE' AND
                ah.EventTimestamp BETWEEN @StartTime AND @EndTime
            GROUP BY ad.AlarmText
            ORDER BY AlarmCount DESC
            LIMIT @Limit;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@Limit", limit);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topAlarms.Add(new TopAlarmData
                        {
                            AlarmText = reader.GetString("AlarmText"),
                            Count = reader.GetInt32("AlarmCount")
                        });
                    }
                }
            }
            return topAlarms;
        }
        public List<AlarmReportItem> GetAlarmsForDateRange(int machineId, DateTime startTime, DateTime endTime)
        {
            var reportItems = new List<AlarmReportItem>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                m.MachineName,
                ad.AlarmNumber,
                ad.AlarmText,
                ah.EventTimestamp AS StartTime
            FROM alarm_history AS ah
            JOIN machines AS m ON ah.MachineId = m.Id
            JOIN alarm_definitions AS ad ON ah.AlarmDefinitionId = ad.Id
            WHERE 
                ah.EventType = 'ACTIVE' AND
                ah.MachineId = @MachineId AND
                ah.EventTimestamp BETWEEN @StartTime AND @EndTime
            ORDER BY StartTime DESC;";

                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineId", machineId);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new AlarmReportItem
                        {
                            MachineName = reader.GetString("MachineName"),
                            AlarmNumber = reader.GetInt32("AlarmNumber"),
                            AlarmText = reader.GetString("AlarmText"),
                            StartTime = reader.GetDateTime("StartTime"),
                        };
                        reportItems.Add(item);
                    }
                }
            }
            return reportItems;
        }
    }
}
