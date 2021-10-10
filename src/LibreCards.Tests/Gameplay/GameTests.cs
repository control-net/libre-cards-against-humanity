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

    private readonly Player LobbyOwner = new(Guid.NewGuid());
    private readonly Player JudgePlayer = new(Guid.NewGuid());

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

        Assert.Throws<InvalidOperationException>(() => _game.StartGame(LobbyOwner.Id));
    }

    [Theory]
    [InlineData(GameState.Playing)]
    [InlineData(GameState.Judging)]
    public void StartGame_InProgress_ShouldThrow(GameState state)
    {
        ArrangeReadyToStartGame();
        _gameStatusMock.Setup(s => s.CurrentState).Returns(state);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame(LobbyOwner.Id));
    }

    [Fact]
    public void StartGame_PlayersShouldGetCards()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwner.Id);

        _cardStateMock.Verify(s => s.RefillPlayerCards(It.IsAny<IReadOnlyCollection<Player>>()), Times.Once());
    }

    [Fact]
    public void StartGame_ShouldPresentTemplateCard()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwner.Id);

        _cardStateMock.Verify(s => s.DrawTemplateCard(), Times.Once());
    }

    [Fact]
    public void StartGame_ShouldSetStatusToPlaying()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwner.Id);

        _gameStatusMock.Verify(s => s.CurrentState, Times.AtLeastOnce());
        _gameStatusMock.Verify(s => s.SwitchToPlaying(), Times.Once());
        _gameStatusMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void StartGame_ShouldPickNewJudge()
    {
        ArrangeReadyToStartGame();

        _game.StartGame(LobbyOwner.Id);

        _judgePickerMock.Verify(j => j.PickNewJudge(It.IsAny<IReadOnlyCollection<Player>>()), Times.Once());
    }

    [Fact]
    public void StartGame_NotLobbyOwner_ShouldThrow()
    {
        ArrangeReadyToStartGame();

        Assert.Throws<InvalidOperationException>(() => _game.StartGame(Guid.NewGuid()));
    }

    [Fact]
    public void PlayCards_NoCardIds_ShouldThrow()
    {
        ArrangeStartedGame();

        Assert.Throws<ArgumentException>(() => _game.PlayCards(LobbyOwner.Id, Array.Empty<int>()));
    }

    [Fact]
    public void PlayCards_NotPlaying_ShouldThrow()
    {
        const int cardId = 1;
        LobbyOwner.Cards.Add(new Card { Id = cardId });
        ArrangeReadyToStartGame();

        Assert.Throws<InvalidOperationException>(() => _game.PlayCards(LobbyOwner.Id, new[] { cardId }));
    }

    [Fact]
    public void PlayCards_JudgePlay_ShouldThrow()
    {
        const int cardId = 1;
        JudgePlayer.Cards.Add(new Card { Id = cardId });
        ArrangeStartedGame();

        Assert.Throws<InvalidOperationException>(() => _game.PlayCards(JudgePlayer.Id, new[] { cardId }));
    }

    [Fact]
    public void PlayCards_UnknownCardId_ShouldThrow()
    {
        const int cardId = 3;
        ArrangeStartedGame();

        Assert.Throws<InvalidOperationException>(() => _game.PlayCards(LobbyOwner.Id, new[] { cardId }));
    }

    [Fact]
    public void PlayCards_MoreCardsThanInHand_ShouldThrow()
    {
        const int cardId = 3;
        LobbyOwner.Cards.Add(new Card { Id = cardId });
        ArrangeStartedGame();

        Assert.Throws<InvalidOperationException>(() => _game.PlayCards(LobbyOwner.Id, new[] { cardId, cardId }));
    }

    [Fact]
    public void PlayCards_MoreCardsThanInTemplate_ShouldThrow()
    {
        LobbyOwner.Cards.Add(new Card { Id = 1 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 3 });
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template("<BLANK> + <BLANK>"));
        ArrangeStartedGame();

        Assert.Throws<InvalidOperationException>(() => _game.PlayCards(LobbyOwner.Id, new[] { 1, 2, 3 }));
    }

    [Fact]
    public void PlayCards_LessCardsThanInTemplate_ShouldThrow()
    {
        LobbyOwner.Cards.Add(new Card { Id = 1 });
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template("<BLANK> + <BLANK>"));
        ArrangeStartedGame();

        Assert.Throws<InvalidOperationException>(() => _game.PlayCards(LobbyOwner.Id, new[] { 1 }));
    }

    [Fact]
    public void PlayCards_CorrectCards_ShouldRemoveCards()
    {
        LobbyOwner.Cards.Add(new Card { Id = 1 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 3 });
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template("<BLANK> + <BLANK>"));
        ArrangeStartedGame();

        _game.PlayCards(LobbyOwner.Id, new[] { 1, 2 });

        Assert.Single(LobbyOwner.Cards);
        Assert.Equal(3, LobbyOwner.Cards.First().Id);
    }

    [Fact]
    public void PlayCards_CorrectCards_ShouldNotRemoveDuplicates()
    {
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template("<BLANK> + <BLANK>"));
        ArrangeStartedGame();

        _game.PlayCards(LobbyOwner.Id, new[] { 2, 2 });

        Assert.Single(LobbyOwner.Cards);
        Assert.Equal(2, LobbyOwner.Cards.First().Id);
    }

    [Fact]
    public void PlayCards_CorrectCards_ShouldCallCardState()
    {
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template("<BLANK> + <BLANK>"));
        ArrangeStartedGame();

        _game.PlayCards(LobbyOwner.Id, new[] { 2, 2 });

        _cardStateMock.Verify(c => c.AddPlayerResponse(LobbyOwner.Id, It.IsAny<IEnumerable<Card>>()), Times.Once());
    }

    [Fact]
    public void PlayCards_CorrectCards_FinalVote_ShouldChangeGameState()
    {
        _cardStateMock.Setup(c => c.GetVotingCompleted(It.IsAny<IReadOnlyCollection<Player>>())).Returns(true);
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        LobbyOwner.Cards.Add(new Card { Id = 2 });
        _cardStateMock.Setup(c => c.CurrentTemplateCard).Returns(new Template("<BLANK> + <BLANK>"));
        ArrangeStartedGame();

        _game.PlayCards(LobbyOwner.Id, new[] { 2, 2 });

        _gameStatusMock.Verify(s => s.SwitchToJudging(), Times.Once());
        _cardStateMock.Verify(c => c.ClearResponses(), Times.Once());
    }

    [Fact]
    public void JudgeCard_NotJudging_ShouldThrow()
    {
        ArrangeJudgingGame();
        _gameStatusMock.Setup(s => s.CurrentState).Returns(GameState.Playing);

        Assert.Throws<InvalidOperationException>(() => _game.JudgeCard(JudgePlayer.Id, 1));
    }

    [Fact]
    public void JudgeCard_NotJudge_ShouldThrow()
    {
        ArrangeJudgingGame();

        Assert.Throws<InvalidOperationException>(() => _game.JudgeCard(LobbyOwner.Id, 1));
    }

    [Fact]
    public void JudgeCard_ShouldAssignPointsCorrectlyAndSwitchGameMode()
    {
        ArrangeJudgingGame();

        _cardStateMock.Setup(c => c.PickBestResponse(1)).Returns(LobbyOwner.Id);

        _game.JudgeCard(JudgePlayer.Id, 1);

        Assert.Equal(1, LobbyOwner.Points);
        _gameStatusMock.Verify(g => g.SwitchToPlaying(), Times.Once());
    }

    private void ArrangeStartedGame()
    {
        _gameStatusMock.Setup(s => s.CurrentState).Returns(GameState.Playing);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { LobbyOwner, JudgePlayer, new Player(Guid.NewGuid()) });
        _judgePickerMock.Setup(j => j.CurrentJudgeId).Returns(JudgePlayer.Id);
    }

    private void ArrangeReadyToStartGame()
    {
        _gameStatusMock.Setup(s => s.CurrentState).Returns(GameState.Waiting);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _lobbyMock.Setup(l => l.OwnerId).Returns(LobbyOwner.Id);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { LobbyOwner, JudgePlayer, new Player(Guid.NewGuid()) });
    }

    private void ArrangeJudgingGame()
    {
        _gameStatusMock.Setup(s => s.CurrentState).Returns(GameState.Judging);
        _lobbyMock.Setup(l => l.HasEnoughPlayers).Returns(true);
        _lobbyMock.Setup(l => l.OwnerId).Returns(LobbyOwner.Id);
        _judgePickerMock.Setup(j => j.CurrentJudgeId).Returns(JudgePlayer.Id);
        _lobbyMock.Setup(l => l.Players).Returns(new[] { LobbyOwner, JudgePlayer, new Player(Guid.NewGuid()) });
    }
}
