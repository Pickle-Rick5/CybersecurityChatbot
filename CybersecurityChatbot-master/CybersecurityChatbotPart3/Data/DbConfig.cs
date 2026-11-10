using System;
using System.IO;
using MySqlConnector;

namespace CybersecurityChatbotPart3.Data
{
    public static class DbConfig
    {
        public static string GetConnectionString()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbconfig.txt");
            string password = File.Exists(path) ? File.ReadAllText(path).Trim() : "";

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "127.0.0.1",
                Port = 3306,
                Database = "CyberChatbotDB",
                UserID = "root",
                Password = password
            };
            return builder.ConnectionString;
        }
    }
}