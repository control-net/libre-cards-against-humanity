using LibreCards.Core;
using LibreCards.Core.Entities;
using Microsoft.AspNetCore.SignalR;

namespace LibreCards.WebApp.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGame _game;

        public GameHub(IGame game)
        {
            _game = game;
        }

        public async Task Join()
        {
            var id = Guid.NewGuid();

            await Clients.Others.SendAsync("PlayerJoined", id);
            await Clients.Caller.SendAsync("IdAssigned", id);

            _game.Lobby.AddPlayer(new Player(id));
        }

        public async Task Leave(Guid id)
        {
            _game.Lobby.RemovePlayer(id);

            await Clients.Others.SendAsync("PlayerLeft", id);
        }

        public async Task GetPlayers()
        {
            await Clients.Caller.SendAsync("PlayerList", _game.Lobby.Players.Select(p => p.Id));
        }

        public async Task StartGame()
        {
            // TODO(Peter): Only the lobby owner (Player who has been in it the longest) can start the game

            // FIXME(Peter): This will throw if the game cannot start for some reason.
            _game.StartGame();

            await Clients.All.SendAsync("GameStarted");
        }

        public async Task GetMyCards(Guid id)
        {
            var player = _game.Lobby.GetPlayer(id);

            if (player is null)
                return;

            await Clients.Caller.SendAsync("UpdateCards", player.Cards.Select(c => c.Text));
        }
    }
}
