using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Primitives;

namespace Omen.Controls.Popup.Application.Positioning;

public static partial class PositionCalculator
{
    /// <summary>
    /// Offsets the anchor point to the popup's top‑left corner based on the specified alignment and offset.
    /// </summary>
    /// <param name="anchor">The anchor point (reference X,Y).</param>
    /// <param name="popupSize">The size of the popup.</param>
    /// <param name="alignment">How the popup aligns relative to the anchor point.</param>
    /// <param name="offset">Additional pixel offset (X,Y).</param>
    /// <returns>The top‑left position of the popup.</returns>
    private static Point AlignPopup(Point anchor, Size popupSize, PopupAlignment alignment, (int X, int Y) offset)
    {
        double x = anchor.X;
        double y = anchor.Y;

        switch (alignment)
        {
            case PopupAlignment.TopLeft:
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
            case PopupAlignment.MiddleCenter:
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
            default:
                break;
        }

        x += offset.X;
        y += offset.Y;

        return new Point(x, y);
    }
}