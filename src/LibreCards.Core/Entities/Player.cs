using System;

namespace LibreCards.Core.Entities
{
    public class Player
    {
        public Guid Id { get; set; }

        public Player(Guid id)
        {
            Id = id;
        }
    }
}
