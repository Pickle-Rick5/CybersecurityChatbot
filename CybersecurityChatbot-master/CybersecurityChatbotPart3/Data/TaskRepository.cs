using System.Linq;
using System;
using System.Collections.Generic;
using MySqlConnector;
using CybersecurityChatbotPart3.Models;

namespace CybersecurityChatbotPart3.Data
{
    public class TaskRepository
    {
        private readonly string _connectionString = DbConfig.GetConnectionString();

        public bool TestConnection(out string errorMessage)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();
                errorMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            const string sql = @"INSERT INTO Tasks (Title, Description, ReminderDate)
                                 VALUES (@title, @description, @reminder);
                                 SELECT LAST_INSERT_ID();";

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@reminder", (object?)reminderDate ?? DBNull.Value);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            const string sql = @"SELECT Id, Title, Description, ReminderDate, IsCompleted, CreatedAt
                                 FROM Tasks ORDER BY IsCompleted, CreatedAt DESC";

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                    ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? null : reader.GetDateTime("ReminderDate"),
                    IsCompleted = reader.GetBoolean("IsCompleted"),
                    CreatedAt = reader.GetDateTime("CreatedAt")
                });
            }
            return tasks;
        }

        public List<TaskItem> GetDueReminders()
        {
            return GetAllTasks()
                .Where(t => !t.IsCompleted && t.ReminderDate.HasValue && t.ReminderDate.Value <= DateTime.Now)
                .ToList();
        }

        public bool UpdateReminder(int id, DateTime reminderDate)
        {
            const string sql = "UPDATE Tasks SET ReminderDate = @reminder WHERE Id = @id";

            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@reminder", reminderDate);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteTask(int id) =>
            ExecuteById("DELETE FROM Tasks WHERE Id = @id", id);

        public bool MarkCompleted(int id) =>
            ExecuteById("UPDATE Tasks SET IsCompleted = TRUE WHERE Id = @id", id);

        private bool ExecuteById(string sql, int id)
        {
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}