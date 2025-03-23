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
        private static MySqlConnection conn = null;
        
        public static void Connect()
        {
            Console.WriteLine("Connexion à MySQL...");
            conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de connexion MySQL : {ex.Message}");
            }
        }
        public static void Disconnect()
        {
            conn = new MySqlConnection(connectionString);
            try
            {
                conn.Close();
                Console.WriteLine("Déconnexion réussie !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de déconnexion MySQL : {ex.Message}");
            }
        }

        public static void GetAllDataFromTable(string tableName, List<string> columns)
        {
            try
            {
                using (conn)
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
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        returnArray = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            returnArray[i] = reader.GetValue(i).ToString();
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
        
        public static bool ExecuteNonQuery(string query)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur MySQL : {ex.Message}");
                return false;
            }
        }
        
        public static int CreateLobby(string dimension, string tempsLimite)
        {
            int newId = 1;
            try
            {
                string idQuery = "SELECT MAX(Id_Partie) FROM Gr3_Partie;";
                using (MySqlCommand cmd = new MySqlCommand(idQuery, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        newId = Convert.ToInt32(result) + 1;
                    }
                }

                string dateCreation = DateTime.Now.ToString("yyyy-MM-dd");

                string insertQuery = "INSERT INTO Gr3_Partie (Id_Partie, Dimension, Temps_limite, Etat_Partie, Date) " +
                                     "VALUES (@id, @dimension, @temps, @etat, @date);";

                using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", newId);
                    cmd.Parameters.AddWithValue("@dimension", dimension);
                    cmd.Parameters.AddWithValue("@temps", tempsLimite);
                    cmd.Parameters.AddWithValue("@etat", "ECO");
                    cmd.Parameters.AddWithValue("@date", dateCreation);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Partie créée avec succès, l'Id de la partie est : {newId}!");
                    }
                    else
                    {
                        Console.WriteLine("Échec de la création de la partie.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de la partie : {ex.Message}");
            }
            return newId;
        }

    }
}
