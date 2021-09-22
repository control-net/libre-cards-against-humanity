namespace LibreCards.Core.Entities.Client
{
    public class LobbyModel
    {
        public int PlayerCount { get; set; }

        public int MaximumPlayers { get; set; }

        public bool CanJoin { get; set; }

        public LobbyModel()
        {
        }

        public LobbyModel(ILobby lobby)
        {
            PlayerCount = lobby.Players.Count;
            MaximumPlayers = lobby.MaximumPlayerCount;
            CanJoin = PlayerCount < MaximumPlayers;
        }
    }
}
