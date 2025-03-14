using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using MongoDB.Driver;
using MongoDB.Bson;
using BattleShip3D_TP;

class Program
{
    static void Main()
    {
        BattleshipLobby battleshipLobby = new BattleshipLobby();
        MySQLDatabase.Connect();
        battleshipLobby.Login();
        battleshipLobby.Lobby();
        string pseudo = battleshipLobby.Pseudo;
        int id_partie = battleshipLobby.Id_partie;

        BattleshipGame battleshipGame = new BattleshipGame(pseudo, id_partie);
        battleshipGame.GameLoop();

        MySQLDatabase.Disconnect();
    }
}