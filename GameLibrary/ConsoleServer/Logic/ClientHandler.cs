﻿using Common.NetworkUtils;
using Common.Protocol;
using ConsoleServer.BussinessLogic;
using ConsoleServer.Utils.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ConsoleServer.Logic
{
    public static class ClientHandler
    {
        private static GameController _gameController;
        private static UserController _userController;

        private static Dictionary<SocketHandler, string> loggedClients;
        public static void HandleClient(SocketHandler clientSocketHandler)
        {
            _gameController = GameController.GetInstance();
            _userController = UserController.GetInstance();
            loggedClients = new Dictionary<SocketHandler, string>();
            bool isSocketActive = true;
            while (!ServerSocketHandler.exit && isSocketActive)
            {
                try
                {
                    Header header = clientSocketHandler.ReceiveHeader();
                    switch (header.ICommand)
                    {
                        case CommandConstants.Login:
                            HandleLogin(header, clientSocketHandler);
                            break;
                        case CommandConstants.Logout:
                            HandleLogout(clientSocketHandler);
                            break;
                        case CommandConstants.ListGames:
                            HandleListGames(clientSocketHandler);
                            break;
                        case CommandConstants.BuyGame:
                            HandleBuyGame(header, clientSocketHandler);
                            break;
                        case CommandConstants.UploadGame:
                            HandleUploadGame(header, clientSocketHandler);
                            break;
                        case 0:
                            isSocketActive = false;
                            loggedClients.Remove(clientSocketHandler);
                            clientSocketHandler.ShutdownSocket();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server is closing, will not process more data -> Message {e.Message}..");
                }
            }
        }

        private static void HandleUploadGame(Header header, SocketHandler clientSocketHandler)
        {
            clientSocketHandler.ReceiveImage(header.IDataLength);
            Console.WriteLine("Server says image arraived");
        }

        private static void HandleLogin(Header header, SocketHandler clientSocketHandler)
        {
            string userName = clientSocketHandler.ReceiveString(header.IDataLength); //Podriamos hacer un metodo que haga todo esto de una
            string responseMessageResult;
            if (loggedClients.ContainsValue(userName))
            {
                responseMessageResult = ResponseConstants.LoginErrorAlreadyLogged;
            }
            else
            {
                if (!loggedClients.ContainsKey(clientSocketHandler))
                {
                    loggedClients.Add(clientSocketHandler, userName);
                    _userController.TryAddUser(userName);
                    responseMessageResult = ResponseConstants.LoginSuccess;
                }
                else
                {
                    responseMessageResult = ResponseConstants.LoginErrorSocketAlreadyInUse;
                }
            }
            clientSocketHandler.SendMessage(HeaderConstants.Response, CommandConstants.Login, responseMessageResult);
        }

        private static void HandleLogout(SocketHandler clientSocketHandler)
        {
            if (loggedClients.ContainsKey(clientSocketHandler))
                loggedClients.Remove(clientSocketHandler);
            string responseMessageResult = ResponseConstants.LogoutSuccess;
            clientSocketHandler.SendMessage(HeaderConstants.Response, CommandConstants.Logout, responseMessageResult);
        }

        private static void HandleListGames(SocketHandler clientSocketHandler)
        {
            string gameList = _gameController.GetGames();
            string responseMessage = gameList;
            clientSocketHandler.SendMessage(HeaderConstants.Response, CommandConstants.ListGames, responseMessage);
        }

        private static void HandleBuyGame(Header header, SocketHandler clientSocketHandler)
        {
            string gameName = clientSocketHandler.ReceiveString(header.IDataLength);
            string username = "";
            string responseMessageResult = "";
            if (loggedClients.ContainsKey(clientSocketHandler))
            {
                username = loggedClients[clientSocketHandler];
                try
                {
                    _userController.BuyGame(username, gameName);
                    responseMessageResult = ResponseConstants.BuyGameSuccess;
                }
                catch (InvalidUsernameException e)
                {
                    responseMessageResult = ResponseConstants.InvalidUsernameError;
                }
                catch (InvalidGameException e)
                {
                    responseMessageResult = ResponseConstants.InvalidGameError;
                }
            }
            else
            {
                responseMessageResult = ResponseConstants.AuthenticationError;
            }
            clientSocketHandler.SendMessage(HeaderConstants.Response, CommandConstants.BuyGame, responseMessageResult);
        }
    }
}
