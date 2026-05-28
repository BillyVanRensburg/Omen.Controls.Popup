using System;
using System.Collections.Generic;
using System.Text;

namespace Omen.Controls.Popup.Core.Enums;

/// <summary>
/// Defines the easing (acceleration/deceleration) curves for popup animations.
/// Used for both enter and exit animations to control the smoothness of transitions.
/// </summary>
public enum EasingType
{
    /// <summary>
    /// Linear easing – constant speed, no acceleration or deceleration.
    /// </summary>
    Linear,

    /// <summary>
    /// Ease in – animation starts slowly and then accelerates.
    /// </summary>
    EaseIn,

    /// <summary>
    /// Ease out – animation starts fast and then decelerates.
    /// </summary>
    EaseOut,

    /// <summary>
    /// Ease in and out – animation starts slowly, accelerates, then decelerates.
    /// </summary>
    EaseInOut,

    /// <summary>
    /// Cubic Bezier – custom easing curve defined by four control points.
    /// Additional parameters (e.g., X1,Y1,X2,Y2) are required to define the curve.
    /// </summary>
    CubicBezier
}