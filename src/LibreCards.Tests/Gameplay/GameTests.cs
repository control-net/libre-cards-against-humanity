using LibreCards.Core;
using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using Moq;
using Xunit;

namespace LibreCards.Tests.Gameplay
{
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
        public void WhenGameIsInProgress_PlayersCannotJoin()
        {
            _game.StartGame();

            Assert.Throws<InvalidOperationException>(() => _game.Lobby.AddPlayer(new Player(Guid.NewGuid())));
        }

        [Fact]
        public void WhenGameIsNotInProgress_PlayersCanJoin()
        {
            _game.Lobby.AddPlayer(new Player(Guid.NewGuid()));

            Assert.Equal(1, _game.Lobby.PlayerCount);
        }

        [Fact]
        public void NewGame_HasNoPlayers()
        {
            Assert.Equal(0, _game.Lobby.PlayerCount);
        }

        [Fact]
        public void NotEnoughPlayers_CannotStartGame()
        {
            ArrangeGameWithMinPlayers(1);

            Assert.Throws<InvalidOperationException>(() => _game.StartGame());
        }

        [Fact]
        public void CannotSetMaximumPlayerCountLowerThanMinimum()
        {
            ArrangeGameWithMinPlayers(2);

            Assert.Throws<ArgumentOutOfRangeException>(() => _game.Lobby.SetMaxPlayerCount(1));
        }

        [Fact]
        public void CannotSetZeroMaxPlayerCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _game.Lobby.SetMaxPlayerCount(0));
        }

        [Fact]
        public void RemovingNonExistentPlayerDoesNothing()
        {
            _game.Lobby.AddPlayer(new Player(Guid.NewGuid()));

            _game.Lobby.RemovePlayer(Guid.NewGuid());

            Assert.Equal(1, _game.Lobby.PlayerCount);
        }

        [Fact]
        public void RemovingValidPlayerShouldRemovePlayer()
        {
            var id = Guid.NewGuid();

            _game.Lobby.AddPlayer(new Player(id));
            _game.Lobby.AddPlayer(new Player(Guid.NewGuid()));

            _game.Lobby.RemovePlayer(id);

            Assert.Equal(1, _game.Lobby.PlayerCount);
        }

        [Fact]
        public void WhenGameIsInProgress_CannotChangeMaxPlayers()
        {
            _game.StartGame();

            Assert.Throws<InvalidOperationException>(() => _game.Lobby.SetMaxPlayerCount(10));
        }

        [Fact]
        public void NewGame_PlayerShouldGetCards()
        {
            var playerId = Guid.NewGuid();
            _cardRepoMock.Setup(r => r.DrawCards(8)).Returns(new Card[8]);
            _game.Lobby.AddPlayer(new Player(playerId));
            _game.StartGame();

            var player = _game.Lobby.GetPlayer(playerId);

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
}
