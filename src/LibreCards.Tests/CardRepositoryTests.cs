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
        public void CardRepository_DrawOneCard()
        {
            var cards = _cardRepository.DrawCards();
            Assert.Single(cards);
        }
        [Fact]
        public void CardRepository_DrawManyCards()
        {
            var cards = _cardRepository.DrawCards(2).ToList();
            cards.AddRange(_cardRepository.DrawCards(2));
            Assert.Equal(4, cards.Count());
            Assert.Equal(cards.Count(), cards.Distinct().Count());
        }
        [Fact]
        public void CardRepository_DrawCardsOverLimit()
        {
            Assert.Throws<IndexOutOfRangeException>(() => _cardRepository.DrawCards(short.MaxValue));
        }
    }
}
