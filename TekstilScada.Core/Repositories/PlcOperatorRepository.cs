// Repositories/PlcOperatorRepository.cs
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TekstilScada.Models;
using TekstilScada.Core; // Bu satırı ekleyin
namespace TekstilScada.Repositories
{
    public class PlcOperatorRepository
    {
        private readonly string _connectionString = AppConfig.ConnectionString;

        public List<PlcOperator> GetAll()
        {
            var operators = new List<PlcOperator>();
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, UserId, Password FROM plc_operator_templates ORDER BY Name;";
                var cmd = new MySqlCommand(query, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        operators.Add(new PlcOperator
                        {
                            // Bu Id veritabanı Id'sidir, SlotIndex değil.
                            SlotIndex = reader.GetInt32("Id"),
                            Name = reader.GetString("Name"),
                            UserId = reader.GetInt16("UserId"),
                            Password = reader.GetInt16("Password")
                        });
                    }
                }
            }
            return operators;
        }
        public void AddDefaultOperator()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                // Yeni boş bir operatör şablonu ekle
                string query = "INSERT INTO plc_operator_templates (Name, UserId, Password) VALUES (@Name, @UserId, @Password);";
                var cmd = new MySqlCommand(query, connection);
                // Varsayılan boş veya sıfır değerleri ekliyoruz.
                cmd.Parameters.AddWithValue("@Name", "");
                cmd.Parameters.AddWithValue("@UserId", 0);
                cmd.Parameters.AddWithValue("@Password", 0);
                cmd.ExecuteNonQuery();
            }
        }
        public void SaveOrUpdate(PlcOperator op)
        {
            // Aynı isim ve ID'ye sahip bir kayıt var mı diye kontrol et
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string checkQuery = "SELECT Id FROM plc_operator_templates WHERE Name = @Name AND UserId = @UserId;";
                var checkCmd = new MySqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@Name", op.Name);
                checkCmd.Parameters.AddWithValue("@UserId", op.UserId);
                var existingId = checkCmd.ExecuteScalar();

                if (existingId != null)
                {
                    // Varsa güncelle (üstüne yaz)
                    string updateQuery = "UPDATE plc_operator_templates SET Password = @Password WHERE Id = @Id;";
                    var updateCmd = new MySqlCommand(updateQuery, connection);
                    updateCmd.Parameters.AddWithValue("@Password", op.Password);
                    updateCmd.Parameters.AddWithValue("@Id", Convert.ToInt32(existingId));
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Yoksa yeni ekle
                    string insertQuery = "INSERT INTO plc_operator_templates (Name, UserId, Password) VALUES (@Name, @UserId, @Password);";
                    var insertCmd = new MySqlCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@Name", op.Name);
                    insertCmd.Parameters.AddWithValue("@UserId", op.UserId);
                    insertCmd.Parameters.AddWithValue("@Password", op.Password);
                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int templateId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM plc_operator_templates WHERE Id = @Id;";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Id", templateId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}