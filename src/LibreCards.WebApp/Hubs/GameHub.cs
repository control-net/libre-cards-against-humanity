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
            _game = new Game(2);
        }

        public async Task Join()
        {
            var id = Guid.NewGuid();

            await Clients.Others.SendAsync("PlayerJoined", id);
            await Clients.Caller.SendAsync("IdAssigned", id);

            _game.AddPlayer(new Player(id));
        }

        public async Task Leave(Guid id)
        {
            _game.RemovePlayer(id);

            await Clients.Others.SendAsync("PlayerLeft", id);
        }

        public async Task GetPlayers()
        {
            await Clients.Caller.SendAsync("PlayerList", _game.GetPlayers().Select(p => p.Id));
        }
    }
}
