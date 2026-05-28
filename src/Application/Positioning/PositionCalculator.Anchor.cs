using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Primitives;

namespace Omen.Controls.Popup.Application.Positioning;

public static partial class PositionCalculator
{
    /// <summary>
    /// Determines the anchor point (reference X,Y) on the target based on the anchor target and optional rectangle.
    /// </summary>
    private static Point GetAnchorPoint(PopupRequest request, Rectangle? anchorRect, Size popupSize)
    {
        return request.AnchorTarget switch
        {
            AnchorTarget.UiElement when anchorRect.HasValue => GetAnchorFromRect(anchorRect.Value, request.Alignment),
            AnchorTarget.MouseCursor when anchorRect.HasValue => new Point(anchorRect.Value.X, anchorRect.Value.Y),
            AnchorTarget.CustomCoordinates when request.CustomX.HasValue && request.CustomY.HasValue
                => new Point(request.CustomX.Value, request.CustomY.Value),
            AnchorTarget.ParentContainer => throw new InvalidOperationException("ParentContainer should be handled separately."),
            _ => new Point(0, 0)
        };
    }

    /// <summary>
    /// Returns the anchor point on a rectangle (e.g., UI element) based on the desired alignment.
    /// </summary>
    private static Point GetAnchorFromRect(Rectangle rect, PopupAlignment alignment)
    {
        return alignment switch
        {
            PopupAlignment.TopLeft => new Point(rect.Left, rect.Top),
            PopupAlignment.TopCenter => new Point(rect.Left + rect.Width / 2, rect.Top),
            PopupAlignment.TopRight => new Point(rect.Right, rect.Top),
            PopupAlignment.LeftCenter => new Point(rect.Left, rect.Top + rect.Height / 2),
            PopupAlignment.MiddleCenter => new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2),
            PopupAlignment.RightCenter => new Point(rect.Right, rect.Top + rect.Height / 2),
            PopupAlignment.BottomLeft => new Point(rect.Left, rect.Bottom),
            PopupAlignment.BottomCenter => new Point(rect.Left + rect.Width / 2, rect.Bottom),
            PopupAlignment.BottomRight => new Point(rect.Right, rect.Bottom),
            _ => new Point(rect.Left, rect.Bottom) // fallback
        };
    }
}