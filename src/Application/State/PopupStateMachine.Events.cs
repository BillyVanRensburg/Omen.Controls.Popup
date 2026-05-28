using Omen.Controls.Popup.Core.Events;

namespace Omen.Controls.Popup.Application.State;

/// <summary>
/// Partial class of <see cref="PopupStateMachine"/> that defines and raises lifecycle events.
/// Events are asynchronous, allowing handlers to perform async operations and (for cancellable events)
/// to cancel the operation by setting <see cref="CancellableEventArgs.Cancel"/> to <c>true</c>.
/// </summary>
public partial class PopupStateMachine
{
    /// <summary>
    /// Occurs just before the popup opens. Handlers can cancel the opening by setting
    /// <see cref="CancellableEventArgs.Cancel"/> to <c>true</c>.
    /// </summary>
    public event Func<CancellableEventArgs, Task>? Opening;

    /// <summary>
    /// Occurs after the popup has fully opened (including any enter animation).
    /// </summary>
    public event Func<Task>? Opened;

    /// <summary>
    /// Occurs just before the popup closes. Handlers can cancel the closing by setting
    /// <see cref="CancellableEventArgs.Cancel"/> to <c>true</c>.
    /// </summary>
    public event Func<CancellableEventArgs, Task>? Closing;

    /// <summary>
    /// Occurs after the popup has fully closed (including any exit animation).
    /// </summary>
    public event Func<Task>? Closed;

    /// <summary>
    /// Invokes the <see cref="Opening"/> event asynchronously.
    /// </summary>
    /// <param name="args">Event arguments that allow cancellation.</param>
    private async Task OnOpeningAsync(CancellableEventArgs args)
    {
        if (Opening != null)
            await Opening(args);
    }

    /// <summary>
    /// Invokes the <see cref="Opened"/> event asynchronously.
    /// </summary>
    private async Task OnOpenedAsync()
    {
        if (Opened != null)
            await Opened();
    }

    /// <summary>
    /// Invokes the <see cref="Closing"/> event asynchronously.
    /// </summary>
    /// <param name="args">Event arguments that allow cancellation.</param>
    private async Task OnClosingAsync(CancellableEventArgs args)
    {
        if (Closing != null)
            await Closing(args);
    }

    /// <summary>
    /// Invokes the <see cref="Closed"/> event asynchronously.
    /// </summary>
    private async Task OnClosedAsync()
    {
        if (Closed != null)
            await Closed();
    }
}