using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public class GameStatus : IGameStatus
    {
        private GameState _gameState;

        public GameStatus()
        {
            _gameState = GameState.Waiting;
        }

        public bool IsInProgress => _gameState == GameState.InProgress;

        public void SetInProgress() => _gameState = GameState.InProgress;

        public void SetWaiting() => _gameState = GameState.Waiting;
    }
}
