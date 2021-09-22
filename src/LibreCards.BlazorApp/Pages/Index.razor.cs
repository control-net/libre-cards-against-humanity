using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp.Pages;

public partial class Index : IAsyncDisposable
{
    private string _usernameInput = string.Empty;

    private HubConnection? _hubConnection;

    private LocalGameState? _gameState;

    private string _statusMessage = "Connecting to a server...";

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/signalr/cardsgame")
            .Build();

        _gameState = new LocalGameState(_hubConnection);
        _gameState.GameStateChanged += (s, args) => StateHasChanged();

        await _hubConnection.StartAsync();

        await _gameState.InitializeAsync();

        _statusMessage = "Hub Connection established";
        StateHasChanged();
    }

    private async ValueTask JoinGameAsync()
    {
        await _hubConnection.SendAsync("Join", _usernameInput);
    }

    private async ValueTask StartGameAsync()
    {
        await _hubConnection.SendAsync("StartGame");
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
