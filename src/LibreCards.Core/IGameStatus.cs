using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public interface IGameStatus
    {
        GameState Current { get; }
        void SwitchToPlaying();
        void SwitchToWaiting();
        void SwitchToJudging();
    }
}
