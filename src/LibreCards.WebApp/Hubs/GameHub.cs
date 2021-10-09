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
            await SendError("The user already received a GUID");
            return;
        }

        if (_connections.UsernameIsTaken(username))
        {
            await SendError($"Someone already has the same username ({username})");
            return;
        }

        var id = Guid.NewGuid();
        var model = new PlayerModel
        {
            Id = id,
            Username = username
        };

        _connections.AddConnection(connId, model);

        await ExecuteSafelyAsync(() =>
            _game.Lobby.AddPlayer(new Player(id)
            {
                Username = username
            }));

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
                LocalPlayerState = GetPlayerState(),
                Cards = gameUser.Cards.Select(CardModel.FromEntity),
                JudgeId = _game.JudgePlayerId,
                LocalPlayerId = gameUser.Id,
                Template = TemplateModel.FromEntity(_game.TemplateCard),
                Players = _game.Lobby.Players.Select(PlayerModel.FromEntity)
            };

            await Clients.Client(user.Key).SendAsync("GameUpdated", gameModel);
        }
    }

    private PlayerState GetPlayerState()
    {
        if (_game.GameState == GameState.Waiting)
            return PlayerState.InLobby;

        if (_game.GameState == GameState.Playing)
            return PlayerState.Playing;

        return PlayerState.Judging;
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
        var id = _connections.GetByConnectionId(Context.ConnectionId).Id;
        await ExecuteSafelyAsync(() => _game.StartGame(id));
        await SendUpdatedGameModelAsync();
    }

    public async Task PlayCards(int[] cardIds)
    {
        var id = _connections.GetByConnectionId(Context.ConnectionId).Id;
        await ExecuteSafelyAsync(() => _game.PlayCards(id, cardIds));
        await SendUpdatedGameModelAsync();
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

        await Clients.All.SendAsync("LobbyUpdated", new LobbyModel(_game.Lobby));
    }

    private async Task ExecuteSafelyAsync(Func<Task> task)
    {
        try
        {
            await task();
        }
        catch (Exception e)
        {
            await SendError(e.Message);
        }
    }

    private Task ExecuteSafelyAsync(Action task)
    {
        return ExecuteSafelyAsync(() =>
        {
            task?.Invoke();
            return Task.CompletedTask;
        });
    }

    private Task SendError(string m) => Clients.Caller.SendAsync("Exception", m);
}
