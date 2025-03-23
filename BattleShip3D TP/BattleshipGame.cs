using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Diagnostics;
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
        private int actionCount = 0;

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
            Console.WriteLine("You can pause the game at any time by writing P.");
            Console.WriteLine("The game end when there is no more enemy ships, or the time is over. Good luck space fighter !");
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

                actionCount++;

                return (action, x, y, z);
            }
            Console.WriteLine("Invalid input. Expected format: 'TIR x,y,z' or 'MOV x,y,z'");
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
                            Console.WriteLine($"Hit on ship {ship.id_ship} at {position} !");
                            hit = true;

                            UpdateCellDamageInMongo(id_partie, ship.id_ship, position);

                            UpdateAction(position, action, 1);

                            break;
                        }
                    }
                    if (hit) break;
                }

                if (!hit)
                {
                    Console.WriteLine("Missed shot !");
                    UpdateAction(position, action, 0);
                }
            }
            else if (action == "MOV")
            {
                playerPosition = position;
                Console.WriteLine($"Player moved to {position}");
                UpdateAction(position, action, 0);
            }
            else
            {
                Console.WriteLine("Unknown action.");
            }
        }
        
        public void GameLoop()
        {
            PrintRules();
            enemyShips = GetEnemiesShip(id_partie);
            playerPosition = Vector3.Zero;
            string[] result = MySQLDatabase.CustomQuery($"SELECT Temps_limite FROM Gr3_Partie WHERE Id_Partie = {id_partie};");
            int timeLimit = (result != null && result.Length > 0 && int.TryParse(result[0], out int parsedTime)) ? parsedTime : 60;
            bool game_in_progress = true;
            string command;
    
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
    
            long pauseTime = 0;

            while (game_in_progress)
            {
                int elapsedSeconds = (int)((stopwatch.ElapsedMilliseconds - pauseTime) / 1000);
                int remainingTime = timeLimit - elapsedSeconds;

                if (remainingTime <= 0)
                {
                    Console.WriteLine("Time's up! Game over.");
                    game_in_progress = false;
                    break;
                }

                Console.Write($"Enter your command ({remainingTime} seconds left) : ");
                command = Console.ReadLine();

                if (command == "P")
                {
                    PauseGame(id_partie, stopwatch, ref pauseTime, remainingTime);
                    continue;
                }
                else
                {
                    ParsePlayerInput(command, id_partie);
                }
            }
            stopwatch.Stop();
            string updateEtatPartie = $"UPDATE Gr3_Partie SET Temps_limite = \"0\", Etat_Partie = \"FIN\" WHERE Id_Partie = {id_partie};";
            Console.WriteLine(MySQLDatabase.ExecuteNonQuery(updateEtatPartie) ? "" : "Error while ending the game");
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

                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("Id_Partie", idPartie),
                    Builders<BsonDocument>.Filter.Eq("Id_Vaisseau", shipId)
                );

                var arrayFilter = Builders<BsonDocument>.Filter.Eq("Coordonnees.pos", new BsonArray { position.X, position.Y, position.Z });

                var combinedFilter = Builders<BsonDocument>.Filter.And(filter, arrayFilter);

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

        void UpdateAction(Vector3 position, string typeCoup, int? resultat = null)
        {
            try
            {
                var collection = MongoDatabase.GetCollection("Gr3_Action");

                var newAction = new BsonDocument
                {
                    { "Pseudo", pseudo },
                    { "IdPartie", id_partie },
                    { "NumeroCoup", actionCount },
                    { "TypeCoup", typeCoup },
                    { "Position", new BsonDocument
                        {
                            { "x", position.X },
                            { "y", position.Y },
                            { "z", position.Z }
                        }
                    }
                };

                if (typeCoup == "TIR" && resultat.HasValue)
                {
                    newAction.Add("Resultat", resultat.Value);
                }

                collection.InsertOne(newAction);
                Console.WriteLine("Action insérée dans la base de donnée !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour : {ex.Message}");
            }
        }

        public void PauseGame(int id_partie, Stopwatch stopwatch, ref long pauseTime, int remainingTime)
        {
            stopwatch.Stop();
            long pauseStart = stopwatch.ElapsedMilliseconds;

            string updateQuery = $"UPDATE Gr3_Partie SET Temps_limite = \"{remainingTime}\", Etat_Partie = \"PAU\" WHERE Id_Partie = {id_partie};";
            Console.WriteLine(MySQLDatabase.ExecuteNonQuery(updateQuery) ? "Game paused." : "Error while pausing the game");

            Console.WriteLine("Press any key to resume the game.");
            Console.ReadKey();

            updateQuery = $"UPDATE Gr3_Partie SET Etat_Partie = \"ECO\" WHERE Id_Partie = {id_partie};";
            Console.WriteLine(MySQLDatabase.ExecuteNonQuery(updateQuery));

            long pauseEnd = stopwatch.ElapsedMilliseconds;
            pauseTime += (pauseEnd - pauseStart);

            stopwatch.Start();
            Console.WriteLine("Game resumed.");
        }
    }
}