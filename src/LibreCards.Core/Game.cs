using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class Game : IGame
    {
        public Guid JudgePlayerId { get; private set; }
        private Queue<Guid> _judgeQueue;
        
        private readonly IGameStatus _gameStatus;
        private readonly ICardRepository _cardRepository;
        private readonly ILobby _lobby;

        public ILobby Lobby => _lobby;

        public Template TemplateCard { get; private set; }

        public Game(IGameStatus gameStatus, ICardRepository cardRepository, ILobby lobby)
        {
            _gameStatus = gameStatus;
            _cardRepository = cardRepository;
            _lobby = lobby;
        }

        public void StartGame()
        {
            if (_gameStatus.IsInProgress)
                return;

            if(!_lobby.HasEnoughPlayers)
                throw new InvalidOperationException("Not enough players.");

            _gameStatus.SetInProgress();
            SetupJudgeQueue();
            SetupNewRound();
        }

        private void SetupNewRound()
        {
            if(_judgeQueue.Count == 0)
                SetupJudgeQueue();
            
            if(_judgeQueue.Count != 0)
                JudgePlayerId = _judgeQueue.Dequeue();

            foreach(var player in _lobby.Players)
            {
                var playerCards = player.Cards.ToList();
                var cards = _cardRepository.DrawCards(8 - player.Cards.Count);
                playerCards.AddRange(cards);
                player.Cards = playerCards;
            }

            TemplateCard = _cardRepository.DrawTemplate();
        }

        private void SetupJudgeQueue()
            => _judgeQueue = new Queue<Guid>(_lobby.Players.Select(p => p.Id).Reverse());
    }
}
