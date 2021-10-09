using System.Collections.Generic;

namespace LibreCards.Core.Entities
{
    public class Response
    {
        public int Id { get; set; }
        public IEnumerable<Card> Cards { get; set; }
    }
}
