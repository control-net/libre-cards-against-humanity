using LibreCards.Core;
using Xunit;

namespace LibreCards.Tests
{
    public class GameStatusTests
    {
        private readonly GameStatus _status;

        public GameStatusTests()
        {
            _status = new GameStatus();
        }

        [Fact]
        public void GameStatus_ShouldStartAsWaiting()
        {
            Assert.False(_status.IsInProgress);
        }

        [Fact]
        public void GameStatus_ShouldSwitchToProgress()
        {
            _status.SetInProgress();

            Assert.True(_status.IsInProgress);
        }

        [Fact]
        public void GameStatus_ShouldSwitchToProgressAndThenToWaiting()
        {
            _status.SetInProgress();
            Assert.True(_status.IsInProgress);

            _status.SetWaiting();

            Assert.False(_status.IsInProgress);
        }
    }
}
