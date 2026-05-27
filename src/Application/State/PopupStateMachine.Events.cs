using Omen.Controls.Popup.Core.Events;

namespace Omen.Controls.Popup.Application.State;

public partial class PopupStateMachine
{
    /// <summary>Raised before opening (can be cancelled).</summary>
    public event Func<CancellableEventArgs, Task>? Opening;
    /// <summary>Raised after the popup is fully open.</summary>
    public event Func<Task>? Opened;
    /// <summary>Raised before closing (can be cancelled).</summary>
    public event Func<CancellableEventArgs, Task>? Closing;
    /// <summary>Raised after the popup is fully closed.</summary>
    public event Func<Task>? Closed;

    private async Task OnOpeningAsync(CancellableEventArgs args)
    {
        if (Opening != null)
            await Opening(args);
    }

    private async Task OnOpenedAsync()
    {
        if (Opened != null)
            await Opened();
    }

    private async Task OnClosingAsync(CancellableEventArgs args)
    {
        if (Closing != null)
            await Closing(args);
    }

    private async Task OnClosedAsync()
    {
        if (Closed != null)
            await Closed();
    }
}