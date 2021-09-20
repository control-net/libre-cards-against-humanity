using LibreCards.Core.Entities;
using System.Collections.Generic;

namespace LibreCards.Core.Persistence
{
    public interface ICardRepository
    {
        IEnumerable<Card> DrawCards(int count = 1);
        Template DrawTemplate();
    }
}
