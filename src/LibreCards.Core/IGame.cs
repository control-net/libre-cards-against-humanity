using LibreCards.Core.Entities;

namespace LibreCards.Core
{
    public interface IGame
    {
        ILobby Lobby { get; }
        void StartGame();
        Template TemplateCard { get; }
    }
}
