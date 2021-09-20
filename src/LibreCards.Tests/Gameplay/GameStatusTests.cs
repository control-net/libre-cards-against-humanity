using LibreCards.Core;
using LibreCards.Core.Entities;
using Xunit;

namespace LibreCards.Tests.Gameplay
{
    public class GameStatusTests
    {
        private readonly IGameStatus _status;

        public GameStatusTests()
        {
            _status = new GameStatus();
        }

        [Fact]
        public void GameStatus_ShouldStartAsWaiting()
        {
            Assert.Equal(GameState.Waiting, _status.Current);
        }

        [Fact]
        public void GameStatus_ShouldSwitchToProgress()
        {
            _status.SwitchToPlaying();

            Assert.Equal(GameState.Playing, _status.Current);
        }

        [Fact]
        public void GameStatus_ShouldSwitchToJudging()
        {
            _status.SwitchToJudging();

            Assert.Equal(GameState.Judging, _status.Current);
        }

        [Fact]
        public void GameStatus_ShouldSwitchToWaiting()
        {
            _status.SwitchToJudging();
            _status.SwitchToWaiting();

            Assert.Equal(GameState.Waiting, _status.Current);
        }
    }
}
