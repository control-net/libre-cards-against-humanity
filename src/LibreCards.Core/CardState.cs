
using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibreCards.Core
{
    public class CardState : ICardState
    {
        private readonly ICardRepository _cardRepository;

        private readonly Dictionary<Guid, IEnumerable<Card>> _responses;

        public CardState(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            _responses = new Dictionary<Guid, IEnumerable<Card>>();
        }

        public Template CurrentTemplateCard { get; private set; }

        public IEnumerable<Response> PlayerResponses => _responses.Select(ToResponse).ToList();

        private static Response ToResponse(KeyValuePair<Guid, IEnumerable<Card>> source, int index)
        {
            return new Response
            {
                Cards = source.Value,
                Id = index
            };
        }

        public Task AddFromUrl(string url) => _cardRepository.AddFromUrl(url);

        public void AddPlayerResponse(Guid playerId, IEnumerable<Card> cards)
        {
            if (_responses.ContainsKey(playerId))
                throw new InvalidOperationException("A player with this ID already responded.");

            _responses.Add(playerId, cards.ToList());
        }

        public void ClearResponses() => _responses.Clear();

        public void DrawTemplateCard()
        {
            CurrentTemplateCard = _cardRepository.DrawTemplate();
        }

        public bool GetPlayerVoted(Guid id) => _responses.ContainsKey(id);

        public bool GetVotingCompleted(IReadOnlyCollection<Player> players)
        {
            return _responses.Count == players.Count - 1;
        }

        public Guid PickBestResponse(int id)
        {
            if (id < 0 || id >= _responses.Count)
                throw new IndexOutOfRangeException("The best response index was out of range.");

            return _responses.ElementAt(id).Key;
        }

        public void RefillPlayerCards(IReadOnlyCollection<Player> players)
        {
            if (players is null)
                throw new ArgumentNullException(nameof(players));

            foreach (var player in players)
            {
                var playerCards = player.Cards.ToList();
                var cards = _cardRepository.DrawCards(8 - player.Cards.Count);
                playerCards.AddRange(cards);
                player.Cards = playerCards;
            }
        }
    }
}
