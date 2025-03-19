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

        private List<EnemyShip> enemyShips;
        private Vector3 playerPosition;

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

        
        public (string action, int x, int y, int z)? ParsePlayerInput(string input, int id_partie)
        {
            var match = Regex.Match(input, "^(TIR|MOV) (\\d+),(\\d+),(\\d+)$");
            if (match.Success)
            {
                string action = match.Groups[1].Value;
                int x = int.Parse(match.Groups[2].Value);
                int y = int.Parse(match.Groups[3].Value);
                int z = int.Parse(match.Groups[4].Value);
      
                ActionPlayer(action, x, y, z, id_partie);

                return (action, x, y, z);
            }
            Console.WriteLine("Entrée invalide. Format attendu: 'TIR x,y,z' ou 'MOV x,y,z'");
            return null;
        }

        public void ActionPlayer(string action, int x, int y, int z, int id_partie)
        {
            Vector3 position = new Vector3(x, y, z);

            if (action == "TIR")
            {
                bool hit = false;
                foreach (var ship in enemyShips)
                {
                    for (int i = 0; i < ship.cells.Length; i++)
                    {
                        if (ship.cells[i].coordinate == position)
                        {
                            ship.cells[i].is_damaged = true;
                            Console.WriteLine($"Touché sur le vaisseau ID {ship.id_ship} à {position} !");
                            hit = true;

                            UpdateCellDamageInMongo(id_partie, ship.id_ship, position);

                            break;
                        }
                    }
                    if (hit) break;
                }

                if (!hit)
                {
                    Console.WriteLine("Tir manqué !");
                }
            }
            else if (action == "MOV")
            {
                // Exemple simple : mise à jour de la position joueur
                playerPosition = position;
                Console.WriteLine($"Joueur déplacé en {position}");
            }
            else
            {
                Console.WriteLine("Action inconnue.");
            }
        }
        
        public void GameLoop()
        {
            PrintRules();
            enemyShips = GetEnemiesShip(id_partie);
            playerPosition = Vector3.Zero;

            bool game_in_progess = true;
            string command;
            while (game_in_progess)
            {
                Console.Write("Enter your command : ");
                command = Console.ReadLine();
                //string action, 
                ParsePlayerInput(command, id_partie);
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

        public static void UpdateCellDamageInMongo(int idPartie, int shipId, Vector3 position)
        {
            try
            {
                var collection = MongoDatabase.GetCollection("Gr3_Vaisseaux");

                // On filtre le bon document (le vaisseau)
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("Id_Partie", idPartie),
                    Builders<BsonDocument>.Filter.Eq("Id_Vaisseau", shipId)
                );

                // Filtrer la sous-collection Coordonnees[].pos pour matcher la bonne cellule
                var arrayFilter = Builders<BsonDocument>.Filter.Eq("Coordonnees.pos", new BsonArray { position.X, position.Y, position.Z });

                // Combine les filtres pour cibler la cellule dans le tableau Coordonnees
                var combinedFilter = Builders<BsonDocument>.Filter.And(filter, arrayFilter);

                // Création de l'update : mettre damaged = true
                var update = Builders<BsonDocument>.Update.Set("Coordonnees.$.damaged", true);

                var result = collection.UpdateOne(combinedFilter, update);

                if (result.ModifiedCount > 0)
                {
                    Console.WriteLine("Cellule endommagée mise à jour dans MongoDB.");
                }
                else
                {
                    Console.WriteLine("Aucune cellule trouvée à cette position.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour de la cellule dans MongoDB : {ex.Message}");
            }
        }
    }
}