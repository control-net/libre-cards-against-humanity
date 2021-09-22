using LibreCards.Core;
using LibreCards.Core.Entities;
using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.SignalR;

namespace LibreCards.WebApp.Hubs;

public class GameHub : Hub
{
    private readonly IGame _game;

    private readonly IPlayerConnectionStorage _connections;

    public GameHub(IGame game, IPlayerConnectionStorage connections)
    {
        _game = game;
        _connections = connections;
    }

    public LobbyModel GetLobbyState() => new(_game.Lobby);

    public async Task Join(string username)
    {
        var connId = Context.ConnectionId;

        if (_connections.ConnectionExists(connId))
        {
            Console.WriteLine("The user already received a GUID");
            return;
        }

        if (_connections.UsernameIsTaken(username))
        {
            Console.WriteLine($"Someone already has the same username (${username})");
            return;
        }

        var id = Guid.NewGuid();
        var model = new PlayerModel
        {
            Id = id,
            Username = username
        };

        _connections.AddConnection(connId, model);

        _game.Lobby.AddPlayer(new Player(id)
        {
            Username = username
        });

        await Clients.All.SendAsync("LobbyUpdated", new LobbyModel(_game.Lobby));
        await SendUpdatedGameModelAsync();
    }

    private async Task SendUpdatedGameModelAsync()
    {
        foreach (var user in _connections.Connections)
        {
            var gameUser = _game.Lobby.Players.FirstOrDefault(p => p.Id == user.Value.Id);

            if (gameUser is null)
                continue;

            var gameModel = new GameModel
            {
                LocalPlayerState = GetPlayerState(gameUser),
                Cards = gameUser.Cards.Select(CardModel.FromEntity),
                JudgeId = _game.JudgePlayerId,
                LocalPlayerId = gameUser.Id,
                Template = TemplateModel.FromEntity(_game.TemplateCard),
                Players = _game.Lobby.Players.Select(PlayerModel.FromEntity)
            };

            await Clients.Client(user.Key).SendAsync("GameUpdated", gameModel);
        }
    }

    private PlayerState GetPlayerState(Player p)
    {
        if (_game.GameState == GameState.Waiting)
            return PlayerState.InLobby;

        return PlayerState.Playing;
    }

    public async Task Leave(Guid id)
    {
        if (!_connections.ConnectionExists(Context.ConnectionId))
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

        await Clients.All.SendAsync("LobbyUpdated", new LobbyModel(_game.Lobby));
    }

    public async Task GetPlayers()
    {
        await Clients.Caller.SendAsync("PlayerList", _game.Lobby.Players.Select(p => PlayerModel.FromEntity));
    }

    public async Task StartGame()
    {
        _game.StartGame();

        //await Clients.All.SendAsync("GameStarted", new GameModel { JudgeId = _game.JudgePlayerId });
        //await Clients.All.SendAsync("UpdateTemplate", _game.TemplateCard.Content, _game.TemplateCard.BlankCount);

        foreach (var user in _connections.Connections)
        {
            var gameUser = _game.Lobby.Players.FirstOrDefault(p => p.Id == user.Value.Id);
            //await Clients.Client(user.Key).SendAsync("UpdateCards", gameUser.Cards.Select(c => new CardModel(c.Id, c.Text)));
        }

        await SendUpdatedGameModelAsync();
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
        await Clients.Caller.SendAsync("UpdateTemplate", _game.TemplateCard.Content, _game.TemplateCard.BlankCount);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);

        if (!_connections.ConnectionExists(Context.ConnectionId))
        {
            // User wasn't registered...
            return;
        }

        var id = _connections.GetByConnectionId(Context.ConnectionId).Id;
        _game.Lobby.RemovePlayer(id);
        _connections.RemoveConnection(Context.ConnectionId);

        //await Clients.Others.SendAsync("PlayerLeft", id);
        await Clients.All.SendAsync("LobbyUpdated", new LobbyModel(_game.Lobby));
    }
}
