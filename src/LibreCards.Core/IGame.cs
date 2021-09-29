﻿using LibreCards.Core.Entities;
using System;

namespace LibreCards.Core
{
    public interface IGame
    {
        ILobby Lobby { get; }
        void StartGame(Guid playerId);
        Template TemplateCard { get; }
        Guid JudgePlayerId { get; }
        Guid LobbyOwnerId { get; }
        GameState GameState { get; }
    }
}
