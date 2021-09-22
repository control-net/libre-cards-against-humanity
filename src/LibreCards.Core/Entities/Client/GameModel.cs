using System;
using System.Collections.Generic;

namespace LibreCards.Core.Entities.Client
{
    public class GameModel
    {
        public PlayerState LocalPlayerState { get; set; } = PlayerState.NotInLobby;

        public Guid LocalPlayerId { get; set; }

        public Guid JudgeId { get; set; }

        public IEnumerable<PlayerModel> Players { get; set; } = new List<PlayerModel>();

        public IEnumerable<CardModel> Cards { get; set; } = new List<CardModel>();

        public TemplateModel Template { get; set; }
    }
}
