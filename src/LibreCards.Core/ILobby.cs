using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface ILobby
    {
        int MinimumPlayerCount { get; }
        int MaximumPlayerCount { get; }
        bool HasEnoughPlayers { get; }
        void SetMinimumPlayerCount(int count);
        void SetMaximumPlayerCount(int count);
        IReadOnlyCollection<Player> Players { get; }
        void AddPlayer(Player player);
        void RemovePlayer(Guid id);
        Guid OwnerId { get; }
    }
}
