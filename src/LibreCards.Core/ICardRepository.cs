using System.Collections.Generic;
using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public interface ICardRepository
    {
        IEnumerable<Card> DrawCards(int count = 1);
        Template DrawTemplate();
    }
}
