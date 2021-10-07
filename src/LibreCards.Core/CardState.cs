using LibreCards.Core.Entities;
using LibreCards.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core
{
    public class CardState : ICardState
    {
        private readonly ICardRepository _cardRepository;

        private Dictionary<Guid, IEnumerable<Card>> _responses;

        public CardState(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            _responses = new Dictionary<Guid, IEnumerable<Card>>();
        }

        public Template CurrentTemplateCard { get; private set; }

        public IEnumerable<Response> PlayerResponses => _responses.Select(ToResponse);

        private static Response ToResponse(KeyValuePair<Guid, IEnumerable<Card>> source, int index)
        {
            return new Response
            {
                Cards = source.Value,
                Id = index
            };
        }

        public void AddPlayerResponse(Guid playerId, IEnumerable<Card> cards)
        {
            if (_responses.ContainsKey(playerId))
                throw new InvalidOperationException("A player with this ID already responded.");

            _responses.Add(playerId, cards);
        }

        public void DrawTemplateCard()
        {
            CurrentTemplateCard = _cardRepository.DrawTemplate();
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
