using LibreCards.Core.Entities;
using System;

namespace LibreCards.Core
{
    public interface IGame
    {
        ILobby Lobby { get; }
        void StartGame();
        Template TemplateCard { get; }
        Guid JudgePlayerId { get; }
    }
}
