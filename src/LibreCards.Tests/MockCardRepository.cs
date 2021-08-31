using LibreCards.Core;
using LibreCards.Core.Entities;

namespace LibreCards.Tests
{
    public class MockCardRepository : ICardRepository
    {
        public IEnumerable<Card> ReturnedCards { get; set; } = new[] { new Card() };

        public IEnumerable<Card> DrawCards(int count = 1) => ReturnedCards;

        public Template ReturnedTemplate { get; set; }

        public Template DrawTemplate() => ReturnedTemplate;
    }
}
