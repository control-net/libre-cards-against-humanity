using System;

namespace LibreCards.Core.Entities.Client
{
    public class PlayerModel
    {
        public Guid Id {  get; set; }

        public string Username { get; set; }

        public static PlayerModel FromEntity(Player player)
        {
            return new PlayerModel
            {
                Id = player.Id,
                Username = player.Username
            };
        }
    }
}
