using LibreCards.Core;
using LibreCards.Core.Entities;
using Moq;
using Xunit;

namespace LibreCards.Tests
{
    public class CardRepositoryTests
    {
        private readonly Mock<IDataStorage> _dataStorageMock;
        private readonly ICardRepository _cardRepository;

        public CardRepositoryTests()
        {
            _dataStorageMock = new Mock<IDataStorage>();
            _cardRepository = new CardRepository(_dataStorageMock.Object);
        }

        [Fact]
        public void CardRepository_DrawOneCard()
        {
            ArrangeStorage(new[] { new Card { Id = 1, Text = "CardText" } });

            var cards = _cardRepository.DrawCards();

            Assert.Single(cards);
        }

        [Fact]
        public void CardRepository_DrawManyCards()
        {
            ArrangeStorage(4);

            var cards = _cardRepository.DrawCards(2).ToList();
            cards.AddRange(_cardRepository.DrawCards(2));

            Assert.Equal(4, cards.Count);
        }

        [Fact]
        public void CardRepository_DrawTemplate()
        {
            var expected = new Template(Template.BlankPlaceholder);
            _dataStorageMock.Setup(ds => ds.DefaultTemplates).Returns(new[] { expected });

            var actual = _cardRepository.DrawTemplate();

            Assert.NotNull(actual);
            Assert.Equal(expected.Content, actual.Content);
            Assert.Equal(expected.BlankCount, actual.BlankCount);
        }

        private void ArrangeStorage(IEnumerable<Card> cards)
            => _dataStorageMock.Setup(ds => ds.DefaultCards).Returns(cards);

        private void ArrangeStorage(int count)
            => _dataStorageMock.Setup(ds => ds.DefaultCards).Returns(Enumerable.Range(1, count).Select(i => new Card { Id = i, Text = "CardText" }));
    }
}
