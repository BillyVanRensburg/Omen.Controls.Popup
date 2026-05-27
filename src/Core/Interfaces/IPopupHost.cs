using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Events;

namespace Omen.Controls.Popup.Core.Interfaces;

/// <summary>
/// Platform-specific host for showing and controlling a popup.
/// Implemented by WPF, MAUI, Blazor, etc.
/// </summary>
public interface IPopupHost
{
    /// <summary>
    /// Shows the popup with the specified configuration.
    /// </summary>
    /// <param name="request">Configuration and content for the popup.</param>
    /// <param name="cancellationToken">Cancellation token to abort showing.</param>
    /// <returns>A task that completes when the popup is fully open.</returns>
    Task ShowAsync(PopupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes the popup.
    /// </summary>
    /// <returns>A task that completes when the popup is fully closed.</returns>
    Task CloseAsync();

    /// <summary>
    /// Updates the popup's position (e.g., when target moves or screen changes).
    /// </summary>
    Task UpdatePositionAsync();

    /// <summary>
    /// Event raised when the popup is about to open (cancellable).
    /// </summary>
    event Func<CancellableEventArgs, Task> Opening;

    /// <summary>
    /// Event raised after the popup has opened.
    /// </summary>
    event Func<Task> Opened;

    /// <summary>
    /// Event raised when the popup is about to close (cancellable).
    /// </summary>
    event Func<CancellableEventArgs, Task> Closing;

    /// <summary>
    /// Event raised after the popup has closed.
    /// </summary>
    event Func<Task> Closed;
}