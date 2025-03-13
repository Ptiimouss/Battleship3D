using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip3D_TP
{
    class BattleshipLobby
    {
        private string pseudo = "";

        public void Login()
        {
            Console.WriteLine("Welcome to BattleShip 3D !");
            Console.WriteLine("Enter your pseudo :");
            bool player_found = false;
            while (!player_found)
            {
                pseudo = Console.ReadLine();
                string[] result = MySQLDatabase.CustomQuery($"SELECT Pseudo FROM Gr3_Joueur WHERE Pseudo = \"{pseudo}\";");
                if (result != null)
                {
                    player_found = true;
                }
                else
                {
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

        private bool CreateGame()
        {
            // @TODO
            Console.WriteLine("Create Game :");
            return false;
        }

        private bool JoinGame()
        {
            // @TODO
            Console.WriteLine("join Game :");
            return false;
        }
    }
}
