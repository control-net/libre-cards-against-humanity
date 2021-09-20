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

        public CardState(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Template CurrentTemplateCard { get; private set; }

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
