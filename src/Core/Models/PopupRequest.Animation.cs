using Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Represents the configuration and content for showing a popup.
/// This partial class contains animation properties.
/// </summary>
public partial class PopupRequest
{
    /// <summary>
    /// Gets or sets the animation type for the opening transition.
    /// </summary>
    public AnimationType EnterAnimation { get; set; } = AnimationType.Fade;

    /// <summary>
    /// Gets or sets the animation type for the closing transition.
    /// </summary>
    public AnimationType ExitAnimation { get; set; } = AnimationType.Fade;

    /// <summary>
    /// Gets or sets the duration of the enter animation in milliseconds.
    /// </summary>
    public int EnterDuration { get; set; } = 200;

    /// <summary>
    /// Gets or sets the duration of the exit animation in milliseconds.
    /// </summary>
    public int ExitDuration { get; set; } = 200;

    /// <summary>
    /// Gets or sets the easing function for the enter animation.
    /// </summary>
    public EasingType EnterEasing { get; set; } = EasingType.EaseOut;

    /// <summary>
    /// Gets or sets the easing function for the exit animation.
    /// </summary>
    public EasingType ExitEasing { get; set; } = EasingType.EaseIn;

    /// <summary>
    /// Gets or sets the cubic Bezier control points for the enter animation when <see cref="EnterEasing"/> is <see cref="EasingType.CubicBezier"/>.
    /// The format is a comma‑separated string of four double values: "X1,Y1,X2,Y2".
    /// Example: "0.25,0.1,0.25,1.0".
    /// </summary>
    public string? EnterCubicBezierPoints { get; set; }

    /// <summary>
    /// Gets or sets the cubic Bezier control points for the exit animation when <see cref="ExitEasing"/> is <see cref="EasingType.CubicBezier"/>.
    /// The format is a comma‑separated string of four double values: "X1,Y1,X2,Y2".
    /// Example: "0.25,0.1,0.25,1.0".
    /// </summary>
    public string? ExitCubicBezierPoints { get; set; }
}