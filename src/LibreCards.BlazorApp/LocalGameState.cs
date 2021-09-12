using LibreCards.BlazorApp.Entities;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp;
public class LocalGameState
{
    public event EventHandler? GameStateChanged;

    public PlayerState LocalPlayerState { get; private set; } = PlayerState.NotInLobby;

    public Guid LocalPlayerId { get; private set; }

    public IEnumerable<Guid> Players => _players;

    private List<Guid> _players = new();

    private readonly HubConnection _connection;

    public LocalGameState(HubConnection connection)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        _connection = connection;

        connection.On<Guid>("PlayerJoined", OnPlayerJoined);
        connection.On<Guid>("IdAssigned", OnIdAssigned);
        connection.On<List<Guid>>("PlayerList", OnPlayerListReceived);
    }

    private void OnPlayerListReceived(List<Guid> ids)
    {
        _players = ids;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask InitializeAsync()
    {
        await _connection.SendAsync("GetPlayers");
    }

    private void OnIdAssigned(Guid id)
    {
        LocalPlayerId = id;
        LocalPlayerState = PlayerState.InLobby;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnPlayerJoined(Guid id)
    {
        _players.Add(id);
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
