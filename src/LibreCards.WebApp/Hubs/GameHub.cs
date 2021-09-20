using LibreCards.Core;
using LibreCards.Core.Entities;
using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.SignalR;

namespace LibreCards.WebApp.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGame _game;

        private readonly IPlayerConnectionStorage _connections;

        public GameHub(IGame game, IPlayerConnectionStorage connections)
        {
            _game = game;
            _connections = connections;
        }

        public async Task Join(string username)
        {
            var connId = Context.ConnectionId;

            if(_connections.ConnectionExists(connId))
            {
                Console.WriteLine("The user already received a GUID");
                return;
            }

            if(_connections.UsernameIsTaken(username))
            {
                Console.WriteLine($"Someone already has the same username (${username})");
                return;
            }

            var id = Guid.NewGuid();
            var model = new PlayerModel(id, username);

            _connections.AddConnection(connId, model);

            await Clients.Others.SendAsync("PlayerJoined", model);
            await Clients.Caller.SendAsync("IdAssigned", id);

            _game.Lobby.AddPlayer(new Player(id)
            {
                Username = username
            });
        }

        public async Task Leave(Guid id)
        {
            if(!_connections.ConnectionExists(Context.ConnectionId))
            {
                // This connection isn't even registered
                return;
            }

            var storedConn = _connections.GetConnectionIdByPlayerId(id);
            if (storedConn is null || storedConn != Context.ConnectionId)
            {
                // The provided GUID doesn't match this connection
                return;
            }

            _game.Lobby.RemovePlayer(id);
            _connections.RemoveConnection(Context.ConnectionId);

            await Clients.Others.SendAsync("PlayerLeft", id);
        }

        public async Task GetPlayers()
        {
            await Clients.Caller.SendAsync("PlayerList", _game.Lobby.Players.Select(p => new PlayerModel(p.Id, p.Username)));
        }

        public async Task StartGame()
        {
            // TODO(Peter): Only the lobby owner (Player who has been in it the longest) can start the game

            // FIXME(Peter): This will throw if the game cannot start for some reason.
            _game.StartGame();

            // NOTE(Peter): We should probably reduce the number of calls to everyone here.
            //              Technically the Client could request a template on their own when they
            //              receive a GameStarted event.
            await Clients.All.SendAsync("GameStarted", new GameModel { JudgeId = _game.JudgePlayerId });
            await Clients.All.SendAsync("UpdateTemplate", _game.TemplateCard.Content, _game.TemplateCard.BlankCount);

            foreach(var user in _connections.Connections)
            {
                var gameUser = _game.Lobby.Players.FirstOrDefault(p => p.Id == user.Value.Id);
                await Clients.Client(user.Key).SendAsync("UpdateCards", gameUser.Cards.Select(c => c.Text));
            }
        }

        public async Task GetMyCards(Guid id)
        {
            var player = _game.Lobby.Players.FirstOrDefault(p => p.Id == id);

            if (player is null)
                return;

            await Clients.Caller.SendAsync("UpdateCards", player.Cards.Select(c => new CardModel(c.Id, c.Text)));
        }

        public async Task RequestTemplate()
        {
            // FIXME(Peter): The game might not be in progress...

            await Clients.Caller.SendAsync("UpdateTemplate", _game.TemplateCard.Content, _game.TemplateCard.BlankCount);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            if(!_connections.ConnectionExists(Context.ConnectionId))
            {
                // User wasn't registered...
                return;
            }

            var id = _connections.GetByConnectionId(Context.ConnectionId).Id;
            _game.Lobby.RemovePlayer(id);
            _connections.RemoveConnection(Context.ConnectionId);

            await Clients.Others.SendAsync("PlayerLeft", id);
        }
    }
}
