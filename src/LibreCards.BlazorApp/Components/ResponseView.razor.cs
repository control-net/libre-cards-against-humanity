using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp.Components;

public partial class ResponseView
{
    [Parameter]
    public LocalGameState? GameState { get; set; }

    [Parameter]
    public HubConnection? HubConnection { get; set; }

    private async ValueTask SubmitWinnerAsync(int id)
    {
        await HubConnection.SendAsync("PickResponse", id);
    }
}
