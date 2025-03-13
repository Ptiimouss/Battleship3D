using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MongoDB.Driver;
using MongoDB.Bson;

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

class MongoDatabase
{
    private static string connectionString = "mongodb://AdminLJV:!!DBLjv1858**@81.1.20.23:27017/";
    private static MongoClient client = new MongoClient(connectionString);
    private static IMongoDatabase database = client.GetDatabase("USRS6N_2025");

    // Connexion à une collection
    public static IMongoCollection<BsonDocument> GetCollection(string collectionName)
    {
        return database.GetCollection<BsonDocument>(collectionName);
    }

    // Récupérer des données depuis une collection MongoDB, mettre en paramètre le nom de la collection et une liste de champs
    public static void GetAllDataFromCollection(string collectionName, List<string> fields)
    {
        try
        {
            var collection = GetCollection(collectionName);
            
            var projection = new BsonDocument();
            foreach (var field in fields)
            {
                projection.Add(field, 1);
            }

            var documents = collection.Find(new BsonDocument()).Project<BsonDocument>(projection).ToList();

            Console.WriteLine($"Documents dans la collection {collectionName} (champs: {string.Join(", ", fields)}):");
            foreach (var doc in documents)
            {
                Console.WriteLine(doc.ToJson());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur de connexion MongoDB : {ex.Message}");
        }
    }
}

class BattleshipLogger
{   
    public void Login()
    {
        Console.WriteLine("Welcome to BattleShip 3D !");
        Console.WriteLine("Enter your pseudo :");
        bool player_found = false;
        string pseudo = "";
        while (!player_found)
        {
            pseudo = Console.ReadLine();
            string[] result = MySQLDatabase.CustomQuery($"SELECT Pseudo FROM Gr3_Joueur WHERE Pseudo = \"{pseudo}\";");
            if (result != null) 
            {
                player_found = true;
            }
            else {
                Console.WriteLine("Player Not Found, please enter a new pseudo : ");
                // @TODO ASK TO CREATE A NEW ACCOUNT
            }
        }
        bool player_connected = false;
        Console.WriteLine("Enter your password :");
        while (!player_connected)
        {
            string password = Console.ReadLine();
            string[] result = MySQLDatabase.CustomQuery($"SELECT Pseudo FROM Gr3_Joueur WHERE Pseudo = \"{pseudo}\" AND Mot_De_Passe = \"{password}\"  ;");
            if (result != null)
            {
                player_connected = true;
            }
            else
            {
                Console.WriteLine("Password incorrect, please try again : ");
                // @TODO ASK IF BACK TO LOGIN
            }
        }
        Console.WriteLine("Player connected !");
    }
}

class Program
{
    static void Main()
    {
        BattleshipLogger battleshipLogger = new BattleshipLogger();
        battleshipLogger.Login();

        /*
        // Liste de colonnes à récupérer dans MySQL
        
        Console.WriteLine("Connexion à MySQL...");
        List<string> fieldsSQL = new List<string> { "Pseudo", "Nom","Prenom" };
        MySQLDatabase.GetAllDataFromTable("Gr3_Joueur", fieldsSQL);
        
        // Liste de champs à récupérer dans MongoDB
        
        Console.WriteLine("Connexion à MongoDB...");
        List<string> fieldsMDB = new List<string> { "IdPartie", "Pseudo","TypeCoup" };
        MongoDatabase.GetAllDataFromCollection("Gr3_Action", fieldsMDB);
        */
    }
}