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
    }
}
