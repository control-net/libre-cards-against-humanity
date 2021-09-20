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
    private readonly Mock<IJudgePicker> _judgePickerMock;

    private readonly IGame _game;

    private const int TestCardsCount = 8;
    private const string TestTemplate = "<BLANK>";

    public GameTests()
    {
        _cardRepoMock = new Mock<ICardRepository>();
        _gameStatusMock = new Mock<IGameStatus>();
        _lobbyMock = new Mock<ILobby>();
        _judgePickerMock = new Mock<IJudgePicker>();

        _game = new Game(_gameStatusMock.Object, _cardRepoMock.Object, _lobbyMock.Object, _judgePickerMock.Object);
    }

    [Fact]
    public void StartGame_NotEnoughPlayers_ShouldThrow()
    {
        ArrangeReadyToStartGame();
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(false);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame());
    }

    [Theory]
    [InlineData(GameState.Playing)]
    [InlineData(GameState.Judging)]
    public void StartGame_InProgress_ShouldThrow(GameState state)
    {
        ArrangeReadyToStartGame();
        _gameStatusMock.Setup(s => s.Current).Returns(state);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame());
    }

    [Fact]
    public void StartGame_PlayerShouldGetCards()
    {
        ArrangeReadyToStartGame();
        var player = new Player(Guid.NewGuid());
        _lobbyMock.Setup(l => l.Players).Returns(new[] { player });
        
        _game.StartGame();

        Assert.Equal(TestCardsCount, player.Cards.Count);
    }

    [Fact]
    public void StartGame_ShouldPresentTemplateCard()
    {
        ArrangeReadyToStartGame();

        _game.StartGame();

        Assert.Equal(TestTemplate, _game.TemplateCard.Content);
    }

    private void ArrangeReadyToStartGame()
    {
        _gameStatusMock.Setup(s => s.Current).Returns(GameState.Waiting);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { new Player(Guid.NewGuid()), new Player(Guid.NewGuid()), new Player(Guid.NewGuid()) });
        _cardRepoMock.Setup(r => r.DrawTemplate()).Returns(new Template(TestTemplate));
        _cardRepoMock.Setup(r => r.DrawCards(8)).Returns(new Card[TestCardsCount]);
    }
}
