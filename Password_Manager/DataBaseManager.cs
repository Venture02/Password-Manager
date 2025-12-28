using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Password_Manager
{
    public class DataBaseManager
    {
        private readonly string dbFilePath;
        private readonly string _connectionString;

        public DataBaseManager()
        {
            // Use a Database folder inside the app directory
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");

            // Create folder if it doesn't exist
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Set the database file path
            dbFilePath = Path.Combine(folder, "passwords.db");

            // Connection string
            _connectionString = $"Data Source={dbFilePath};Version=3;";

            // If the DB doesn't exist, create it
            if (!File.Exists(dbFilePath))
            {
                CreateDatabase();
            }
        }

        // Get SQLite connection
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        // Create the database and table
        private void CreateDatabase()
        {
            // Create the SQLite file
            SQLiteConnection.CreateFile(dbFilePath);

            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Passwords (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Website TEXT,
                            Username TEXT,
                            PasswordEncrypted TEXT,
                            DateAdded TEXT
                        );
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetEncryptedPasswordById(int id)
        {
            using (SQLiteConnection connection = GetConnection())
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT PasswordEncrypted FROM Passwords WHERE Id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    object result = command.ExecuteScalar();

                    if (result == null)
                    {
                        return null;
                    }

                    return result.ToString();
                }
            }
        }
    }
}
