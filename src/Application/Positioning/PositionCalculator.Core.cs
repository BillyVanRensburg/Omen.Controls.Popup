using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Primitives;

namespace Omen.Controls.Popup.Application.Positioning;

/// <summary>
/// Calculates final screen coordinates based on anchor target, alignment, offset, and auto‑flip.
/// This class is platform‑agnostic and uses Core primitives.
/// </summary>
public static partial class PositionCalculator
{
    /// <summary>
    /// Computes the popup's top‑left position.
    /// </summary>
    /// <param name="request">Popup configuration (anchor target, alignment, offset, auto‑flip).</param>
    /// <param name="anchorRect">Screen rectangle of the anchor (e.g., UI element bounds or mouse point). May be <c>null</c> for some targets.</param>
    /// <param name="popupSize">Size of the popup after layout.</param>
    /// <param name="containerBounds">Bounds of the container (e.g., parent window or screen) used for centering and overflow checks.</param>
    /// <returns>Calculated top‑left coordinates.</returns>
    public static Point CalculatePosition(
        PopupRequest request,
        Rectangle? anchorRect,
        Size popupSize,
        Rectangle containerBounds)
    {
        Point position;

        // Special case: ParentContainer – center within the container bounds
        if (request.AnchorTarget == AnchorTarget.ParentContainer)
        {
            double x = containerBounds.Left + (containerBounds.Width - popupSize.Width) / 2;
            double y = containerBounds.Top + (containerBounds.Height - popupSize.Height) / 2;
            position = new Point(x, y);
            if (request.AutoFlip)
                position = ApplyAutoFlip(position, popupSize, containerBounds);
            return position;
        }

        // Determine the anchor point (reference point) on the target
        var anchorPoint = GetAnchorPoint(request, anchorRect, popupSize);

        // Align the popup relative to the anchor point
        position = AlignPopup(anchorPoint, popupSize, request.Alignment, request.Offset);

        // Apply auto‑flip to keep the popup inside the container bounds
        if (request.AutoFlip)
            position = ApplyAutoFlip(position, popupSize, containerBounds);

        return position;
    }
}