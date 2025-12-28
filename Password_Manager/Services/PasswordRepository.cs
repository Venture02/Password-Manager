using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace Password_Manager.Services
{
    internal class PasswordRepository
    {
        private DataBaseManager dataBase = new DataBaseManager();

        public void AddPassword(string website, string username, string encryptedPassword) // Add password to the database
        {
            using (SQLiteConnection connection = dataBase.GetConnection()) 
            {
                connection.Open(); // Open the database

                using (SQLiteCommand command = connection.CreateCommand()) // Write the command to use on SQL
                {
                    command.CommandText = @"
                        INSERT INTO Passwords (Website, Username, PasswordEncrypted, DateAdded)
                        VALUES (@w, @u, @p, datetime('now'));"; // Query to add the values

                    command.Parameters.AddWithValue("@w", website);
                    command.Parameters.AddWithValue("@u", username);
                    command.Parameters.AddWithValue("@p", encryptedPassword);

                    command.ExecuteNonQuery(); // Execute the query that I wrote
                }
            }
        }

        public DataTable GetPasswords() // Get all the passwords from the database
        {
            using (SQLiteConnection connection = dataBase.GetConnection())
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Passwords;"; // Return all the values

                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
        public void DeletePassword(int id) // Delete the selected password
        {
            using (SQLiteConnection connection = dataBase.GetConnection())
            {
                connection.Open();

                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Passwords WHERE Id = @id";

                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
