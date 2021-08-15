using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public interface IGameStatus
    {
        bool IsInProgress { get; }
        void SetInProgress();
        void SetWaiting();
    }
}
