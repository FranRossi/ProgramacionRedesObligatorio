﻿using Common.FileUtils;
using Common.FileUtils.Interfaces;
using Common.NetworkUtils;
using Common.NetworkUtils.Interfaces;
using Common.Protocol;
using ConsoleServer.Domain;
using ConsoleServer.Utils.CustomExceptions;

namespace ConsoleServer.Logic.Commands.Strategies
{
    public class ModifyGamePublished : CommandStrategy
    {

        public override void HandleRequest(Header header, ISocketHandler clientSocketHandler)
        {
            string responseMessage;
            if (_clientHandler.IsSocketInUse(clientSocketHandler))
            {
                int firstElement = 0;
                int secondElement = 1;
                int thirdElement = 2;
                int fouthElement = 3;
                string userName = _clientHandler.GetUsername(clientSocketHandler);
                string rawData = clientSocketHandler.ReceiveString(header.IDataLength).Result;
                string[] gameData = rawData.Split('%');
                string oldGameName = gameData[firstElement];
                string newGameName = gameData[secondElement];
                string newGamegenre = gameData[thirdElement];
                string newGameSynopsis = gameData[fouthElement];
                string gameName = (newGameName == "") ? oldGameName : newGameName;
                string pathToImage = UpdateImage(clientSocketHandler, gameName);

                User user = _userController.GetUser(userName);
                Game newGame = new Game
                {
                    Name = newGameName,
                    Genre = newGamegenre,
                    Synopsis = newGameSynopsis,
                    OwnerUser = user,
                    PathToPhoto = pathToImage
                };
                responseMessage = ModifyGame(newGame, user, oldGameName);
            }
            else
                responseMessage = ResponseConstants.AuthenticationError;
            clientSocketHandler.SendMessage(HeaderConstants.Response, CommandConstants.ListOwnedGames, responseMessage);
        }

        private string ModifyGame(Game newGame, User user, string oldGameName)
        {
            string responseMessage;
            try
            {
                Game gameToModify = _gameController.GetCertainGamePublishedByUser(user, oldGameName);
                if (gameToModify != null)
                {
                    DeletePreviousImage(gameToModify, newGame);
                    //_userController.ModifyGameFromAllUser(gameToModify, newGame);
                    _gameController.ModifyGame(gameToModify, newGame);
                    responseMessage = ResponseConstants.ModifyPublishedGameSuccess;
                }
                else
                {
                    responseMessage = ResponseConstants.UnauthorizedGame;
                }
            }
            catch (InvalidUsernameException)
            {
                responseMessage = ResponseConstants.InvalidUsernameError;
            }
            catch (InvalidGameException)
            {
                responseMessage = ResponseConstants.InvalidGameError;
            }
            return responseMessage;
        }

        private string UpdateImage(ISocketHandler clientSocketHandler, string gameName)
        {
            int imageDataLength = SpecificationHelper.GetImageDataLength();
            string rawImageData = clientSocketHandler.ReceiveString(imageDataLength).Result;
            string emptyImageData = 0.ToString("D" + imageDataLength);
            string pathToImageGame = "";

            if (rawImageData != emptyImageData)
            {
                ISettingsManager SettingsMgr = new SettingsManager();
                string pathToImageFolder = SettingsMgr.ReadSetting(ServerConfig.ServerPathToImageFolder);
                pathToImageGame = clientSocketHandler.ReceiveImage(rawImageData, pathToImageFolder, gameName).Result;
            }
            return pathToImageGame;
        }

        private void DeletePreviousImage(Game gameToModify, Game newGame)
        {
            IFileHandler fileStreamHandler = new FileHandler();
            string previousImagePath = gameToModify.PathToPhoto;
            if (newGame.PathToPhoto != "")
                fileStreamHandler.DeleteFile(previousImagePath);
        }
    }
}
