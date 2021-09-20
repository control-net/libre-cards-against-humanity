using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class Lobby : ILobby
    {
        private readonly IGameStatus _gameStatus;

        public Lobby(int minimumPlayerCount, IGameStatus gameStatus)
        {
            MinimumPlayerCount = minimumPlayerCount;
            MaximumPlayerCount = 10;

            _gameStatus = gameStatus;
        }

        public int MinimumPlayerCount { get; private set; }

        public int MaximumPlayerCount { get; private set; }

        public int PlayerCount => _players.Count;

        private readonly ICollection<Player> _players = new List<Player>();

        public bool HasEnoughPlayers => PlayerCount >= MinimumPlayerCount;

        public void AddPlayer(Player player)
        {
            if (_gameStatus.Current != GameState.Waiting)
                throw new InvalidOperationException();

            _players.Add(player);
        }

        public IEnumerable<Player> Players => _players;

        public void RemovePlayer(Guid id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);

            if (player is null)
                return;

            _players.Remove(player);

            if (PlayerCount < MaximumPlayerCount)
                _gameStatus.SwitchToWaiting();
        }

        public void SetMaxPlayerCount(int maxPlayerCount)
        {
            if (_gameStatus.Current != GameState.Waiting)
                throw new InvalidOperationException();

            if (maxPlayerCount == 0 || maxPlayerCount < MinimumPlayerCount)
                throw new ArgumentOutOfRangeException(nameof(maxPlayerCount));

            MaximumPlayerCount = maxPlayerCount;
        }

        public Player GetPlayer(Guid id)
        {
            var player = _players.FirstOrDefault(p => p.Id == id);

            if (player is null)
                throw new InvalidOperationException($"No player with id '{id}' found.");

            return player;
        }
    }
}
