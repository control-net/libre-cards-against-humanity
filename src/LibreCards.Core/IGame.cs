namespace LibreCards.Core
{
    public interface IGame
    {
        ILobby Lobby { get; }
        void StartGame();
    }
}
