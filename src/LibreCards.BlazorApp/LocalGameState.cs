using LibreCards.BlazorApp.Entities;
using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp;
public class LocalGameState
{
    public event EventHandler? GameStateChanged;

    public PlayerState LocalPlayerState { get; private set; }

    public Guid LocalPlayerId { get; private set; }

    public IEnumerable<PlayerModel> Players => _players;

    public IEnumerable<CardModel> Cards { get; private set; }

    public string TemplateCard { get; private set; }

    private List<PlayerModel> _players = new();

    private readonly HubConnection _connection;

    public LobbyModel LobbyModel { get; private set; }

    public LocalGameState(HubConnection connection)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));

        Cards = new List<CardModel>();
        TemplateCard = string.Empty;

        _connection = connection;

        connection.On<PlayerModel>("PlayerJoined", OnPlayerJoined);
        connection.On<Guid>("IdAssigned", OnIdAssigned);
        connection.On<List<PlayerModel>>("PlayerList", OnPlayerListReceived);
        connection.On<GameModel>("GameStarted", OnGameStarted);
        connection.On<string, int>("UpdateTemplate", OnUpdateTemplate);
        connection.On<IEnumerable<CardModel>>("UpdateCards", OnUpdateCards);

        connection.On<LobbyModel>("LobbyUpdated", OnLobbyUpdated);
    }

    private void OnLobbyUpdated(LobbyModel lobbyModel)
    {
        LobbyModel = lobbyModel;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnUpdateCards(IEnumerable<CardModel> cards)
    {
        Cards = cards;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnGameStarted(GameModel game)
    {
        // TODO: assign judge
        LocalPlayerState = PlayerState.Playing;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnUpdateTemplate(string template, int numOfSlots)
    {
        TemplateCard = template;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnPlayerListReceived(List<PlayerModel> ids)
    {
        _players = ids;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask InitializeAsync()
    {
        LocalPlayerState = PlayerState.NotInLobby;
        LobbyModel = await _connection.InvokeAsync<LobbyModel>("GetLobbyState");
    }

    private void OnIdAssigned(Guid id)
    {
        LocalPlayerId = id;
        LocalPlayerState = PlayerState.InLobby;
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnPlayerJoined(PlayerModel player)
    {
        _players.Add(player);
        GameStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
