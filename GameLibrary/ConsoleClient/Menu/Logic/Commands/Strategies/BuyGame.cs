﻿using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using System;

namespace ConsoleClient.Menu.Logic.Commands.Strategies
{
    public class BuyGame : MenuStrategy
    {
        public override string HandleSelectedOption(ISocketHandler clientSocket)
        {
            ListGamesAvailable(clientSocket);
            Console.WriteLine("Por favor ingrese el nombre del juego para comprar:");
            string gameName = Console.ReadLine().ToLower();
            string response = "";
            if (_menuHandler.ValidateNotEmptyFields(gameName))
                response = clientSocket.SendMessageAndRecieveResponse(CommandConstants.BuyGame, gameName);
            return response;
        }

        private void ListGamesAvailable(ISocketHandler clientSocket)
        {
            ListGames listGames = new ListGames();
            listGames.ListGamesAvailable(clientSocket);
        }
    }
}
