using Microsoft.AspNetCore.Components;

namespace LibreCards.BlazorApp.Components
{
    public partial class ErrorDialog : ComponentBase
    {
        [Parameter]
        public string? ErrorMessage { get; set; }

        [Parameter]
        public EventCallback<string> ErrorMessageChanged { get; set; }

        public void Close()
        {
            ErrorMessage = null;
            StateHasChanged();
        }
    }
}
