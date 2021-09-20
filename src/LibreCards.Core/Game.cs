using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class Game : IGame
    {
        private readonly IGameStatus _gameStatus;
        private readonly ICardRepository _cardRepository;
        private readonly ILobby _lobby;
        private readonly IJudgePicker _judgePicker;

        public Game(IGameStatus gameStatus, ICardRepository cardRepository, ILobby lobby, IJudgePicker judgePicker)
        {
            _gameStatus = gameStatus;
            _cardRepository = cardRepository;
            _lobby = lobby;
            _judgePicker = judgePicker;
        }

        public ILobby Lobby => _lobby;

        public Guid JudgePlayerId => _judgePicker.CurrentJudgeId;

        public Template TemplateCard { get; private set; }

        public void StartGame()
        {
            if (_gameStatus.Current != GameState.Waiting)
                throw new InvalidOperationException("Game is already in progress.");

            if(!_lobby.HasEnoughPlayers)
                throw new InvalidOperationException("Not enough players.");

            _gameStatus.SwitchToPlaying();
            _judgePicker.PickNewJudge(_lobby.Players);
            SetupNewRound();
        }

        private void SetupNewRound()
        {
            foreach(var player in _lobby.Players)
            {
                var playerCards = player.Cards.ToList();
                var cards = _cardRepository.DrawCards(8 - player.Cards.Count);
                playerCards.AddRange(cards);
                player.Cards = playerCards;
            }

            TemplateCard = _cardRepository.DrawTemplate();
        }
    }
}
