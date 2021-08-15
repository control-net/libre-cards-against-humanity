using LibreCards.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class Game : IGame
    {
        private Guid _judgePlayer;
        private Queue<Guid> _judgeQueue;
        
        private readonly IGameStatus _gameStatus;
        private readonly ICardRepository _cardRepository;

        public Game(int minimumPlayerCount, IGameStatus gameStatus, ICardRepository cardRepository)
        {
            MinimumPlayerCount = minimumPlayerCount;
            MaximumPlayerCount = 10;
            _gameStatus = gameStatus;
            _cardRepository = cardRepository;
        }

        public int MinimumPlayerCount { get; private set; }

        public int MaximumPlayerCount { get; private set; }

        public int PlayerCount => Players.Count;

        public ICollection<Player> Players { get; set; } = new List<Player>();

        public void AddPlayer(Player player)
        {
            if(_gameStatus.IsInProgress)
                throw new InvalidOperationException();

            Players.Add(player);
        }

        public IEnumerable<Player> GetPlayers() => Players;

        public void RemovePlayer(Guid id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);

            if (player is null)
                return;

            Players.Remove(player);

            if (PlayerCount < MaximumPlayerCount)
                _gameStatus.SetWaiting();
        }

        public void SetMaxPlayerCount(int maxPlayerCount)
        {
            if(_gameStatus.IsInProgress)
                throw new InvalidOperationException();

            if(maxPlayerCount == 0 || maxPlayerCount < MinimumPlayerCount)
                throw new ArgumentOutOfRangeException(nameof(maxPlayerCount));

            MaximumPlayerCount = maxPlayerCount;
        }

        public void StartGame()
        {
            if (_gameStatus.IsInProgress)
                return;

            if(PlayerCount < MinimumPlayerCount)
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
                _judgePlayer = _judgeQueue.Dequeue();

            foreach(var player in Players)
            {
                var playerCards = player.Cards.ToList();
                var cards = _cardRepository.DrawCards(8 - player.Cards.Count);
                playerCards.AddRange(cards);
                player.Cards = playerCards;
            }
        }

        private void SetupJudgeQueue()
            => _judgeQueue = new Queue<Guid>(Players.Select(p => p.Id).Reverse());

        public Player GetPlayer(Guid id)
        {
            var player = Players.FirstOrDefault(p => p.Id == id);

            if(player is null)
                throw new InvalidOperationException($"No player with id '{id}' found.");

            return player;
        }
    }
}
