using LibreCards.BlazorApp.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp.Pages;

public partial class Index : IAsyncDisposable
{
    private string _usernameInput = string.Empty;

    private HubConnection? _hubConnection;

    private LocalGameState? _state;

    private string _statusMessage = "Connecting to a server...";

    private string? _errMessage;

    private string _customUrl = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/signalr/cardsgame")
            .Build();

        _state = new LocalGameState(_hubConnection);
        _state.GameStateChanged += (s, args) => StateHasChanged();
        _state.OnError += (s, msg) => _errMessage = msg;

        await _hubConnection.StartAsync();

        await _state.InitializeAsync();

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

    private async ValueTask AddCards()
    {
        await _hubConnection.SendAsync("ImportCards", _customUrl);
    }
}
