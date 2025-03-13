using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MongoDB.Driver;
using MongoDB.Bson;

class Program
{
    static void Main()
    {
        BattleShip3D_TP.BattleshipLobby battleshipLobby = new BattleShip3D_TP.BattleshipLobby();
        battleshipLobby.Login();
        battleshipLobby.Lobby();

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