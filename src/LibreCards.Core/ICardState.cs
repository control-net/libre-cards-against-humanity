using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface ICardState
    {
        Template CurrentTemplateCard { get; }
        IEnumerable<Response> PlayerResponses { get; }
        void DrawTemplateCard();
        void RefillPlayerCards(IReadOnlyCollection<Player> players);
        void AddPlayerResponse(Guid playerId, IEnumerable<Card> cards);
        bool GetVotingCompleted(IReadOnlyCollection<Player> players);
        void ClearResponses();
        Guid PickBestResponse(int id);
    }
}
