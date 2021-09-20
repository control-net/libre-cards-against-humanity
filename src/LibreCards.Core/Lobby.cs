using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class Lobby : ILobby
    {
        private readonly IGameStatus _gameStatus;

        private readonly List<Player> _players = new List<Player>();

        public Lobby(int minimumPlayerCount, IGameStatus gameStatus)
        {
            MinimumPlayerCount = minimumPlayerCount;
            MaximumPlayerCount = 10;

            _gameStatus = gameStatus;
        }

        public Lobby(IGameStatus gameStatus)
        {
            _gameStatus = gameStatus;

            MinimumPlayerCount = 3;
            MaximumPlayerCount = 10;
        }

        public IReadOnlyCollection<Player> Players => _players.AsReadOnly();

        public int MinimumPlayerCount { get; private set; }

        public int MaximumPlayerCount { get; private set; }

        public bool HasEnoughPlayers => _players.Count >= MinimumPlayerCount;

        public void SetMinimumPlayerCount(int count)
        {
            if (count < 1)
                throw new ArgumentException("Cannot set minimum player count to zero or lower.");

            if (count > MaximumPlayerCount)
                throw new ArgumentException($"Cannot set minimum player count to more than the current maximum of {MaximumPlayerCount}.");

            MinimumPlayerCount = count;
        }

        public void SetMaximumPlayerCount(int count)
        {
            if (count < 1)
                throw new ArgumentException("Cannot set maximum player count to zero or lower.");

            if (count < MinimumPlayerCount)
                throw new ArgumentException($"Cannot set maximum player count to less than the current minimum of {MinimumPlayerCount}.");

            if (count < _players.Count)
                throw new ArgumentException("Cannot set maximum player count to less than the current player count.");

            MaximumPlayerCount = count;
        }

        public void AddPlayer(Player player)
        {
            if (_gameStatus.Current != GameState.Waiting)
                throw new InvalidOperationException("Cannot add a player while the game is running.");

            if (_players.Count == MaximumPlayerCount)
                throw new InvalidOperationException("Cannot add a player, because the lobby is full.");

            if (player is null)
                throw new ArgumentNullException(nameof(player));

            _players.Add(player);
        }

        public void RemovePlayer(Guid id)
        {
            var player = _players.FirstOrDefault(p => p.Id == id);

            if (player is null)
                throw new ArgumentException($"No player with id '{id}' exists.");

            _players.Remove(player);
        }
    }
}
