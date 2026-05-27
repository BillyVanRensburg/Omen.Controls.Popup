namespace Omen.Controls.Popup.Core.Events;

/// <summary>Event args that allow cancellation.</summary>
public class CancellableEventArgs : EventArgs
{
    /// <summary>If true, the operation (opening or closing) is aborted.</summary>
    public bool Cancel { get; set; }
}