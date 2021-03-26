using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Server_Auction
{
    class DB
    {
        private static string LoadConnectionString(string name = "MySQL")
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static bool AddBid(string itemId, string amount, string userId)
        {
            string query = $"UPDATE items SET highest_bid='{amount}', bidder_id='{userId}' WHERE id='{itemId}'";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
                return false;
            }
        }
        public static bool UserLogIn(string userId, string password)
        {
            bool exists = false;

            string query = $"SELECT * FROM users WHERE id='{userId}' AND password='{password}'";

            // Prepare the connection
            MySqlConnection dbConnection = new MySqlConnection(LoadConnectionString());
            MySqlCommand dbCommand = new MySqlCommand(query, dbConnection);
            dbCommand.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();

                if (reader.Read())
                {
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
            }

            return exists;
        }
    }
}
