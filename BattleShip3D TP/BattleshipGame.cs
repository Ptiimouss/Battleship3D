using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace BattleShip3D_TP
{
    class BattleshipGame
    {
        private string pseudo = "";
        private int id_partie;

        public BattleshipGame(string in_pseudo, int in_id_partie)
        {
            pseudo = in_pseudo;
            id_partie = in_id_partie;
        }

        private void PrintRules()
        {
            Console.WriteLine("Welcome to a game of Battleship 3, here are the rules : ");
            Console.WriteLine("Your goal is to destroy the most enemy ships in the dangerous space of ELYSIUM.");
            Console.WriteLine("You can MOV freely with command MOV X,X,X , with X a coordinate in space.");
            Console.WriteLine("You can TIR freely with command TIR X,X,X , with X a coordinate in space.");
            Console.WriteLine("The game end when there is no more enemy ships, good luck space fighter !");
        }

        /*
        public static (string action, int x, int y, int z)? ParsePlayerInput(string input)
        {
            var match = Regex.Match(input, "^(TIR|MOV) (\\d+),(\\d+),(\\d+)$");
            if (match.Success)
            {
                string action = match.Groups[1].Value;
                int x = int.Parse(match.Groups[2].Value);
                int y = int.Parse(match.Groups[3].Value);
                int z = int.Parse(match.Groups[4].Value);
                return (action, x, y, z);
            }
            Console.WriteLine("Entrée invalide. Format attendu: 'TIR x,y,z' ou 'MOV x,y,z'");
            return null;
        }
        */
        public void GameLoop()
        {
            PrintRules();
            GetEnemiesShip(id_partie);
            bool game_in_progess = true;
            string command;
            while (game_in_progess)
            {
                Console.Write("Enter your command : ");
                command = Console.ReadLine();
                //string action, 
            }
        }

        public static List<EnemyShip> GetEnemiesShip(int idPartie)
        {
            try
            {
                string collectionName = "Gr3_Vaisseaux";
                var collection = MongoDatabase.GetCollection(collectionName);

                var filter = Builders<BsonDocument>.Filter.Eq("Id_Partie", idPartie);

                var documents = collection.Find(filter).ToList();

                List<EnemyShip> enemyShips = new List<EnemyShip>();

                foreach (var doc in documents)
                {
                    int id = doc["Id_Vaisseau"].AsInt32;
                    ShipType type = Enum.TryParse(doc["Type"].AsString, out ShipType parsedType) ? parsedType : ShipType.Segment;
                    int size = doc["Taille"].AsInt32;

                    var coordinatesArray = doc["Coordonnees"].AsBsonArray;
                    ShipCell[] cells = new ShipCell[coordinatesArray.Count];

                    for (int i = 0; i < coordinatesArray.Count; i++)
                    {
                        var coordObject = coordinatesArray[i].AsBsonDocument;
                        var posArray = coordObject["pos"].AsBsonArray; 

                        Vector3 position = new Vector3(
                            (float)posArray[0].AsDouble,
                            (float)posArray[1].AsDouble,
                            (float)posArray[2].AsDouble);

                        bool isDamaged = coordObject["damaged"].AsBoolean;

                        cells[i] = new ShipCell(position, isDamaged);
                    }

                    enemyShips.Add(new EnemyShip(id, type, size, cells));
                }

                return enemyShips;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des vaisseaux ennemis : {ex.Message}");
                return new List<EnemyShip>();
            }
        }

    }
}