namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Represents the configuration and content for showing a popup.
/// This partial class contains core properties.
/// </summary>
public partial class PopupRequest
{
    /// <summary>
    /// Gets or sets the content to display inside the popup.
    /// Can be any object (string, UIElement, UserControl). The platform host is responsible for rendering it.
    /// </summary>
    public object? Content { get; set; }

    /// <summary>
    /// Gets or sets whether the popup is modal.
    /// When <c>true</c>, an overlay covers the parent container and blocks interaction.
    /// </summary>
    public bool IsModal { get; set; }

    /// <summary>
    /// Gets or sets whether clicking outside the popup closes it.
    /// For non‑modal popups only.
    /// </summary>
    public bool CloseOnOutsideClick { get; set; } = true;
}