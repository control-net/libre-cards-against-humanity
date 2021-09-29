using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.Components;

namespace LibreCards.BlazorApp.Components;

public partial class LobbyView
{
    [Parameter]
    public LobbyModel? LobbyModel { get; set; }
}
