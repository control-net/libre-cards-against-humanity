using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface IGame
    {
        GameState State { get; }
        void StartGame();
        void AddPlayer(Player player);
        void RemovePlayer(Guid id);
        int PlayerCount { get; }
        IEnumerable<Player> GetPlayers();
        int MinimumPlayerCount { get; }
        int MaximumPlayerCount { get; }
        void SetMaxPlayerCount(int maxPlayerCount);
    }
}
