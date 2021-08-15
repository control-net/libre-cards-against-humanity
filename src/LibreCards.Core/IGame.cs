using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface IGame
    {
        void StartGame();
        void AddPlayer(Player player);
        void RemovePlayer(Guid id);
        Player GetPlayer(Guid id);
        int PlayerCount { get; }
        IEnumerable<Player> GetPlayers();
        int MinimumPlayerCount { get; }
        int MaximumPlayerCount { get; }
        void SetMaxPlayerCount(int maxPlayerCount);
    }
}
