using System;

namespace LibreCards.Core.Entities.Client
{
    public class PlayerModel
    {
        public PlayerModel(Guid id, string username)
        {
            Id = id;
            Username = username;
        }

        public Guid Id {  get; private set; }

        public string Username { get; private set; }
    }
}
