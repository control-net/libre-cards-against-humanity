using LibreCards.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LibreCards.Tests
{
    public class CardRepositoryTests
    {
        private readonly ICardRepository _cardRepository;

        public CardRepositoryTests()
        {
            _cardRepository = new CardRepository();
        }

        [Fact]
        public void CardRepository_GetOneCard()
        {
            var cards = _cardRepository.DrawCards();
            Assert.Single(cards);
        }
        [Fact]
        public void CardRepository_DrawManyCards()
        {
            var cards = _cardRepository.DrawCards(2);
            Assert.Equal(2, cards.Count());
        }
        [Fact]
        public void CardRepository_DrawCardsOverLimit()
        {
            // Why does this not throw? It escapes return GetResponseCards()
            Assert.Throws<IndexOutOfRangeException>(() => _cardRepository.DrawCards(int.MaxValue));
        }
    }
}
