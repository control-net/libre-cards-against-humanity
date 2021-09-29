using LibreCards.Core.Entities;
using System;

namespace LibreCards.Core
{
    public class Game : IGame
    {
        private readonly ICardState _cardState;
        private readonly IGameStatus _gameStatus;
        private readonly ILobby _lobby;
        private readonly IJudgePicker _judgePicker;

        public Game(IGameStatus gameStatus, ICardState cardState, ILobby lobby, IJudgePicker judgePicker)
        {
            _cardState = cardState;
            _gameStatus = gameStatus;
            _lobby = lobby;
            _judgePicker = judgePicker;
        }

        public ILobby Lobby => _lobby;

        public Guid JudgePlayerId => _judgePicker.CurrentJudgeId;

        public Template TemplateCard => _cardState.CurrentTemplateCard;

        public GameState GameState => _gameStatus.CurrentState;

        public Guid LobbyOwnerId => _lobby.OwnerId;

        public void StartGame(Guid playerId)
        {
            if (_gameStatus.CurrentState != GameState.Waiting)
                throw new InvalidOperationException("Game is already in progress.");

            if(!_lobby.HasEnoughPlayers)
                throw new InvalidOperationException("Not enough players.");

            if (playerId != LobbyOwnerId)
                throw new InvalidOperationException("Only the lobby owner can start the game.");

            _gameStatus.SwitchToPlaying();
            SetupNewRound();
        }

        private void SetupNewRound()
        {
            _judgePicker.PickNewJudge(_lobby.Players);
            _cardState.RefillPlayerCards(_lobby.Players);
            _cardState.DrawTemplateCard();
        }
    }
}
