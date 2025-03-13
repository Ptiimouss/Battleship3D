using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip3D_TP
{
    class MySQLDatabase
    {
        private static string connectionString = "Server=81.1.20.23;Database=USRS6N_1;UserId=EtudiantJvd;Password=!?CnamNAQ01?!;";

        // Connexion MySQL
        public static MySqlConnection Connect()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        // Récupérer des données depuis une table MySQL, mettre en paramètre le nom de la table et une liste de colonnes
        public static void GetAllDataFromTable(string tableName, List<string> columns)
        {
            try
            {
                using (MySqlConnection conn = Connect())
                {
                    string columnsString = string.Join(", ", columns);
                    string query = $"SELECT {columnsString} FROM {tableName} LIMIT 10;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"Données de la table {tableName} (colonnes: {columnsString}):");
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                // Affiche les données de chaque colonne
                                Console.WriteLine($"{reader.GetName(i)}: {reader[i]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion MySQL : {ex.Message}");
            }
        }

        public static string[] CustomQuery(string query)
        {
            string[] returnArray = null;
            try
            {
                using (MySqlConnection conn = Connect())
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnArray = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                returnArray[i] = (string)reader[i];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion MySQL : {ex.Message}");
            }
            return returnArray;
        }
    }
}
