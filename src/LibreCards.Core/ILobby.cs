using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface ILobby
    {
        bool HasEnoughPlayers { get; }
        void AddPlayer(Player player);
        void RemovePlayer(Guid id);
        Player GetPlayer(Guid id);
        int PlayerCount { get; }
        IEnumerable<Player> Players();
        int MinimumPlayerCount { get; }
        int MaximumPlayerCount { get; }
        void SetMaxPlayerCount(int maxPlayerCount);
    }
}
