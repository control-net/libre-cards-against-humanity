using System;

namespace LibreCards.Core.Entities.Client
{
    public class CardModel
    {
        public int Id { get; private set; }

        public string Text { get; private set; }

        public CardModel(int id, string text)
        {
            Id = id;
            Text = text;
        }

        public static CardModel FromEntity(Card card)
        {
            return new CardModel(card.Id, card.Text);
        }
    }
}
