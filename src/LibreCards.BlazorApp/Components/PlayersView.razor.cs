using Microsoft.AspNetCore.Components;

namespace LibreCards.BlazorApp.Components;

public partial class PlayersView
{
    [Parameter]
    public LocalGameState? GameState { get; set; }
}
