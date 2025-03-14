using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Common;
using System.Numerics;
using System.Runtime.InteropServices;

namespace BattleShip3D_TP
{
    class BattleshipLobby
    {
        // Keys to transfer to MongoDb
        private string pseudo = "";
        public string Pseudo => pseudo;
        private int id_partie;
        public int Id_partie => id_partie;

        // Game Variables
        private string dimension = "";
        private string time_limit = "";
        private string player_number = "";

        bool CreateAccount()
        {
            bool nicknameTaken = false;
            while (!nicknameTaken)
            {
                Console.Write("Pseudo: ");
                pseudo = Console.ReadLine();
                string[] result = MySQLDatabase.CustomQuery($"SELECT Pseudo FROM Gr3_Joueur WHERE Pseudo = \"{pseudo}\";");
                if (result != null)
                {
                    Console.WriteLine("This username is already taken!");
                }
                // Vérifier si le pseudo existe déjà
                else
                {
                    nicknameTaken = true;
                    Console.Write("Password: ");
                    string mdp = Console.ReadLine();

                    Console.Write("Lastname: ");
                    string nom = Console.ReadLine();

                    Console.Write("Firstname: ");
                    string prenom = Console.ReadLine();

                    Console.Write("Age: ");
                    int age;
                    while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
                    {
                        Console.WriteLine("Invalid age, try again.");
                    }

                    Console.Write("Mail: ");
                    string mail = Console.ReadLine();

                    // Insérer le joueur
                    string insertQuery = $"INSERT INTO Gr3_Joueur (Pseudo, Mot_de_passe, Nom, Prenom, Age, Mail) " +
                                         $"VALUES (\"{pseudo}\", \"{mdp}\", \"{nom}\", \"{prenom}\", {age}, \"{mail}\");";

                    if (MySQLDatabase.ExecuteNonQuery(insertQuery))
                    {
                        Console.WriteLine("Player successfully added!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Failed to add the player.");
                        // return;
                    }
                }
            }
            return false;
        }

