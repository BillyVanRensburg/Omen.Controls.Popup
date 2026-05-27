using Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Configuration and content for showing a popup.
/// </summary>
public class PopupRequest
{
    /// <summary>Content to display inside the popup (any object, platform will render).</summary>
    public object? Content { get; set; }

    /// <summary>Whether the popup is modal (blocks parent interaction and shows overlay).</summary>
    public bool IsModal { get; set; }

    /// <summary>Whether clicking outside the popup closes it (only for non‑modal).</summary>
    public bool CloseOnOutsideClick { get; set; } = true;

    /// <summary>Target to anchor the popup to (UI element, mouse, screen edge, etc.).</summary>
    public AnchorTarget AnchorTarget { get; set; }

    /// <summary>Alignment of the popup relative to the anchor point.</summary>
    public PopupAlignment Alignment { get; set; } = PopupAlignment.BottomCenter;

    /// <summary>Offset in pixels (X, Y) from the anchor point.</summary>
    public (int X, int Y) Offset { get; set; }

    /// <summary>Automatically flip popup if it goes off screen.</summary>
    public bool AutoFlip { get; set; } = true;

    /// <summary>Animation type for opening.</summary>
    public AnimationType EnterAnimation { get; set; } = AnimationType.Fade;

    /// <summary>Animation type for closing.</summary>
    public AnimationType ExitAnimation { get; set; } = AnimationType.Fade;

    /// <summary>Duration of enter animation in milliseconds.</summary>
    public int EnterDuration { get; set; } = 200;

    /// <summary>Duration of exit animation in milliseconds.</summary>
    public int ExitDuration { get; set; } = 200;

    /// <summary>Easing function for enter animation.</summary>
    public EasingType EnterEasing { get; set; } = EasingType.EaseOut;

    /// <summary>Easing function for exit animation.</summary>
    public EasingType ExitEasing { get; set; } = EasingType.EaseIn;

    /// <summary>Overlay color (e.g., "#80000000") – only used if IsModal is true.</summary>
    public string? OverlayBrush { get; set; } = "#80000000";

    /// <summary>Whether clicking the overlay closes the popup.</summary>
    public bool CloseOnOverlayClick { get; set; } = true;

    /// <summary>Data template for close button (platform‑specific).</summary>
    public object? CloseButtonTemplate { get; set; }

    /// <summary>For screen edge anchoring, which edge to use.</summary>
    public ScreenEdge? ScreenEdge { get; set; }

    /// <summary>Custom X coordinate (used when AnchorTarget is CustomCoordinates).</summary>
    public double? CustomX { get; set; }

    /// <summary>Custom Y coordinate (used when AnchorTarget is CustomCoordinates).</summary>
    public double? CustomY { get; set; }
}