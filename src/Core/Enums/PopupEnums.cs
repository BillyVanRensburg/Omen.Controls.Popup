namespace Omen.Controls.Popup.Core.Enums;

/// <summary>What the popup anchors to.</summary>
public enum AnchorTarget
{
    /// <summary>A specific UI element (passed as object).</summary>
    UiElement,
    /// <summary>Current mouse cursor position.</summary>
    MouseCursor,
    /// <summary>Screen edge (top, bottom, left, or right).</summary>
    ScreenEdge,
    /// <summary>Center of the parent window.</summary>
    ParentWindowCenter,
    /// <summary>Absolute X/Y coordinates (provided separately).</summary>
    CustomCoordinates
}

/// <summary>Alignment of the popup relative to the anchor point.</summary>
public enum PopupAlignment
{
    TopLeft, TopCenter, TopRight,
    LeftCenter, Center, RightCenter,
    BottomLeft, BottomCenter, BottomRight
}

/// <summary>Animation types (can be combined).</summary>
[Flags]
public enum AnimationType
{
    None = 0,
    Fade = 1,
    Scale = 2,
    SlideFromTop = 4,
    SlideFromBottom = 8,
    SlideFromLeft = 16,
    SlideFromRight = 32
}

/// <summary>Easing functions for animations.</summary>
public enum EasingType
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    CubicBezier // custom bezier parameters would be extra
}

/// <summary>Result of a modal dialog.</summary>
public enum DialogResult
{
    None,
    OK,
    Cancel,
    Yes,
    No
}

/// <summary>Screen edge for anchoring.</summary>
public enum ScreenEdge
{
    Top,
    Bottom,
    Left,
    Right
}