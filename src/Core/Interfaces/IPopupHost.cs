using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Events;
using Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Core.Interfaces;

/// <summary>
/// Defines the contract for platform‑specific popup hosts (e.g., WPF, MAUI, Blazor).
/// This interface lives in the Core layer and has no UI framework dependencies,
/// allowing the Application layer to remain platform‑agnostic.
/// All methods and events are asynchronous to avoid UI thread blocking.
/// </summary>
public interface IPopupHost
{
    /// <summary>
    /// Asynchronously shows the popup with the given configuration.
    /// The task completes when the popup is fully open and any enter animation has finished.
    /// </summary>
    /// <param name="request">Configuration and content for the popup.</param>
    /// <param name="cancellationToken">Token to cancel the showing operation (if supported).</param>
    /// <returns>A task that completes when the popup is open.</returns>
    Task ShowAsync(PopupRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously closes the popup.
    /// The task completes when the popup is fully closed and any exit animation has finished.
    /// </summary>
    /// <returns>A task that completes when the popup is closed.</returns>
    Task CloseAsync();

    /// <summary>
    /// Asynchronously updates the popup's position (e.g., when the anchor element moves or the screen size changes).
    /// </summary>
    /// <returns>A task that completes when the repositioning is done.</returns>
    Task UpdatePositionAsync();

    /// <summary>
    /// Occurs just before the popup opens. Event handlers can cancel the opening by setting
    /// <see cref="CancellableEventArgs.Cancel"/> to <c>true</c>.
    /// Handlers may be asynchronous; the host awaits all handlers before proceeding.
    /// </summary>
    event Func<CancellableEventArgs, Task> Opening;

    /// <summary>
    /// Occurs after the popup has fully opened (including any enter animation).
    /// Handlers may be asynchronous.
    /// </summary>
    event Func<Task> Opened;

    /// <summary>
    /// Occurs just before the popup closes. Event handlers can cancel the closing by setting
    /// <see cref="CancellableEventArgs.Cancel"/> to <c>true</c>.
    /// Handlers may be asynchronous.
    /// </summary>
    event Func<CancellableEventArgs, Task> Closing;

    /// <summary>
    /// Occurs after the popup has fully closed (including any exit animation), providing:
    /// <list type="bullet">
    ///   <item><description>The <see cref="DialogAction"/> taken by the user (e.g., OK, Cancel). For non‑dialog popups, this may be <c>null</c>.</description></item>
    ///   <item><description>An optional result object containing any data entered or selected in the popup (e.g., text from a <c>TextBox</c>). For simple popups, this is typically <c>null</c>.</description></item>
    /// </list>
    /// Handlers may be asynchronous.
    /// </summary>
    event Func<DialogAction?, object?, Task> Closed;
}