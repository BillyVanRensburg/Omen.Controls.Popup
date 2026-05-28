using System;
using System.Collections.Generic;
using System.Text;

namespace Omen.Controls.Popup.Core.Enums;

/// <summary>
/// Defines the target to which a popup is anchored.
/// Used by both modal and lightweight popups to determine their position.
/// </summary>
public enum AnchorTarget
{
    /// <summary>
    /// Anchors the popup to a specific UI element (e.g., a button, text box).
    /// The element is provided separately via an element reference.
    /// </summary>
    UiElement,

    /// <summary>
    /// Anchors the popup to the current mouse cursor position.
    /// The popup will appear near the cursor.
    /// </summary>
    MouseCursor,

    /// <summary>
    /// Anchors the popup to the parent container (e.g., a Window in WPF, a Page in MAUI,
    /// or the browser viewport in Blazor).
    /// </summary>
    ParentContainer,

    /// <summary>
    /// Places the popup at absolute screen coordinates provided separately.
    /// No anchor element or container is used; the popup is positioned at a fixed (X,Y) point.
    /// </summary>
    CustomCoordinates
}