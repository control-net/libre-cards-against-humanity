using LibreCards.Core;
using LibreCards.Core.Entities;
using Moq;
using Xunit;

namespace LibreCards.Tests.Gameplay;

public class GameTests
{
    private readonly Mock<ICardState> _cardStateMock;
    private readonly Mock<IGameStatus> _gameStatusMock;
    private readonly Mock<ILobby> _lobbyMock;
    private readonly Mock<IJudgePicker> _judgePickerMock;

    private readonly IGame _game;

    public GameTests()
    {
        _cardStateMock = new Mock<ICardState>();
        _gameStatusMock = new Mock<IGameStatus>();
        _lobbyMock = new Mock<ILobby>();
        _judgePickerMock = new Mock<IJudgePicker>();

        _game = new Game(_gameStatusMock.Object, _cardStateMock.Object, _lobbyMock.Object, _judgePickerMock.Object);
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
        _gameStatusMock.Setup(s => s.CurrentState).Returns(state);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame());
    }

    [Fact]
    public void StartGame_PlayersShouldGetCards()
    {
        ArrangeReadyToStartGame();
        
        _game.StartGame();

        _cardStateMock.Verify(s => s.RefillPlayerCards(It.IsAny<IReadOnlyCollection<Player>>()), Times.Once());
    }

    [Fact]
    public void StartGame_ShouldPresentTemplateCard()
    {
        ArrangeReadyToStartGame();

        _game.StartGame();

        _cardStateMock.Verify(s => s.DrawTemplateCard(), Times.Once());
    }

    [Fact]
    public void StartGame_ShouldSetStatusToPlaying()
    {
        ArrangeReadyToStartGame();

        _game.StartGame();

        _gameStatusMock.Verify(s => s.CurrentState, Times.AtLeastOnce());
        _gameStatusMock.Verify(s => s.SwitchToPlaying(), Times.Once());
        _gameStatusMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void StartGame_ShouldPickNewJudge()
    {
        ArrangeReadyToStartGame();

        _game.StartGame();

        _judgePickerMock.Verify(j => j.PickNewJudge(It.IsAny<IReadOnlyCollection<Player>>()), Times.Once());
    }

    private void ArrangeReadyToStartGame()
    {
        _gameStatusMock.Setup(s => s.CurrentState).Returns(GameState.Waiting);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { new Player(Guid.NewGuid()), new Player(Guid.NewGuid()), new Player(Guid.NewGuid()) });
    }
}
