// Repositories/MachineRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TekstilScada.Models;
using TekstilScada.Core; // Bu satırı ekleyin


namespace TekstilScada.Repositories
{
    public class MachineRepository
    {
        private readonly string _connectionString = AppConfig.ConnectionString;


        public List<Machine> GetAllMachines()
        {
            var machines = new List<Machine>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Id, MachineUserDefinedId, MachineName, IpAddress, Port, MachineType, IsEnabled, VncAddress, VncPassword, FtpUsername, FtpPassword, MachineSubType FROM machines ORDER BY Id;";
                    var cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var machine = new Machine
                            {
                                Id = reader.GetInt32("Id"),
                                MachineUserDefinedId = reader.GetString("MachineUserDefinedId"),
                                MachineName = reader.IsDBNull(reader.GetOrdinal("MachineName")) ? string.Empty : reader.GetString("MachineName"),
                                IpAddress = reader.GetString("IpAddress"),
                                Port = reader.GetInt32("Port"),
                                MachineType = reader.IsDBNull(reader.GetOrdinal("MachineType")) ? string.Empty : reader.GetString("MachineType"),
                                IsEnabled = reader.GetBoolean("IsEnabled"),
                                VncAddress = reader.IsDBNull(reader.GetOrdinal("VncAddress")) ? string.Empty : reader.GetString("VncAddress"),
                                VncPassword = reader.IsDBNull(reader.GetOrdinal("VncPassword")) ? string.Empty : reader.GetString("VncPassword"),
                                // GÜNCELLENDİ: FTP alanları okunuyor
                                FtpUsername = reader.IsDBNull(reader.GetOrdinal("FtpUsername")) ? string.Empty : reader.GetString("FtpUsername"),
                                FtpPassword = reader.IsDBNull(reader.GetOrdinal("FtpPassword")) ? string.Empty : reader.GetString("FtpPassword"),
                                MachineSubType = reader.IsDBNull(reader.GetOrdinal("MachineSubType")) ? string.Empty : reader.GetString("MachineSubType") // YENİ SATIR
                            };
                            machines.Add(machine);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine($"Veritabanı hatası (GetAllMachines): {ex.Message}");
                    return new List<Machine>();
                }
            }
            return machines;
        }

        public List<Machine> GetAllEnabledMachines()
        {
            // YENİ VE OPTİMİZE EDİLMİŞ YAPI
            var machines = new List<Machine>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    // Filtrelemeyi doğrudan SQL sorgusunda yapıyoruz
                    string query = "SELECT Id, MachineUserDefinedId, MachineName, IpAddress, Port, MachineType, IsEnabled, VncAddress, VncPassword, FtpUsername, FtpPassword, MachineSubType FROM machines WHERE IsEnabled = TRUE ORDER BY Id;";
                    var cmd = new MySqlCommand(query, connection);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var machine = new Machine
                            {
                                Id = reader.GetInt32("Id"),
                                MachineUserDefinedId = reader.GetString("MachineUserDefinedId"),
                                MachineName = reader.IsDBNull(reader.GetOrdinal("MachineName")) ? string.Empty : reader.GetString("MachineName"),
                                IpAddress = reader.GetString("IpAddress"),
                                Port = reader.GetInt32("Port"),
                                MachineType = reader.IsDBNull(reader.GetOrdinal("MachineType")) ? string.Empty : reader.GetString("MachineType"),
                                IsEnabled = reader.GetBoolean("IsEnabled"),
                                VncAddress = reader.IsDBNull(reader.GetOrdinal("VncAddress")) ? string.Empty : reader.GetString("VncAddress"),
                                VncPassword = reader.IsDBNull(reader.GetOrdinal("VncPassword")) ? string.Empty : reader.GetString("VncPassword"),
                                // GÜNCELLENDİ: FTP alanları okunuyor
                                FtpUsername = reader.IsDBNull(reader.GetOrdinal("FtpUsername")) ? string.Empty : reader.GetString("FtpUsername"),
                                FtpPassword = reader.IsDBNull(reader.GetOrdinal("FtpPassword")) ? string.Empty : reader.GetString("FtpPassword"),
                                MachineSubType = reader.IsDBNull(reader.GetOrdinal("MachineSubType")) ? string.Empty : reader.GetString("MachineSubType") // YENİ SATIR
                            };
                            machines.Add(machine);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Veritabanı hatası (GetAllEnabledMachines): {ex.Message}");
                    return new List<Machine>();
                }
            }
            return machines;
        }

        public void AddMachine(Machine machine)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                // DÜZELTME: MachineSubType sütunu INSERT sorgusuna eklendi.
                string query = "INSERT INTO machines (MachineUserDefinedId, MachineName, IpAddress, Port, MachineType, IsEnabled, VncAddress, VncPassword, FtpUsername, FtpPassword, MachineSubType) VALUES (@MachineUserDefinedId, @MachineName, @IpAddress, @Port, @MachineType, @IsEnabled, @VncAddress, @VncPassword, @FtpUsername, @FtpPassword, @MachineSubType);";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@MachineUserDefinedId", machine.MachineUserDefinedId);
                cmd.Parameters.AddWithValue("@MachineName", machine.MachineName);
                cmd.Parameters.AddWithValue("@IpAddress", machine.IpAddress);
                cmd.Parameters.AddWithValue("@Port", machine.Port);
                cmd.Parameters.AddWithValue("@MachineType", machine.MachineType);
                cmd.Parameters.AddWithValue("@IsEnabled", machine.IsEnabled);
                cmd.Parameters.AddWithValue("@VncAddress", machine.VncAddress);
                cmd.Parameters.AddWithValue("@VncPassword", machine.VncPassword);
                // GÜNCELLENDİ: FTP alanları parametrelere eklendi
                cmd.Parameters.AddWithValue("@FtpUsername", machine.FtpUsername);
                cmd.Parameters.AddWithValue("@FtpPassword", machine.FtpPassword);
                cmd.Parameters.AddWithValue("@MachineSubType", machine.MachineSubType); // YENİ SATIR
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMachine(Machine machine)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                // GÜNCELLENDİ: FTP alanları sorguya eklendi
                string query = "UPDATE machines SET MachineUserDefinedId = @MachineUserDefinedId, MachineName = @MachineName, IpAddress = @IpAddress, Port = @Port, MachineType = @MachineType, IsEnabled = @IsEnabled, VncAddress = @VncAddress, VncPassword = @VncPassword, FtpUsername = @FtpUsername, FtpPassword = @FtpPassword, MachineSubType = @MachineSubType WHERE Id = @Id;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", machine.Id);
                cmd.Parameters.AddWithValue("@MachineUserDefinedId", machine.MachineUserDefinedId);
                cmd.Parameters.AddWithValue("@MachineName", machine.MachineName);
                cmd.Parameters.AddWithValue("@IpAddress", machine.IpAddress);
                cmd.Parameters.AddWithValue("@Port", machine.Port);
                cmd.Parameters.AddWithValue("@MachineType", machine.MachineType);
                cmd.Parameters.AddWithValue("@IsEnabled", machine.IsEnabled);
                cmd.Parameters.AddWithValue("@VncAddress", machine.VncAddress);
                cmd.Parameters.AddWithValue("@VncPassword", machine.VncPassword);
                // GÜNCELLENDİ: FTP alanları parametrelere eklendi
                cmd.Parameters.AddWithValue("@FtpUsername", machine.FtpUsername);
                cmd.Parameters.AddWithValue("@FtpPassword", machine.FtpPassword);
                cmd.Parameters.AddWithValue("@MachineSubType", machine.MachineSubType); // YENİ SATIR
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMachine(int machineId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM machines WHERE Id = @Id;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", machineId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}