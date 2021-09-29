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

    private readonly Guid LobbyOwnerId = Guid.NewGuid();

    public GameTests()
    {
        _cardStateMock = new Mock<ICardState>();
        _gameStatusMock = new Mock<IGameStatus>();
        _lobbyMock = new Mock<ILobby>();
        _judgePickerMock = new Mock<IJudgePicker>();

        _game = new Game(_gameStatusMock.Object, _cardStateMock.Object, _lobbyMock.Object, _judgePickerMock.Object);
    }

    [Fact]
    public void JudgePlayerId_ShouldReturnFromJudgePicker()
    {
        var expected = Guid.NewGuid();
        _judgePickerMock.Setup(j => j.CurrentJudgeId).Returns(expected);

        Assert.Equal(expected, _game.JudgePlayerId);
    }

    [Fact]
    public void TemplateCard_ShouldReturnFromCardState()
    {
        var expected = "<BLANK>";
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template(expected));

        Assert.Equal(expected, _game.TemplateCard.Content);
    }

    [Fact]
    public void GameState_ShouldReturnFromGameStatus()
    {
        var expected = GameState.Judging;
        _gameStatusMock.Setup(s => s.CurrentState).Returns(expected);

        Assert.Equal(expected, _game.GameState);
    }

    [Fact]
    public void StartGame_NotEnoughPlayers_ShouldThrow()
    {
        ArrangeReadyToStartGame();
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(false);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame(LobbyOwnerId));
    }

    [Theory]
    [InlineData(GameState.Playing)]
    [InlineData(GameState.Judging)]
    public void StartGame_InProgress_ShouldThrow(GameState state)
    {
        ArrangeReadyToStartGame();
        _gameStatusMock.Setup(s => s.CurrentState).Returns(state);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame(LobbyOwnerId));
    }

    [Fact]
    public void StartGame_PlayersShouldGetCards()
    {
        ArrangeReadyToStartGame();
        
        _game.StartGame(LobbyOwnerId);

        _cardStateMock.Verify(s => s.RefillPlayerCards(It.IsAny<IReadOnlyCollection<Player>>()), Times.Once());
    }

    [Fact]
    public void StartGame_ShouldPresentTemplateCard()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwnerId);

        _cardStateMock.Verify(s => s.DrawTemplateCard(), Times.Once());
    }

    [Fact]
    public void StartGame_ShouldSetStatusToPlaying()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwnerId);

        _gameStatusMock.Verify(s => s.CurrentState, Times.AtLeastOnce());
        _gameStatusMock.Verify(s => s.SwitchToPlaying(), Times.Once());
        _gameStatusMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void StartGame_ShouldPickNewJudge()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwnerId);

        _judgePickerMock.Verify(j => j.PickNewJudge(It.IsAny<IReadOnlyCollection<Player>>()), Times.Once());
    }

    [Fact]
    public void StartGame_NotLobbyOwner_ShouldThrow()
    {
        ArrangeReadyToStartGame();

        Assert.Throws<InvalidOperationException>(() => _game.StartGame(Guid.NewGuid()));
    }

    private void ArrangeReadyToStartGame()
    {
        _gameStatusMock.Setup(s => s.CurrentState).Returns(GameState.Waiting);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _lobbyMock.Setup(l => l.OwnerId).Returns(LobbyOwnerId);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { new Player(LobbyOwnerId), new Player(Guid.NewGuid()), new Player(Guid.NewGuid()) });
    }
}
