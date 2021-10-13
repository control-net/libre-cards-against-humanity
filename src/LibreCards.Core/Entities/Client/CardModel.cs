using System;

namespace LibreCards.Core.Entities.Client
{
    public class CardModel
    {
        public int UniqueId { get; private set; }

        public int Id { get; private set; }

        public string Text { get; private set; }

        public CardModel(int id, string text, int uniqueId)
        {
            Id = id;
            Text = text;
            UniqueId = uniqueId;
        }

        public static CardModel FromEntity(Card card, int uniqueId)
        {
            return new CardModel(card.Id, card.Text, uniqueId);
        }
    }
}
