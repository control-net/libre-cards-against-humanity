using LibreCards.Core;
using LibreCards.Core.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibreCards.WebApp.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGame _game;

        public GameHub()
        {
            var gameStatus = new GameStatus();
            var lobby = new Lobby(2, gameStatus);
            _game = new Game(gameStatus, null, lobby);
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
            await Clients.Caller.SendAsync("PlayerList", _game.Lobby.GetPlayers().Select(p => p.Id));
        }
    }
}
