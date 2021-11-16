﻿using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using CommonLog;
using ServerGRPC.Domain;
using ServerGRPC.Utils.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerGRPC.Logic.Commands.Strategies
{
    public class GetGameDetails : CommandStrategy
    {

        public override async Task<GameLogModel> HandleRequest(Header header, INetworkStreamHandler clientNetworkStreamHandler)
        {
            GameLogModel log = new GameLogModel(header.ICommand);
            string gameName = await clientNetworkStreamHandler.ReceiveString(header.IDataLength);
            string responseMessageResult;
            if (_clientHandler.IsSocketInUse(clientNetworkStreamHandler))
            {
                try
                {
                    Game game = _gameController.GetGame(gameName);
                    responseMessageResult = game.ToString();
                }
                catch (InvalidGameException)
                {
                    responseMessageResult = ResponseConstants.InvalidGameError;
                }
            }
            else
                responseMessageResult = ResponseConstants.AuthenticationError;
            await clientNetworkStreamHandler.SendMessage(HeaderConstants.Response, CommandConstants.GetGameDetails, responseMessageResult);
            return log;
        }
    }
}