        public void Login()
        {
            Console.WriteLine("Welcome to BattleShip 3D !");
            bool player_found = false;
            while (!player_found)
            {
                Console.WriteLine("Enter your nickname :");
                pseudo = Console.ReadLine();
                string[] result = MySQLDatabase.CustomQuery($"SELECT Pseudo FROM Gr3_Joueur WHERE Pseudo = \"{pseudo}\";");
                if (result != null)
                {
                    player_found = true;
                }
                else
                {
                    Console.WriteLine("Player Not Found, do you want to create it ? (yes/no)");
                    if (Console.ReadLine() == "yes")
                    {
                        player_found = CreateAccount();
                    }
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

        public void Lobby()
        {
            string input;
            bool is_player_in_a_game = false;
            while (!is_player_in_a_game)
            {
                Console.WriteLine("Would you like to : ");
                Console.WriteLine("1 _ Create a game ?");
                Console.WriteLine("2 _ Join a game ?");

                input = Console.ReadLine();
                int player_choice = 0;
                int.TryParse(input, out player_choice);
                if (player_choice == 1)
                {
                    is_player_in_a_game = CreateGame();
                }
                else if (player_choice == 2)
                {
                    is_player_in_a_game = JoinGame();
                }
                else
                {
                    Console.WriteLine("Command not valid, please try again.");
                }
            }
        }

        private EnemyShip[] CreateEnemyShips()
        {
            List<EnemyShip> ships = new List<EnemyShip>();
            bool isInputValid = false;

            Console.WriteLine("Enter the list of enemy ships (size,type,position).");
            Console.WriteLine("Valid types: segment, carre, cube.");
            Console.WriteLine("Example input: 3, segment, 0,0,0");
            Console.WriteLine("Type 'done' when you are finished.");

            while (!isInputValid)
            {
                Console.Write("Enter a ship (or 'done' to finish): ");
                string input = Console.ReadLine().Trim();

                if (input.ToLower() == "done")
                {
                    if (ships.Count > 0)
                    {
                        isInputValid = true;
                    }
                    else
                    {
                        Console.WriteLine("You must enter at least one ship before finishing.");
                    }
                    continue;
                }

                string[] parts = input.Split(',');
                if (parts.Length == 5)
                {
                    string type_string = parts[1].Trim().ToLower();
                    bool isSizeValid = int.TryParse(parts[0].Trim(), out int size);
                    bool isXValid = int.TryParse(parts[2].Trim(), out int x);
                    bool isYValid = int.TryParse(parts[3].Trim(), out int y);
                    bool isZValid = int.TryParse(parts[4].Trim(), out int z);

                    // @TODO RANDOMIZE ENEMY SHIPS POSITION

                    if (EnemyShip.ShipTypeFromString(type_string).HasValue && isSizeValid && isSizeValid && isXValid && isYValid && isZValid)
                    {
                        Vector3 shipPosition = new Vector3(x,y,z);
                        ShipCell shipCell = new ShipCell(shipPosition,false);
                        EnemyShip enyShip = new EnemyShip(ships.Count, EnemyShip.ShipTypeFromString(type_string).Value, size, new ShipCell[] {shipCell});
                        // @TODO DEDUCE OTHER CELLS
                        ships.Add(enyShip);
                    }
                    else
                    {
                        Console.WriteLine("Invalid format. Please enter: (size,type,position) (3, segment, 0,0,0)");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid format. Please enter: (size,type,position) (3, segment, 0,0,0)");
                }
            }

            return ships.ToArray();
        }

        private bool CreateGame()
        {
            bool is_input_valid = false;
            while (!is_input_valid)
            {
                Console.WriteLine("What will be the dimensions of the cube? (between 4 and 100)");
                dimension = Console.ReadLine();
                int player_choice = 0;
                int.TryParse(dimension, out player_choice);
                if (player_choice > 3 && player_choice < 101)
                {
                    is_input_valid = true;
                }
                else
                {
                    Console.WriteLine("Dimensions not valid, please try again.");
                }
            }
            is_input_valid = false;
            while (!is_input_valid)
            {
                Console.WriteLine("What will be the time limit of the game? (between 1 and 10 minutes)");
                time_limit = Console.ReadLine();
                int player_choice = 0;
                int.TryParse(time_limit, out player_choice);
                if (player_choice > 0 && player_choice < 11)
                {
                    is_input_valid = true;
                }
                else
                {
                    Console.WriteLine("Time limit not valid, please try again.");
                }
            }

            EnemyShip[] ships = CreateEnemyShips();
            //@TODO WRITE SHIPS TO MONGO DB
            
            is_input_valid = false;
            while (!is_input_valid)
            {
                Console.WriteLine("How many players will be in the game? (between 2 and 5)");
                player_number = Console.ReadLine();
                int player_choice = 0;
                int.TryParse(player_number, out player_choice);
                if (player_choice > 1 && player_choice < 6)
                {
                    is_input_valid = true;
                }
                else
                {
                    Console.WriteLine("Player amount not valid, please try again.");
                }
            }

            Console.WriteLine("\nList of ships:");
            foreach (var ship in ships)
            {
                Console.WriteLine($"- Type: {ship.type.ToString()}, Size: {ship.size}");
            }

            Console.WriteLine("\n Cube dimensions : " + dimension + "\n Time limit : " + time_limit + "\n Player number : " + player_number);
            id_partie = MySQLDatabase.CreateLobby(dimension, time_limit);
            return true;
        }

        private bool JoinGame()
        {
            bool game_found = false;
            while (!game_found)
            {
                Console.WriteLine("Which Id would you like to join ? :");
                string input = Console.ReadLine();
                string[] result = MySQLDatabase.CustomQuery($"SELECT Id_Partie FROM Gr3_Partie WHERE Id_Partie = {input};");
                if (result != null)
                {
                    int.TryParse(input, out id_partie);
                    return true;
                }
                else
                {
                    Console.WriteLine("The game is not found, please try again.");
                }
            }
            return false;
        }
    }
}