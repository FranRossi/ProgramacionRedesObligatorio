﻿using Common.NetworkUtils;
using Common.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleClient.Menu.Logic.Strategies
{
    public class DeleteOwnedGame : MenuStrategy
    {
        public override void HandleSelectedOption(SocketHandler clientSocket)
        {
            Console.WriteLine("Ingrese nombre del juego de su lista a modificar:");
            string gameName = Console.ReadLine();
            string response = clientSocket.SendMessageAndRecieveResponse(CommandConstants.DeletePublishedGame, gameName);
            Console.WriteLine(response);
            bool acceptedResponses = response == ResponseConstants.DeleteGameSuccess;
            acceptedResponses |= response == ResponseConstants.InvalidGameError;
            acceptedResponses |= response == ResponseConstants.InvalidUsernameError;
            if (acceptedResponses)
                _menuHandler.LoadLoggedUserMenu(clientSocket);
            else
                _menuHandler.LoadMainMenu(clientSocket);
        }
    }
}
