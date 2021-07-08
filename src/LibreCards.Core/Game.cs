using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class Game : IGame
    {
        public Game(int minimumPlayerCount)
        {
            MinimumPlayerCount = minimumPlayerCount;
            MaximumPlayerCount = 10;
        }

        public GameState State { get; private set; }

        public int MinimumPlayerCount { get; private set; }

        public int MaximumPlayerCount { get; private set; }

        public int PlayerCount => Players.Count;

        public ICollection<Player> Players { get; set; } = new List<Player>();

        public void AddPlayer(Player player)
        {
            if(State == GameState.InProgress)
            {
                throw new InvalidOperationException();
            }

            Players.Add(player);
        }

        public IEnumerable<Player> GetPlayers() => Players;

        public void RemovePlayer(Guid id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);

            if (player is null)
                return;

            Players.Remove(player);

            if(PlayerCount < MaximumPlayerCount)
                State = GameState.Waiting;
        }

        public void SetMaxPlayerCount(int maxPlayerCount)
        {
            if(State == GameState.InProgress)
                throw new InvalidOperationException();

            if(maxPlayerCount == 0 || maxPlayerCount < MinimumPlayerCount)
                throw new ArgumentOutOfRangeException(nameof(maxPlayerCount));

            MaximumPlayerCount = maxPlayerCount;
        }

        public void StartGame()
        {
            if (State == GameState.InProgress)
                return;

            if(PlayerCount < MinimumPlayerCount)
                throw new InvalidOperationException("Not enough players.");

            State = GameState.InProgress;
        }
    }
}
