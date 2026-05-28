using System;
using System.Collections.Generic;
using System.Text;

namespace Omen.Controls.Popup.Core.Enums;

/// <summary>
/// Defines the animation types that can be applied to popup enter and exit transitions.
/// These values can be combined using bitwise OR (e.g., Fade | Scale).
/// The naming is direction‑neutral: "SlideTop" means vertical movement toward/from the top,
/// and the actual direction (from or to) is determined by the animation context (enter or exit).
/// </summary>
[Flags]
public enum AnimationType
{
    /// <summary>No animation – the popup appears/disappears instantly.</summary>
    None = 0,

    /// <summary>Fade in/out (opacity transition).</summary>
    Fade = 1,

    /// <summary>Scale in/out (zoom from/to 0.8 to 1.0).</summary>
    Scale = 2,

    /// <summary>Vertical slide from/to the top edge.</summary>
    SlideTop = 4,

    /// <summary>Vertical slide from/to the bottom edge.</summary>
    SlideBottom = 8,

    /// <summary>Horizontal slide from/to the left edge.</summary>
    SlideLeft = 16,

    /// <summary>Horizontal slide from/to the right edge.</summary>
    SlideRight = 32
}
