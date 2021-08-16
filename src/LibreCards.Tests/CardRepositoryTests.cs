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
        private readonly CardRepository _cardRepository;

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
    }
}
