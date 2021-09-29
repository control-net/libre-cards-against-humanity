using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp;
public class LocalGameState
{
    public event EventHandler? GameStateChanged;

    public event EventHandler<string>? OnError;

    private readonly HubConnection _connection;

    public LobbyModel? Lobby { get; private set; }

    public GameModel Game { get; private set; }

    public LocalGameState(HubConnection connection)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        _connection = connection;

        _connection.On<LobbyModel>("LobbyUpdated", OnLobbyUpdated);
        _connection.On<GameModel>("GameUpdated", OnGameUpdated);
        _connection.On<string>("Exception", OnException);

        Game = new GameModel();
    }

    private void OnException(string message)
    {
        OnError?.Invoke(this, message);
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    internal async ValueTask InitializeAsync()
    {
        Lobby = await _connection.InvokeAsync<LobbyModel>("GetLobbyState");
    }

    private void OnLobbyUpdated(LobbyModel lobbyModel)
    {
        Lobby = lobbyModel;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnGameUpdated(GameModel gameModel)
    {
        Game = gameModel;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
