﻿using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using CommonLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerGRPC.Logic.Commands.Strategies
{
    public class ListOwnedGames : CommandStrategy
    {

        public override async Task<GameLogModel> HandleRequest(Header header, INetworkStreamHandler clientNetworkStreamHandler)
        {
            GameLogModel log = new GameLogModel(header.ICommand);
            string emptyString = "";
            string responseMessage;
            if (_clientHandler.IsSocketInUse(clientNetworkStreamHandler))
            {
                string userName = _clientHandler.GetUsername(clientNetworkStreamHandler);
                string gameList = _userController.ListOwnedGameByUser(userName);
                responseMessage = gameList;
                if (gameList == emptyString)
                    responseMessage = ResponseConstants.LibraryError;

            }
            else
                responseMessage = ResponseConstants.AuthenticationError;
            await clientNetworkStreamHandler.SendMessage(HeaderConstants.Response, CommandConstants.ListOwnedGames, responseMessage);
            return log;
        }
    }
}