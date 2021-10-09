using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace LibreCards.BlazorApp.Components;

public partial class CardDrawer
{
    [Parameter]
    public LocalGameState? GameState { get; set; }

    [Parameter]
    public HubConnection? HubConnection { get; set; }

    private readonly List<int> selectedCardIds = new();

    private async ValueTask SubmitCardsAsync()
    {
        Console.WriteLine($"Sending cards: {string.Join(", ", selectedCardIds)}");
        await HubConnection.SendAsync("PlayCards", selectedCardIds);
    }

    private void ToggleCardSelection(ChangeEventArgs args, int cardId)
    {
        if (args.Value is null)
            return;

        Console.WriteLine($"Toggle card with ID {cardId}");

        bool IsSelected = (bool)args.Value;
        
        if (IsSelected)
            selectedCardIds.Add(cardId);
        else
            selectedCardIds.Remove(cardId);
    }
}
