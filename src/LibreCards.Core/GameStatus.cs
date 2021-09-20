using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public class GameStatus : IGameStatus
    {
        public GameState Current { get; private set; }

        public GameStatus()
        {
            Current = GameState.Waiting;
        }

        public void SwitchToPlaying() => Current = GameState.Playing;

        public void SwitchToWaiting() => Current = GameState.Waiting;

        public void SwitchToJudging() => Current = GameState.Judging;
    }
}
