using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        void PlayCards(Guid playerId, IEnumerable<int> cardIds);
        void JudgeCard(Guid playerId, int responseId);
        IEnumerable<Response> PlayerResponses { get; }
        bool GetPlayerVoted(Guid id);
        Task ImportCardSetFromUrl(string url);
    }
}
