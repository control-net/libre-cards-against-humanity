using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibreCards.Core
{
    public class CardRepository : ICardRepository
    {
        public IEnumerable<Card> DrawCards(int count = 1)
        {
            var result = new List<Card> { new Card() { Id = 0 } };
            return result;
        }
    }
}
