using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PastPapers.Models
{
    public class DatabaseContext
    {

        public string ConnectionString { get; set; }

        public DatabaseContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            return connection;
        }

        public int GetUserID(string username)
        {
            try
            {
                MySqlConnection connection = GetConnection();
                connection.Open();
                string sql = "SELECT id FROM users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = cmd.ExecuteReader();

                int ID = 0;

                if (reader.Read())
                {
                    ID = reader.GetInt32(0);
                }

                reader.Close();
                connection.Close();

                return ID;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return -1;
            }            
        }

    }
}
