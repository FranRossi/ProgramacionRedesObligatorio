﻿using System.Collections.Generic;
using System.Linq;
using CommonLog;

namespace ServerLogs.LogsStorage.GameLogs
{
    public class Games
    {
        private static readonly object _padlock = new object();
        private static Games _instance = null;
        private readonly List<GameLogModel> _logs;

        private Games()
        {
            _logs = new List<GameLogModel>();
        }

        public List<GameLogModel> GetLogs()
        {
            return _logs;
        }

        public static Games Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new Games();
                    }
                    return _instance;
                }
            }
        }

        public GameLogModel GetLog(int id)
        {
            lock (_padlock)
                if (_logs != null)
                    return _logs.FirstOrDefault(g => g.Id == id);
            return null;
        }
        
        public void AddGameLog(GameLogModel gameToAdd)
        {
            lock (_padlock)
            {
                _logs.Add(gameToAdd);
            }
        }

        public GameLogModel DeleteLog(int id)
        {
            lock (_padlock)
                if (_logs != null)
                {
                    GameLogModel log = _logs.FirstOrDefault(g => g.Id == id);
                    _logs.Remove(log);
                    return log;
                }
            return null;
        }
    }
}