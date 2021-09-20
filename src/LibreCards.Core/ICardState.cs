using LibreCards.Core.Entities;
using System.Collections.Generic;

namespace LibreCards.Core
{
    public interface ICardState
    {
        Template CurrentTemplateCard { get; }

        void DrawTemplateCard();
        void RefillPlayerCards(IReadOnlyCollection<Player> players);
    }
}
