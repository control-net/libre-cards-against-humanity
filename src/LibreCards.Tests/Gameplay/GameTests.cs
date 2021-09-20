using LibreCards.Core;
using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using Moq;
using Xunit;

namespace LibreCards.Tests.Gameplay;

public class GameTests
{
    private readonly Mock<ICardRepository> _cardRepoMock;
    private readonly Mock<IGameStatus> _gameStatusMock;
    private readonly Mock<ILobby> _lobbyMock;

    private readonly IGame _game;

    public GameTests()
    {
        _cardRepoMock = new Mock<ICardRepository>();
        _gameStatusMock = new Mock<IGameStatus>();
        _lobbyMock = new Mock<ILobby>();

        _game = new Game(_gameStatusMock.Object, _cardRepoMock.Object, _lobbyMock.Object);
    }

    [Fact]
    public void StartGame_NotEnoughPlayers_ShouldThrow()
    {
        _gameStatusMock.Setup(s => s.Current).Returns(GameState.Waiting);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(false);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame());
    }

    [Theory]
    [InlineData(GameState.Playing)]
    [InlineData(GameState.Judging)]
    public void StartGame_InProgress_ShouldThrow(GameState state)
    {
        _gameStatusMock.Setup(s => s.Current).Returns(state);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame());
    }

    [Fact]
    public void StartGame_PlayerShouldGetCards()
    {
        const int expectedCount = 8;

        var player = new Player(Guid.NewGuid());
        _lobbyMock.Setup(l => l.Players).Returns(new[] { player });
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _cardRepoMock.Setup(r => r.DrawCards(8)).Returns(new Card[expectedCount]);

        _game.StartGame();

        Assert.Equal(expectedCount, player.Cards.Count);
    }

    [Fact]
    public void StartGame_ShouldPresentTemplateCard()
    {
        const string ExpectedTemplate = "Test <BLANK> template!";
        
        _gameStatusMock.Setup(s => s.Current).Returns(GameState.Waiting);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { new Player(Guid.NewGuid()), new Player(Guid.NewGuid()), new Player(Guid.NewGuid()) });
        _cardRepoMock.Setup(r => r.DrawTemplate()).Returns(new Template(ExpectedTemplate));

        _game.StartGame();

        Assert.Equal(ExpectedTemplate, _game.TemplateCard.Content);
    }
}
