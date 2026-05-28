namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Represents the configuration and content for showing a popup.
/// This partial class contains properties related to the close button.
/// </summary>
public partial class PopupRequest
{
    /// <summary>
    /// Gets or sets whether to show the close button.
    /// When <c>true</c>, a close button appears in the popup header (if the platform supports it).
    /// </summary>
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Gets or sets a platform‑specific data template for the close button.
    /// If <c>null</c>, a default template is used.
    /// </summary>
    public object? CloseButtonTemplate { get; set; }
}