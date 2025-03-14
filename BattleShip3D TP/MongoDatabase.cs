using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleShip3D_TP
{
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

        /*
        public static List<BsonDocument> QueryCollection(string collectionName, string queryJson)
        {
            try
            {
                var collection = GetCollection(collectionName);
                var filter = BsonDocument.Parse(queryJson);
                var documents = collection.Find(filter).ToList();

                return documents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de requête MongoDB : {ex.Message}");
                return new List<BsonDocument>();
            }
        }
        */

        // A TESTER
        
        public static List<BsonDocument> ExecuteMongoCommand(string collectionName, string operation, string jsonContent)
        {
            try
            {
                var collection = GetCollection(collectionName);

                if (operation == "insertOne")
                {
                    var document = BsonDocument.Parse(jsonContent);
                    collection.InsertOne(document);
                    Console.WriteLine("Insertion réussie.");
                    return new List<BsonDocument> { document };
                }
                else if (operation == "find")
                {
                    var filter = BsonDocument.Parse(jsonContent);
                    var documents = collection.Find(filter).ToList();

                    Console.WriteLine($"Documents trouvés dans {collectionName}:");
                    foreach (var doc in documents)
                    {
                        Console.WriteLine(doc.ToJson());
                    }
                    return documents;
                }
                else
                {
                    Console.WriteLine("Opération MongoDB non supportée. Utilise 'insertOne' ou 'find'.");
                    return new List<BsonDocument>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'exécution de la commande MongoDB : {ex.Message}");
                return new List<BsonDocument>();
            }
        }
    }
}
