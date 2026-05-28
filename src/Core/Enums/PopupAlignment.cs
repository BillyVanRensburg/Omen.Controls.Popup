using System;
using System.Collections.Generic;
using System.Text;

namespace Omen.Controls.Popup.Core.Enums;

/// <summary>
/// Defines how the popup is aligned relative to the anchor point.
/// Combines vertical and horizontal alignment.
/// </summary>
public enum PopupAlignment
{
    /// <summary>Top‑left corner of the popup aligns with the anchor point.</summary>
    TopLeft,

    /// <summary>Top‑center of the popup aligns with the anchor point.</summary>
    TopCenter,

    /// <summary>Top‑right corner of the popup aligns with the anchor point.</summary>
    TopRight,

    /// <summary>Left‑center of the popup aligns with the anchor point.</summary>
    LeftCenter,

    /// <summary>Both axes center – the popup's center aligns with the anchor point.</summary>
    MiddleCenter,

    /// <summary>Right‑center of the popup aligns with the anchor point.</summary>
    RightCenter,

    /// <summary>Bottom‑left corner of the popup aligns with the anchor point.</summary>
    BottomLeft,

    /// <summary>Bottom‑center of the popup aligns with the anchor point.</summary>
    BottomCenter,

    /// <summary>Bottom‑right corner of the popup aligns with the anchor point.</summary>
    BottomRight
}