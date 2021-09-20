using LibreCards.Core;
using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using Moq;
using Xunit;

namespace LibreCards.Tests.Gameplay;

public class GameTests
{
    private readonly Mock<ICardRepository> _cardRepoMock;
    private readonly IGameStatus _gameStatus;
    private ILobby _lobby;
    private IGame _game;

    public GameTests()
    {
        _cardRepoMock = new Mock<ICardRepository>();
        _gameStatus = new GameStatus();
        _lobby = new Lobby(0, _gameStatus);
        _game = new Game(_gameStatus, _cardRepoMock.Object, _lobby);
    }

    [Fact]
    public void NotEnoughPlayers_CannotStartGame()
    {
        ArrangeGameWithMinPlayers(1);

        Assert.Throws<InvalidOperationException>(() => _game.StartGame());
    }

    [Fact]
    public void NewGame_PlayerShouldGetCards()
    {
        var playerId = Guid.NewGuid();
        _cardRepoMock.Setup(r => r.DrawCards(8)).Returns(new Card[8]);
        _game.Lobby.AddPlayer(new Player(playerId));
        _game.StartGame();

        var player = _game.Lobby.Players.FirstOrDefault(p => p.Id == playerId);

        Assert.NotEmpty(player.Cards);
    }

    [Fact]
    public void NewGame_ShouldPresentTemplateCard()
    {
        const string ExpectedTemplate = "Test <BLANK> template!";
        _cardRepoMock.Setup(r => r.DrawTemplate()).Returns(new Template(ExpectedTemplate));

        _game.Lobby.AddPlayer(new Player(Guid.NewGuid()));
        _game.StartGame();

        Assert.NotNull(_game.TemplateCard);
        Assert.Equal(ExpectedTemplate, _game.TemplateCard.Content);
    }

    private void ArrangeGameWithMinPlayers(int minPlayers)
    {
        _lobby = new Lobby(minPlayers, _gameStatus);
        _game = new Game(_gameStatus, _cardRepoMock.Object, _lobby);
    }
}
