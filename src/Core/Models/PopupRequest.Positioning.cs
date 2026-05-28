using Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Represents the configuration and content for showing a popup.
/// This partial class contains positioning properties.
/// </summary>
public partial class PopupRequest
{
    /// <summary>
    /// Gets or sets the target to which the popup is anchored.
    /// Determines the reference point for positioning.
    /// </summary>
    public AnchorTarget AnchorTarget { get; set; }

    /// <summary>
    /// Gets or sets the alignment of the popup relative to the anchor point.
    /// </summary>
    public PopupAlignment Alignment { get; set; } = PopupAlignment.BottomCenter;

    /// <summary>
    /// Gets or sets the offset in pixels (X, Y) from the anchor point.
    /// </summary>
    public (int X, int Y) Offset { get; set; }

    /// <summary>
    /// Gets or sets whether the popup automatically flips to the opposite side if it would go off‑screen.
    /// </summary>
    public bool AutoFlip { get; set; } = true;

    /// <summary>
    /// Gets or sets the custom X coordinate (used when <see cref="AnchorTarget"/> is <see cref="AnchorTarget.CustomCoordinates"/>).
    /// </summary>
    public double? CustomX { get; set; }

    /// <summary>
    /// Gets or sets the custom Y coordinate (used when <see cref="AnchorTarget"/> is <see cref="AnchorTarget.CustomCoordinates"/>).
    /// </summary>
    public double? CustomY { get; set; }
}