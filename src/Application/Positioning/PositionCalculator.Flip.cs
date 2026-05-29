using Omen.Controls.Popup.Core.Primitives;

namespace Omen.Controls.Popup.Application.Positioning;

public static partial class PositionCalculator
{
    /// <summary>
    /// Adjusts the popup position to keep it fully inside the container bounds.
    /// If the popup would extend beyond the container's edges, it is moved inward.
    /// </summary>
    /// <param name="position">The initially calculated top‑left position.</param>
    /// <param name="popupSize">The size of the popup.</param>
    /// <param name="containerBounds">The bounds of the container (e.g., parent window or screen).</param>
    /// <returns>The adjusted top‑left position.</returns>
    private static Point ApplyAutoFlip(Point position, Size popupSize, Rectangle containerBounds)
    {
        double x = position.X;
        double y = position.Y;

        if (x + popupSize.Width > containerBounds.Right)
            x = containerBounds.Right - popupSize.Width;
        if (x < containerBounds.Left)
            x = containerBounds.Left;

        if (y + popupSize.Height > containerBounds.Bottom)
            y = containerBounds.Bottom - popupSize.Height;
        if (y < containerBounds.Top)
            y = containerBounds.Top;

        return new Point(x, y);
    }
}