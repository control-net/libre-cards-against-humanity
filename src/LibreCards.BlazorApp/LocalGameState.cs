using LibreCards.BlazorApp.Entities;
using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp;
public class LocalGameState
{
    public event EventHandler? GameStateChanged;

    public PlayerState LocalPlayerState { get; private set; } = PlayerState.NotInLobby;

    public Guid LocalPlayerId { get; private set; }

    public IEnumerable<Guid> Players => _players;

    public IEnumerable<string> Cards { get; private set; }

    public string TemplateCard { get; private set; }

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
        connection.On<GameModel>("GameStarted", OnGameStarted);
        connection.On<string, int>("UpdateTemplate", OnUpdateTemplate);
        connection.On<IEnumerable<string>>("UpdateCards", OnUpdateCards);
    }

    private void OnUpdateCards(IEnumerable<string> cards)
    {
        Cards = cards;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnGameStarted(GameModel game)
    {
        // TODO: assign judge
        LocalPlayerState = PlayerState.Playing;
        GameStateChanged?.Invoke(this, EventArgs.Empty);

        _ = _connection.SendAsync("GetMyCards", LocalPlayerId);
    }

    public void OnUpdateTemplate(string template, int numOfSlots)
    {
        TemplateCard = template;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
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
