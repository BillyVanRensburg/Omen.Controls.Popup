using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CorePoint = Omen.Controls.Popup.Core.Primitives.Point;
using CoreSize = Omen.Controls.Popup.Core.Primitives.Size;
using CoreRect = Omen.Controls.Popup.Core.Primitives.Rectangle;
using Omen.Controls.Popup.Application.Positioning;
using Omen.Controls.Popup.Core.Models;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing positioning helper methods for both modal and lightweight popups.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Gets the bounding rectangle of the parent window (or the main window if this control is not yet parented).
    /// Used for centering modal popups within their parent window.
    /// </summary>
    /// <returns>A <see cref="CoreRect"/> representing the parent window's screen position and size.
    /// Falls back to full screen bounds if no parent window is found.</returns>
    private CoreRect GetParentWindowBounds()
    {
        var window = Window.GetWindow(this);
        if (window == null) return GetScreenBounds(); // fallback to screen
        return new CoreRect(window.Left, window.Top, window.ActualWidth, window.ActualHeight);
    }

    /// <summary>
    /// Gets the bounding rectangle of the primary screen.
    /// Used as a fallback when no parent window is available or for screen‑edge anchoring.
    /// </summary>
    /// <returns>A <see cref="CoreRect"/> representing the full screen dimensions.</returns>
    private static CoreRect GetScreenBounds()
    {
        var width = SystemParameters.PrimaryScreenWidth;
        var height = SystemParameters.PrimaryScreenHeight;
        return new CoreRect(0, 0, width, height);
    }

    /// <summary>
    /// Custom placement callback for the lightweight popup when anchored to a UI element.
    /// Calculates the popup's position using <see cref="PositionCalculator"/> and applies alignment, offset, and auto‑flip.
    /// </summary>
    /// <param name="popupSize">The desired size of the popup.</param>
    /// <param name="targetSize">The size of the placement target (the anchor element).</param>
    /// <param name="offset">The requested offset (unused; the request's own offset is used instead).</param>
    /// <returns>An array containing the calculated popup placement.</returns>
    private CustomPopupPlacement[] OnCustomPopupPlacement(Size popupSize, Size targetSize, Point offset)
    {
        var anchorRect = GetAnchorRect();
        var screenBounds = GetScreenBounds();
        var corePopupSize = new CoreSize(popupSize.Width, popupSize.Height);
        CoreRect? coreAnchorRect = anchorRect.HasValue ? new CoreRect(anchorRect.Value.X, anchorRect.Value.Y, anchorRect.Value.Width, anchorRect.Value.Height) : null;

        // Use common positioning properties (AnchorTarget, Alignment, OffsetX, OffsetY, AutoFlip)
        var request = new PopupRequest
        {
            AnchorTarget = AnchorTarget,
            Alignment = Alignment,
            Offset = (OffsetX, OffsetY),
            AutoFlip = AutoFlip,
        };

        var position = PositionCalculator.CalculatePosition(request, coreAnchorRect, corePopupSize, screenBounds);
        double x = position.X;
        double y = position.Y;

        if (AnchorElement != null)
        {
            var targetPoint = AnchorElement.PointToScreen(new System.Windows.Point(0, 0));
            x -= targetPoint.X;
            y -= targetPoint.Y;
        }
        return [new CustomPopupPlacement(new System.Windows.Point(x, y), PopupPrimaryAxis.None)];
    }

    /// <summary>
    /// Measures the desired size of a framework element with infinite constraints.
    /// </summary>
    /// <param name="element">The element to measure.</param>
    /// <returns>The desired size of the element.</returns>
    private static Size MeasureContentSize(FrameworkElement element)
    {
        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return element.DesiredSize;
    }

    /// <summary>
    /// Gets the screen rectangle of the anchor element (if set) or the current mouse cursor position.
    /// Used for lightweight popup placement.
    /// </summary>
    /// <returns>
    /// A <see cref="CoreRect"/> representing the anchor's screen bounds, or a 1x1 rectangle at the mouse position
    /// if no anchor element is set.
    /// </returns>
    private CoreRect? GetAnchorRect()
    {
        if (AnchorElement != null)
        {
            var point = AnchorElement.PointToScreen(new System.Windows.Point(0, 0));
            var size = new System.Windows.Size(AnchorElement.ActualWidth, AnchorElement.ActualHeight);
            return new CoreRect(point.X, point.Y, size.Width, size.Height);
        }
        var mainWindow = System.Windows.Application.Current.MainWindow;
        var mousePoint = Mouse.GetPosition(mainWindow);
        var mouseScreen = mainWindow.PointToScreen(mousePoint);
        return new CoreRect(mouseScreen.X, mouseScreen.Y, 1, 1);
    }
}