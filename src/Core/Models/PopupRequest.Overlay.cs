namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Represents the configuration and content for showing a popup.
/// This partial class contains overlay properties used only for modal popups.
/// </summary>
public partial class PopupRequest
{
    /// <summary>
    /// Gets or sets the overlay brush color (e.g., "#80000000").
    /// Only used when <see cref="IsModal"/> is <c>true</c>.
    /// </summary>
    public string? OverlayBrush { get; set; } = "#80000000";

    /// <summary>
    /// Gets or sets whether clicking the overlay closes the popup.
    /// Only applicable when <see cref="IsModal"/> is <c>true</c>.
    /// </summary>
    public bool CloseOnOverlayClick { get; set; } = true;
}