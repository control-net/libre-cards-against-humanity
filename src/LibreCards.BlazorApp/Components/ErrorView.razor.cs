using Microsoft.AspNetCore.Components;

namespace LibreCards.BlazorApp.Components;

public partial class ErrorView
{
    [Parameter]
    public string? ErrorMessage {  get; set; }
}
