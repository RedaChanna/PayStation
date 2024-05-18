using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
namespace PayStationName
{
    public class MySQLDatabaseManager
    {
        private string connectionString;

        public MySQLDatabaseManager(string server, string username, string password)
        {
            connectionString = $"Server={server};Uid={username};Pwd={password};";
        }

        public void CreateDatabase(string databaseName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {databaseName}", connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Database '{databaseName}' created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating database: {ex.Message}");
                }
            }
        }
    }


}