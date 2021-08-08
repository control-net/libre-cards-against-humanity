using System.Linq;
using System.Collections.Generic;
using LibreCards.Core;
using LibreCards.Core.Entities;

namespace LibreCards.Tests
{
    public class MockCardRepository : ICardRepository
    {
        public IEnumerable<Card> DrawCards(int count = 1)
            => Enumerable.Range(0, count).Select(n => new Card());
    }
}
