using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public class GameStatus : IGameStatus
    {
        public GameState CurrentState { get; private set; }

        public GameStatus()
        {
            CurrentState = GameState.Waiting;
        }

        public void SwitchToPlaying() => CurrentState = GameState.Playing;

        public void SwitchToWaiting() => CurrentState = GameState.Waiting;

        public void SwitchToJudging() => CurrentState = GameState.Judging;
    }
}
