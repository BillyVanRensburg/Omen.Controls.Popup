using Omen.Controls.Popup.Application.Positioning;
using Omen.Controls.Popup.Core.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CoreEnums = Omen.Controls.Popup.Core.Enums;
using CorePoint = Omen.Controls.Popup.Core.Primitives.Point;
using CoreRect = Omen.Controls.Popup.Core.Primitives.Rectangle;
using CoreSize = Omen.Controls.Popup.Core.Primitives.Size;
using WpfPopup = System.Windows.Controls.Primitives.Popup;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing the ShowAsync method and its modal/lightweight implementations.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Shows the popup asynchronously, either as modal or lightweight based on <see cref="IsModal"/>.
    /// </summary>
    public async Task ShowAsync()
    {
        if (IsOpen) return;

        if (IsModal)
            await ShowModalAsync();
        else
            await ShowLightweightAsync();
    }

    // ------------------------------------------------------------
    // Modal (overlay) popup with positioning
    // ------------------------------------------------------------

    /// <summary>
    /// Displays the popup as a modal (overlay) popup, applying positioning based on <see cref="ModalAnchorTarget"/>,
    /// alignment, offset, and auto‑flip. Also handles the enter animation and initial transform states.
    /// </summary>
    private async Task ShowModalAsync()
    {
        EnsureTransformGroup(ModalContentBorder);

        // Animation start state (fade starts at 0 if Fade flag is set)
        ModalContentBorder.Opacity = ((EnterAnimation & CoreEnums.AnimationType.Fade) != 0) ? 0.0 : 1.0;

        var tg = ModalContentBorder.RenderTransform as TransformGroup;
        if (tg == null || tg.Children.Count < 2)
            throw new InvalidOperationException("ModalContentBorder must have a TransformGroup with ScaleTransform and TranslateTransform.");

        var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
        var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

        // Set initial scale for Scale animation
        if ((EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
        {
            scale.ScaleX = 0.8;
            scale.ScaleY = 0.8;
        }
        else
        {
            scale.ScaleX = 1;
            scale.ScaleY = 1;
        }

        // Compute initial slide offsets
        double startX = 0, startY = 0;
        double width = ModalContentBorder.ActualWidth > 0 ? ModalContentBorder.ActualWidth : ModalContentBorder.DesiredSize.Width;
        double height = ModalContentBorder.ActualHeight > 0 ? ModalContentBorder.ActualHeight : ModalContentBorder.DesiredSize.Height;

        if ((EnterAnimation & CoreEnums.AnimationType.SlideLeft) != 0)
            startX = -Math.Max(1, width);
        else if ((EnterAnimation & CoreEnums.AnimationType.SlideRight) != 0)
            startX = Math.Max(1, width);
        if ((EnterAnimation & CoreEnums.AnimationType.SlideTop) != 0)
            startY = -Math.Max(1, height);
        else if ((EnterAnimation & CoreEnums.AnimationType.SlideBottom) != 0)
            startY = Math.Max(1, height);
        translate.X = startX;
        translate.Y = startY;

        await EnsureTargetHasSize(ModalContentBorder);

        // Determine actual popup size (fallback to desired size or defaults)
        double popupWidth = ModalContentBorder.ActualWidth;
        double popupHeight = ModalContentBorder.ActualHeight;
        if (popupWidth <= 0) popupWidth = ModalContentBorder.DesiredSize.Width;
        if (popupHeight <= 0) popupHeight = ModalContentBorder.DesiredSize.Height;
        if (popupWidth <= 0) popupWidth = 200;
        if (popupHeight <= 0) popupHeight = 100;

        var screenBounds = GetScreenBounds();

        // Special handling for mouse cursor (direct calculation)
        if (ModalAnchorTarget == CoreEnums.AnchorTarget.MouseCursor)
        {
            var mainWin = System.Windows.Application.Current.MainWindow;
            var mousePos = Mouse.GetPosition(mainWin);
            var mouseScreen = mainWin.PointToScreen(mousePos);

            double x = mouseScreen.X;
            double y = mouseScreen.Y;

            switch (ModalAlignment)
            {
                case CoreEnums.PopupAlignment.TopLeft: break;
                case CoreEnums.PopupAlignment.TopCenter: x -= popupWidth / 2; break;
                case CoreEnums.PopupAlignment.TopRight: x -= popupWidth; break;
                case CoreEnums.PopupAlignment.LeftCenter: y -= popupHeight / 2; break;
                case CoreEnums.PopupAlignment.MiddleCenter: x -= popupWidth / 2; y -= popupHeight / 2; break;
                case CoreEnums.PopupAlignment.RightCenter: x -= popupWidth; y -= popupHeight / 2; break;
                case CoreEnums.PopupAlignment.BottomLeft: y -= popupHeight; break;
                case CoreEnums.PopupAlignment.BottomCenter: x -= popupWidth / 2; y -= popupHeight; break;
                case CoreEnums.PopupAlignment.BottomRight: x -= popupWidth; y -= popupHeight; break;
            }

            x += ModalOffsetX;
            y += ModalOffsetY;

            if (ModalAutoFlip)
            {
                if (x + popupWidth > screenBounds.Right) x = screenBounds.Right - popupWidth;
                if (x < screenBounds.Left) x = screenBounds.Left;
                if (y + popupHeight > screenBounds.Bottom) y = screenBounds.Bottom - popupHeight;
                if (y < screenBounds.Top) y = screenBounds.Top;
            }

            ModalContentBorder.Margin = new Thickness(x, y, 0, 0);
        }
        else
        {
            // Use PositionCalculator for other anchor types
            CoreRect? anchorRect = null;
            switch (ModalAnchorTarget)
            {
                case CoreEnums.AnchorTarget.UiElement when ModalAnchorElement != null:
                    var elem = ModalAnchorElement;
                    var elemPoint = elem.PointToScreen(new System.Windows.Point(0, 0));
                    var elemSize = new System.Windows.Size(elem.ActualWidth, elem.ActualHeight);
                    anchorRect = new CoreRect(elemPoint.X, elemPoint.Y, elemSize.Width, elemSize.Height);
                    break;
                case CoreEnums.AnchorTarget.CustomCoordinates:
                    anchorRect = new CoreRect(ModalCustomX, ModalCustomY, 1, 1);
                    break;
                case CoreEnums.AnchorTarget.ParentContainer:
                default:
                    anchorRect = null;
                    break;
            }

            var popupSize = new CoreSize(popupWidth, popupHeight);
            var request = new PopupRequest
            {
                AnchorTarget = ModalAnchorTarget,
                Alignment = ModalAlignment,
                Offset = (ModalOffsetX, ModalOffsetY),
                AutoFlip = ModalAutoFlip,
                CustomX = ModalAnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? ModalCustomX : null,
                CustomY = ModalAnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? ModalCustomY : null
            };

            var bounds = (ModalAnchorTarget == CoreEnums.AnchorTarget.ParentContainer) ? GetParentWindowBounds() : screenBounds;
            var finalPos = PositionCalculator.CalculatePosition(request, anchorRect, popupSize, bounds);
            ModalContentBorder.Margin = new Thickness(finalPos.X, finalPos.Y, 0, 0);
        }

        // Align margin‑based positioning by resetting alignment to top‑left
        ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Left;
        ModalContentBorder.VerticalAlignment = VerticalAlignment.Top;

        // Show the overlay and force a render pass
        OverlayGrid.Visibility = Visibility.Visible;
        await Dispatcher.InvokeAsync(() => ModalContentBorder.UpdateLayout(), DispatcherPriority.Render);

        IsOpen = true;
        Focusable = true;
        Focus();

        await EnsureTargetHasSize(ModalContentBorder);
        await AnimateEnterAsync(ModalContentBorder, EnterAnimation, EnterDuration, EnterEasing);
    }

    // ------------------------------------------------------------
    // Lightweight (floating) popup
    // ------------------------------------------------------------

    /// <summary>
    /// Displays a lightweight (non‑modal) popup that floats above the UI,
    /// anchored either to a specific element or to the mouse cursor.
    /// </summary>
    private async Task ShowLightweightAsync()
    {
        var border = new Border
        {
            Background = ModalContentBorder.Background,
            BorderBrush = ModalContentBorder.BorderBrush,
            BorderThickness = ModalContentBorder.BorderThickness,
            CornerRadius = ModalContentBorder.CornerRadius,
            Padding = ModalContentBorder.Padding,
            Effect = ModalContentBorder.Effect,
            RenderTransform = new TransformGroup
            {
                Children = new TransformCollection
                {
                    new ScaleTransform(1, 1),
                    new TranslateTransform(0, 0)
                }
            },
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

        // Grid to hold close button (if enabled) and content
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        if (ShowCloseButton)
        {
            var closeButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 5, 5, 0),
                Cursor = Cursors.Hand
            };
            if (CloseButtonTemplate != null)
                closeButton.Template = CloseButtonTemplate;
            else
                closeButton.Template = (ControlTemplate)this.FindResource("DefaultCloseButtonTemplate");
            closeButton.Click += (s, e) => _ = CloseAsync();
            Grid.SetRow(closeButton, 0);
            grid.Children.Add(closeButton);
        }

        var contentPresenter = new ContentPresenter
        {
            Content = this.Content,
            Margin = new Thickness(5)
        };
        Grid.SetRow(contentPresenter, 1);
        grid.Children.Add(contentPresenter);

        border.Child = grid;
        _lightweightContentHost = border;

        // Set initial animation states
        if ((EnterAnimation & CoreEnums.AnimationType.Fade) != 0)
            _lightweightContentHost.Opacity = 0;
        else
            _lightweightContentHost.Opacity = 1;

        if ((EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
        {
            var scale = (ScaleTransform)((TransformGroup)_lightweightContentHost.RenderTransform).Children[0];
            scale.ScaleX = scale.ScaleY = 0.8;
        }

        // Compute slide start offsets based on desired size
        double startX = 0, startY = 0;
        var size = MeasureContentSize(_lightweightContentHost);
        if ((EnterAnimation & CoreEnums.AnimationType.SlideLeft) != 0)
            startX = -size.Width;
        else if ((EnterAnimation & CoreEnums.AnimationType.SlideRight) != 0)
            startX = size.Width;
        if ((EnterAnimation & CoreEnums.AnimationType.SlideTop) != 0)
            startY = -size.Height;
        else if ((EnterAnimation & CoreEnums.AnimationType.SlideBottom) != 0)
            startY = size.Height;
        var translate = (TranslateTransform)((TransformGroup)_lightweightContentHost.RenderTransform).Children[1];
        translate.X = startX;
        translate.Y = startY;

        // Create the floating popup
        _lightweightPopup = new WpfPopup
        {
            Child = _lightweightContentHost,
            AllowsTransparency = true,
            StaysOpen = !StaysOpenOnOutsideClick,
            PlacementTarget = AnchorElement,
            Placement = AnchorElement != null ? PlacementMode.Custom : PlacementMode.MousePoint
        };
        if (AnchorElement != null)
            _lightweightPopup.CustomPopupPlacementCallback = OnCustomPopupPlacement;

        _lightweightPopup.Closed += (s, e) => _ = CloseAsync();
        _lightweightPopup.IsOpen = true;
        IsOpen = true;

        // Run the enter animation
        await AnimateEnterAsync(_lightweightContentHost, EnterAnimation, EnterDuration, EnterEasing);
    }
}