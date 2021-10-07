using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public void PlayCards(Guid playerId, IEnumerable<int> cardIds)
        {
            if (!cardIds.Any())
                throw new ArgumentException("At least one card ID is required.", nameof(cardIds));

            if (_gameStatus.CurrentState != GameState.Playing)
                throw new InvalidOperationException("A Game must be in Play mode in order to play a card.");

            if (playerId == JudgePlayerId)
                throw new InvalidOperationException("A Judge cannot play a card.");

            var player = Lobby.Players.First(p => p.Id == playerId);

            if (!cardIds.All(id => player.Cards.Any(c => c.Id == id)))
                throw new InvalidOperationException("Not all provided card IDs are in the player's hand.");

            if (cardIds.GroupBy(c => c).All(g => player.Cards.Count(c => c.Id == g.Key) < g.Count()))
                throw new InvalidOperationException("Cannot play more cards than the player has in their hand.");

            if (TemplateCard.BlankCount != cardIds.Count())
                throw new InvalidOperationException($"The current template card requires {TemplateCard.BlankCount} cards.");

            foreach(var card in cardIds.Select(id => player.Cards.First(c => c.Id == id)))
                player.Cards.Remove(card);
        }

        public void StartGame(Guid playerId)
        {
            if (_gameStatus.CurrentState != GameState.Waiting)
                throw new InvalidOperationException("Game is already in progress.");

            if (!_lobby.HasEnoughPlayers)
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
