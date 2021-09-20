using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public interface IGameStatus
    {
        GameState CurrentState { get; }
        void SwitchToPlaying();
        void SwitchToWaiting();
        void SwitchToJudging();
    }
}
