using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Primitives;

namespace Omen.Controls.Popup.Application.Positioning;

/// <summary>
/// Calculates final screen coordinates based on anchor, alignment, offset, and auto‑flip.
/// </summary>
public static class PositionCalculator
{
    /// <summary>
    /// Computes the popup's position.
    /// </summary>
    /// <param name="request">Popup configuration.</param>
    /// <param name="anchorRect">Screen rectangle of the anchor (target), if any.</param>
    /// <param name="popupSize">Size of the popup after layout.</param>
    /// <param name="screenBounds">Available screen area.</param>
    /// <returns>Calculated top‑left coordinates.</returns>
    public static Point CalculatePosition(
        PopupRequest request,
        Rectangle? anchorRect,
        Size popupSize,
        Rectangle screenBounds)
    {
        // Step 1: Determine the anchor point (X,Y) on the target
        var anchorPoint = GetAnchorPoint(request, anchorRect, popupSize, screenBounds);

        // Step 2: Determine the popup's top-left based on alignment
        var position = AlignPopup(anchorPoint, popupSize, request.Alignment, request.Offset);

        // Step 3: Apply auto-flip if enabled and needed
        if (request.AutoFlip)
        {
            position = ApplyAutoFlip(position, popupSize, screenBounds, request);
        }

        return position;
    }

    private static Point GetAnchorPoint(PopupRequest request, Rectangle? anchorRect, Size popupSize, Rectangle screenBounds)
    {
        return request.AnchorTarget switch
        {
            AnchorTarget.UiElement when anchorRect.HasValue => GetAnchorFromRect(anchorRect.Value, request.Alignment),
            AnchorTarget.MouseCursor => GetMousePosition(),
            AnchorTarget.ScreenEdge => GetScreenEdgeAnchor(request, screenBounds, popupSize),
            AnchorTarget.ParentWindowCenter => GetWindowCenter(screenBounds),
            AnchorTarget.CustomCoordinates => new Point(request.CustomX ?? 0, request.CustomY ?? 0),
            _ => new Point(0, 0)
        };
    }

    private static Point GetAnchorFromRect(Rectangle rect, PopupAlignment alignment)
    {
        return alignment switch
        {
            PopupAlignment.TopLeft => new Point(rect.Left, rect.Top),
            PopupAlignment.TopCenter => new Point(rect.Left + rect.Width / 2, rect.Top),
            PopupAlignment.TopRight => new Point(rect.Right, rect.Top),
            PopupAlignment.LeftCenter => new Point(rect.Left, rect.Top + rect.Height / 2),
            PopupAlignment.Center => new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2),
            PopupAlignment.RightCenter => new Point(rect.Right, rect.Top + rect.Height / 2),
            PopupAlignment.BottomLeft => new Point(rect.Left, rect.Bottom),
            PopupAlignment.BottomCenter => new Point(rect.Left + rect.Width / 2, rect.Bottom),
            PopupAlignment.BottomRight => new Point(rect.Right, rect.Bottom),
            _ => new Point(rect.Left, rect.Bottom) // fallback
        };
    }

    private static Point GetMousePosition()
    {
        // Platform-specific: in a real app, this would get current mouse coordinates.
        // For now, we return (0,0) and note that the host should provide it.
        // We'll leave this as a stub; the host is responsible for passing the actual mouse position.
        return new Point(0, 0);
    }

    private static Point GetScreenEdgeAnchor(PopupRequest request, Rectangle screenBounds, Size popupSize)
    {
        // For simplicity, assume request.ScreenEdge is set via a custom property.
        // We'll use a default (Top) if not specified.
        var edge = request.ScreenEdge ?? ScreenEdge.Top;
        return edge switch
        {
            ScreenEdge.Top => new Point(screenBounds.Left + screenBounds.Width / 2, screenBounds.Top),
            ScreenEdge.Bottom => new Point(screenBounds.Left + screenBounds.Width / 2, screenBounds.Bottom),
            ScreenEdge.Left => new Point(screenBounds.Left, screenBounds.Top + screenBounds.Height / 2),
            ScreenEdge.Right => new Point(screenBounds.Right, screenBounds.Top + screenBounds.Height / 2),
            _ => new Point(screenBounds.Left + screenBounds.Width / 2, screenBounds.Top)
        };
    }

    private static Point GetWindowCenter(Rectangle screenBounds)
    {
        return new Point(screenBounds.Left + screenBounds.Width / 2, screenBounds.Top + screenBounds.Height / 2);
    }

    private static Point AlignPopup(Point anchor, Size popupSize, PopupAlignment alignment, (int X, int Y) offset)
    {
        double x = anchor.X;
        double y = anchor.Y;

        switch (alignment)
        {
            case PopupAlignment.TopLeft:
                // no shift
                break;
            case PopupAlignment.TopCenter:
                x -= popupSize.Width / 2;
                break;
            case PopupAlignment.TopRight:
                x -= popupSize.Width;
                break;
            case PopupAlignment.LeftCenter:
                y -= popupSize.Height / 2;
                break;
            case PopupAlignment.Center:
                x -= popupSize.Width / 2;
                y -= popupSize.Height / 2;
                break;
            case PopupAlignment.RightCenter:
                x -= popupSize.Width;
                y -= popupSize.Height / 2;
                break;
            case PopupAlignment.BottomLeft:
                y -= popupSize.Height;
                break;
            case PopupAlignment.BottomCenter:
                x -= popupSize.Width / 2;
                y -= popupSize.Height;
                break;
            case PopupAlignment.BottomRight:
                x -= popupSize.Width;
                y -= popupSize.Height;
                break;
        }

        // Apply offset
        x += offset.X;
        y += offset.Y;

        return new Point(x, y);
    }

    private static Point ApplyAutoFlip(Point position, Size popupSize, Rectangle screenBounds, PopupRequest request)
    {
        double x = position.X;
        double y = position.Y;

        // Check horizontal overflow
        if (x + popupSize.Width > screenBounds.Right)
        {
            // Flip horizontally: instead of aligning to the original anchor side, mirror
            // For simplicity, we just move left by overflow amount.
            x = screenBounds.Right - popupSize.Width;
        }
        if (x < screenBounds.Left)
        {
            x = screenBounds.Left;
        }

        // Check vertical overflow
        if (y + popupSize.Height > screenBounds.Bottom)
        {
            y = screenBounds.Bottom - popupSize.Height;
        }
        if (y < screenBounds.Top)
        {
            y = screenBounds.Top;
        }

        return new Point(x, y);
    }
}