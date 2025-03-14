using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleShip3D_TP
{
    internal class BattleshipGame
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
            bool game_in_progess = true;
            string command;
            while (game_in_progess)
            {
                Console.Write("Enter your command : ");
                command = Console.ReadLine();
                //string action, 
            }
        }
    }
}
