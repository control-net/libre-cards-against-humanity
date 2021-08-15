using LibreCards.Core;
using LibreCards.Core.Entities;
using System;
using Xunit;

namespace LibreCards.Tests
{
    public class GameTests
    {
        private IGame _game;
        private ICardRepository _cardRepo;
        private IGameStatus _gameStatus;

        public GameTests()
        {
            _cardRepo = new MockCardRepository();
            _gameStatus = new GameStatus();
            _game = new Game(0, _gameStatus, _cardRepo);
        }

        [Fact]
        public void WhenGameIsInProgress_PlayersCannotJoin()
        {
            _game.StartGame();

            Assert.Throws<InvalidOperationException>(() => _game.AddPlayer(new Player(Guid.NewGuid())));
        }

        [Fact]
        public void WhenGameIsNotInProgress_PlayersCanJoin()
        {
            _game.AddPlayer(new Player(Guid.NewGuid()));

            Assert.Equal(1, _game.PlayerCount);
        }

        [Fact]
        public void NewGame_HasNoPlayers()
        {
            Assert.Equal(0, _game.PlayerCount);
        }

        [Fact]
        public void NotEnoughPlayers_CannotStartGame()
        {
            _game = new Game(1, _gameStatus, _cardRepo);

            Assert.Throws<InvalidOperationException>(() => _game.StartGame());
        }

        [Fact]
        public void CannotSetMaximumPlayerCountLowerThanMinimum()
        {
            _game = new Game(2, _gameStatus, _cardRepo);

            Assert.Throws<ArgumentOutOfRangeException>(() => _game.SetMaxPlayerCount(1));
        }

        [Fact]
        public void CannotSetZeroMaxPlayerCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _game.SetMaxPlayerCount(0));
        }

        [Fact]
        public void RemovingNonExistentPlayerDoesNothing()
        {
            _game.AddPlayer(new Player(Guid.NewGuid()));

            _game.RemovePlayer(Guid.NewGuid());

            Assert.Equal(1, _game.PlayerCount);
        }

        [Fact]
        public void RemovingValidPlayerShouldRemovePlayer()
        {
            var id = Guid.NewGuid();

            _game.AddPlayer(new Player(id));
            _game.AddPlayer(new Player(Guid.NewGuid()));

            _game.RemovePlayer(id);

            Assert.Equal(1, _game.PlayerCount);
        }

        [Fact]
        public void WhenGameIsInProgress_CannotChangeMaxPlayers()
        {
            _game.StartGame();

            Assert.Throws<InvalidOperationException>(() => _game.SetMaxPlayerCount(10));
        }

        [Fact]
        public void NewGame_PlayerShouldGetCards()
        {
            var playerId = Guid.NewGuid();
            _game.AddPlayer(new Player(playerId));
            _game.StartGame();

            var player = _game.GetPlayer(playerId);

            Assert.NotEmpty(player.Cards);
        }

        [Fact(Skip = "Exploration test")]
        public void ExplorationTest()
        {
            _game = new Game(4, _gameStatus, _cardRepo);

            Assert.Throws<InvalidOperationException>(() => _game.StartGame());

            var id = Guid.NewGuid();

            _game.AddPlayer(new Player(id));
            _game.AddPlayer(new Player(Guid.NewGuid()));
            _game.AddPlayer(new Player(Guid.NewGuid()));
            _game.AddPlayer(new Player(Guid.NewGuid()));

            _game.StartGame();

            Assert.Throws<InvalidOperationException>(() => _game.AddPlayer(new Player(Guid.NewGuid())));

            _game.RemovePlayer(id);

            Assert.Equal(3, _game.PlayerCount);
            Assert.False(_gameStatus.IsInProgress);

            _game.AddPlayer(new Player(Guid.NewGuid()));
            _game.AddPlayer(new Player(Guid.NewGuid()));
            _game.StartGame();
        }
    }
}
