using LibreCards.Core;
using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using Moq;
using Xunit;

namespace LibreCards.Tests.Gameplay;

public class CardStateTests
{
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly ICardState _cardState;

    private const string TestTemplate = "<BLANK>";

    public CardStateTests()
    {
        _cardRepositoryMock = new Mock<ICardRepository>();
        _cardState = new CardState(_cardRepositoryMock.Object);
    }

    [Fact]
    public void CurrentTemplateCard_ShouldStartNull()
    {
        Assert.Null(_cardState.CurrentTemplateCard);
    }

    [Fact]
    public void DrawTemplateCard_ShouldDrawTemplateCard()
    {
        _cardRepositoryMock.Setup(r => r.DrawTemplate()).Returns(new Template(TestTemplate));
        _cardState.DrawTemplateCard();

        Assert.Equal(TestTemplate, _cardState.CurrentTemplateCard.Content);
    }

    [Fact]
    public void RefillPlayerCards_Null_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _cardState.RefillPlayerCards(null));
    }

    [Fact]
    public void RefillPlayerCards_EmptyHand_ShouldAddEightCards()
    {
        _cardRepositoryMock.Setup(r => r.DrawCards(8)).Returns(new Card[8]);
        var player = new Player(Guid.NewGuid());

        _cardState.RefillPlayerCards(new[] { player });

        Assert.Equal(8, player.Cards.Count);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(6)]
    public void RefillPlayerCards_NonEmptyHand_ShouldFillToEightCards(int initialCount)
    {
        _cardRepositoryMock.Setup(r => r.DrawCards(8 - initialCount)).Returns(new Card[8 - initialCount]);
        var player = new Player(Guid.NewGuid())
        {
            Cards = new Card[initialCount]
        };

        _cardState.RefillPlayerCards(new[] { player });

        Assert.Equal(8, player.Cards.Count);
    }

    [Fact]
    public void RefillPlayerCards_FullHand_ShouldNotAddCards()
    {
        _cardRepositoryMock.Setup(r => r.DrawCards(0)).Returns(Array.Empty<Card>());
        var player = new Player(Guid.NewGuid())
        {
            Cards = new Card[8]
        };

        _cardState.RefillPlayerCards(new[] { player });

        Assert.Equal(8, player.Cards.Count);
    }

    [Fact]
    public void PlayerResponses_ShouldStartEmpty()
    {
        Assert.Empty(_cardState.PlayerResponses);
    }

    [Fact]
    public void AddPlayerResponse_ShouldAdd()
    {
        var playerId = Guid.NewGuid();
        var cards = new[]
        {
            new Card { Id = 1 },
            new Card { Id = 2 }
        };

        _cardState.AddPlayerResponse(playerId, cards);

        Assert.Single(_cardState.PlayerResponses);
        Assert.Equal(0, _cardState.PlayerResponses.First().Id);
        Assert.Equal(1, _cardState.PlayerResponses.First().Cards.ElementAt(0).Id);
        Assert.Equal(2, _cardState.PlayerResponses.First().Cards.ElementAt(1).Id);
    }

    [Fact]
    public void AddPlayerResponse_AddTwice_ShouldThrow()
    {
        var playerId = Guid.NewGuid();
        var cards = new[]
        {
            new Card { Id = 1 },
            new Card { Id = 2 }
        };

        _cardState.AddPlayerResponse(playerId, cards);
        Assert.Throws<InvalidOperationException>(() => _cardState.AddPlayerResponse(playerId, cards));
        Assert.Single(_cardState.PlayerResponses);
    }

    [Fact]
    public void GetVotingCompleted_NoResponses_ShouldReturnFalse()
    {
        Assert.False(_cardState.GetVotingCompleted(Array.Empty<Player>()));
    }

    [Fact]
    public void GetVotingCompleted_NotEnoughResponses_ShouldReturnFalse()
    {
        var players = new[]
        {
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid())
        };

        _cardState.AddPlayerResponse(players[0].Id, Array.Empty<Card>());

        Assert.False(_cardState.GetVotingCompleted(players));
    }

    [Fact]
    public void GetVotingCompleted_EnoughResponses_ShouldReturnTrue()
    {
        var players = new[]
        {
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid())
        };

        _cardState.AddPlayerResponse(players[0].Id, Array.Empty<Card>());
        _cardState.AddPlayerResponse(players[1].Id, Array.Empty<Card>());

        Assert.True(_cardState.GetVotingCompleted(players));
    }

    [Fact]
    public void ClearResponses_ShouldClearVotes()
    {
        var players = new[]
        {
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid())
        };

        _cardState.AddPlayerResponse(players[0].Id, Array.Empty<Card>());
        _cardState.AddPlayerResponse(players[1].Id, Array.Empty<Card>());

        _cardState.ClearResponses();
        Assert.False(_cardState.GetVotingCompleted(players));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void PickBestResponse_InvalidId_ShouldThrow(int id)
    {
        var players = new[]
        {
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid())
        };

        _cardState.AddPlayerResponse(players[0].Id, Array.Empty<Card>());
        _cardState.AddPlayerResponse(players[1].Id, Array.Empty<Card>());

        Assert.Throws<IndexOutOfRangeException>(() => _cardState.PickBestResponse(id));
    }

    [Fact]
    public void PickBestResponse_ShouldReturnTheCorrectPlayer()
    {
        var players = new[]
{
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid()),
            new Player(Guid.NewGuid())
        };

        _cardState.AddPlayerResponse(players[0].Id, Array.Empty<Card>());
        _cardState.AddPlayerResponse(players[1].Id, Array.Empty<Card>());

        var actual = _cardState.PickBestResponse(1);

        Assert.Equal(players[1].Id, actual);
    }
}
