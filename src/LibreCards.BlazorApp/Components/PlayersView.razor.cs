using LibreCards.Core.Entities.Client;
using Microsoft.AspNetCore.Components;

namespace LibreCards.BlazorApp.Components;

public partial class PlayersView
{
    public PlayersView()
    {
        Players = new List<PlayerModel>();
    }

    [Parameter]
    public IEnumerable<PlayerModel> Players { get; set; }
}
