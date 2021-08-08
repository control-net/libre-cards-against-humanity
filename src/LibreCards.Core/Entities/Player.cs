using System;
using System.Collections.Generic;
using System.Linq;

namespace LibreCards.Core.Entities
{
    public class Player
    {
        public Guid Id { get; set; }

        public ICollection<Card> Cards { get; set; }

        public Player(Guid id)
        {
            Id = id;
            Cards = new List<Card>();
        }
    }
}
