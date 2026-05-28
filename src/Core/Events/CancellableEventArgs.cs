namespace Omen.Controls.Popup.Core.Events;

/// <summary>
/// Provides event arguments that allow the event handler to cancel the ongoing operation.
/// Used for cancellable popup events such as before opening and before closing.
/// </summary>
public class CancellableEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation should be cancelled.
    /// If set to <c>true</c>, the popup will not open or will not close.
    /// </summary>
    public bool Cancel { get; set; }
}