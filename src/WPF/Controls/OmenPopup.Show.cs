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
/// Partial class containing the <see cref="ShowAsync"/> method and its modal/lightweight implementations.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Shows the popup asynchronously. The behavior (modal or lightweight) is determined by <see cref="IsModal"/>.
    /// </summary>
    public async Task ShowAsync()
    {
        if (IsOpen) return;

        if (IsModal)
            await ShowModalAsync();
        else
            await ShowLightweightAsync();
    }

    // ============================================================================================
    // Modal (overlay) popup
    // ============================================================================================

    private async Task ShowModalAsync()
    {
        EnsureTransformGroup(ModalContentBorder);

        ModalContentBorder.Opacity = ((EnterAnimation & CoreEnums.AnimationType.Fade) != 0) ? 0.0 : 1.0;

        if (ModalContentBorder.RenderTransform is not TransformGroup { Children.Count: >= 2 } tg)
            throw new InvalidOperationException("ModalContentBorder must have a TransformGroup with ScaleTransform and TranslateTransform.");

        var scaleTransform = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
        var translateTransform = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

        if ((EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
        {
            scaleTransform.ScaleX = 0.8;
            scaleTransform.ScaleY = 0.8;
        }
        else
        {
            scaleTransform.ScaleX = 1;
            scaleTransform.ScaleY = 1;
        }

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
        translateTransform.X = startX;
        translateTransform.Y = startY;

        await EnsureTargetHasSize(ModalContentBorder);

        double popupWidth = ModalContentBorder.ActualWidth;
        double popupHeight = ModalContentBorder.ActualHeight;
        if (popupWidth <= 0) popupWidth = ModalContentBorder.DesiredSize.Width;
        if (popupHeight <= 0) popupHeight = ModalContentBorder.DesiredSize.Height;
        if (popupWidth <= 0) popupWidth = 200;
        if (popupHeight <= 0) popupHeight = 100;

        if (AnchorTarget == CoreEnums.AnchorTarget.ParentContainer)
        {
            // Ensure the overlay grid has been measured and arranged.
            OverlayGrid.Visibility = Visibility.Visible;
            await Dispatcher.InvokeAsync(() => OverlayGrid.UpdateLayout(), DispatcherPriority.Loaded);
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Render);

            double clientWidth = OverlayGrid.ActualWidth;
            double clientHeight = OverlayGrid.ActualHeight;

            // Fallback to window content area if overlay size is still zero.
            if (clientWidth <= 0 || clientHeight <= 0)
            {
                var window = Window.GetWindow(this);
                if (window != null && window.Content is FrameworkElement content)
                {
                    clientWidth = content.ActualWidth;
                    clientHeight = content.ActualHeight;
                }
                else
                {
                    clientWidth = SystemParameters.WorkArea.Width;
                    clientHeight = SystemParameters.WorkArea.Height;
                }
            }

            double x = 0, y = 0;
            switch (Alignment)
            {
                case CoreEnums.PopupAlignment.TopLeft:
                    x = 0; y = 0;
                    break;
                case CoreEnums.PopupAlignment.TopCenter:
                    x = (clientWidth - popupWidth) / 2; y = 0;
                    break;
                case CoreEnums.PopupAlignment.TopRight:
                    x = clientWidth - popupWidth; y = 0;
                    break;
                case CoreEnums.PopupAlignment.LeftCenter:
                    x = 0; y = (clientHeight - popupHeight) / 2;
                    break;
                case CoreEnums.PopupAlignment.MiddleCenter:
                    x = (clientWidth - popupWidth) / 2; y = (clientHeight - popupHeight) / 2;
                    break;
                case CoreEnums.PopupAlignment.RightCenter:
                    x = clientWidth - popupWidth; y = (clientHeight - popupHeight) / 2;
                    break;
                case CoreEnums.PopupAlignment.BottomLeft:
                    x = 0; y = clientHeight - popupHeight;
                    break;
                case CoreEnums.PopupAlignment.BottomCenter:
                    x = (clientWidth - popupWidth) / 2; y = clientHeight - popupHeight;
                    break;
                case CoreEnums.PopupAlignment.BottomRight:
                    x = clientWidth - popupWidth; y = clientHeight - popupHeight;
                    break;
                default:
                    x = 0; y = 0;
                    break;
            }

            x += OffsetX;
            y += OffsetY;

            if (AutoFlip)
            {
                if (x + popupWidth > clientWidth) x = clientWidth - popupWidth;
                if (x < 0) x = 0;
                if (y + popupHeight > clientHeight) y = clientHeight - popupHeight;
                if (y < 0) y = 0;
            }

            ModalContentBorder.Margin = new Thickness(x, y, 0, 0);
            ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Left;
            ModalContentBorder.VerticalAlignment = VerticalAlignment.Top;

            // Overlay is already visible, no need to set again.
        }
        else
        {
            CoreRect? anchorRect = null;
            switch (AnchorTarget)
            {
                case CoreEnums.AnchorTarget.UiElement when AnchorElement != null:
                    var elem = AnchorElement;
                    var elemPoint = elem.PointToScreen(new System.Windows.Point(0, 0));
                    var elemSize = new System.Windows.Size(elem.ActualWidth, elem.ActualHeight);
                    anchorRect = new CoreRect(elemPoint.X, elemPoint.Y, elemSize.Width, elemSize.Height);
                    break;
                case CoreEnums.AnchorTarget.MouseCursor:
                    var mainWin = System.Windows.Application.Current.MainWindow;
                    var mousePos = Mouse.GetPosition(mainWin);
                    var mouseScreen = mainWin.PointToScreen(mousePos);
                    anchorRect = new CoreRect(mouseScreen.X, mouseScreen.Y, 1, 1);
                    break;
                case CoreEnums.AnchorTarget.CustomCoordinates:
                    anchorRect = new CoreRect(CustomX, CustomY, 1, 1);
                    break;
                default:
                    anchorRect = null;
                    break;
            }

            var screenBounds = GetScreenBounds();
            var bounds = (AnchorTarget == CoreEnums.AnchorTarget.ParentContainer) ? GetParentWindowBounds() : screenBounds;
            var popupSize = new CoreSize(popupWidth, popupHeight);
            var request = new PopupRequest
            {
                AnchorTarget = AnchorTarget,
                Alignment = Alignment,
                Offset = (OffsetX, OffsetY),
                AutoFlip = AutoFlip,
                CustomX = AnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? CustomX : null,
                CustomY = AnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? CustomY : null
            };

            var finalPos = PositionCalculator.CalculatePosition(request, anchorRect, popupSize, bounds);
            ModalContentBorder.Margin = new Thickness(finalPos.X, finalPos.Y, 0, 0);
            ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Left;
            ModalContentBorder.VerticalAlignment = VerticalAlignment.Top;

            OverlayGrid.Visibility = Visibility.Visible;
        }

        await Dispatcher.InvokeAsync(() => ModalContentBorder.UpdateLayout(), DispatcherPriority.Render);

        IsOpen = true;
        Focusable = true;
        Focus();

        await EnsureTargetHasSize(ModalContentBorder);
        await AnimateEnterAsync(ModalContentBorder, EnterAnimation, EnterDuration, EnterEasing);
    }

    // ============================================================================================
    // Lightweight (floating) popup
    // ============================================================================================

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
                Children = [new ScaleTransform(1, 1), new TranslateTransform(0, 0)]
            },
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

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

        if ((EnterAnimation & CoreEnums.AnimationType.Fade) != 0)
            _lightweightContentHost.Opacity = 0;
        else
            _lightweightContentHost.Opacity = 1;

        if ((EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
        {
            var scale = (ScaleTransform)((TransformGroup)_lightweightContentHost.RenderTransform).Children[0];
            scale.ScaleX = scale.ScaleY = 0.8;
        }

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

        await EnsureTargetHasSize(_lightweightContentHost);
        double popupWidth = _lightweightContentHost.ActualWidth;
        double popupHeight = _lightweightContentHost.ActualHeight;
        if (popupWidth <= 0) popupWidth = _lightweightContentHost.DesiredSize.Width;
        if (popupHeight <= 0) popupHeight = _lightweightContentHost.DesiredSize.Height;
        if (popupWidth <= 0) popupWidth = 200;
        if (popupHeight <= 0) popupHeight = 100;

        if (AnchorTarget == CoreEnums.AnchorTarget.ParentContainer)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;

            // Get the window's content element to determine client area size
            var windowContent = window.Content as FrameworkElement;
            if (windowContent == null) return;

            // Force layout to get actual content size
            windowContent.UpdateLayout();
            double clientWidth = windowContent.ActualWidth;
            double clientHeight = windowContent.ActualHeight;

            // Fallback to window size if content size is still zero
            if (clientWidth <= 0) clientWidth = window.ActualWidth;
            if (clientHeight <= 0) clientHeight = window.ActualHeight;

            var windowPos = window.PointToScreen(new System.Windows.Point(0, 0));

            double x = 0, y = 0;
            switch (Alignment)
            {
                case CoreEnums.PopupAlignment.TopLeft:
                    x = windowPos.X; y = windowPos.Y;
                    break;
                case CoreEnums.PopupAlignment.TopCenter:
                    x = windowPos.X + (clientWidth - popupWidth) / 2; y = windowPos.Y;
                    break;
                case CoreEnums.PopupAlignment.TopRight:
                    x = windowPos.X + clientWidth - popupWidth; y = windowPos.Y;
                    break;
                case CoreEnums.PopupAlignment.LeftCenter:
                    x = windowPos.X; y = windowPos.Y + (clientHeight - popupHeight) / 2;
                    break;
                case CoreEnums.PopupAlignment.MiddleCenter:
                    x = windowPos.X + (clientWidth - popupWidth) / 2; y = windowPos.Y + (clientHeight - popupHeight) / 2;
                    break;
                case CoreEnums.PopupAlignment.RightCenter:
                    x = windowPos.X + clientWidth - popupWidth; y = windowPos.Y + (clientHeight - popupHeight) / 2;
                    break;
                case CoreEnums.PopupAlignment.BottomLeft:
                    x = windowPos.X; y = windowPos.Y + clientHeight - popupHeight;
                    break;
                case CoreEnums.PopupAlignment.BottomCenter:
                    x = windowPos.X + (clientWidth - popupWidth) / 2; y = windowPos.Y + clientHeight - popupHeight;
                    break;
                case CoreEnums.PopupAlignment.BottomRight:
                    x = windowPos.X + clientWidth - popupWidth; y = windowPos.Y + clientHeight - popupHeight;
                    break;
                default:
                    x = windowPos.X; y = windowPos.Y;
                    break;
            }

            x += OffsetX;
            y += OffsetY;

            if (AutoFlip)
            {
                var screenBounds = GetScreenBounds();
                if (x + popupWidth > screenBounds.Right) x = screenBounds.Right - popupWidth;
                if (x < screenBounds.Left) x = screenBounds.Left;
                if (y + popupHeight > screenBounds.Bottom) y = screenBounds.Bottom - popupHeight;
                if (y < screenBounds.Top) y = screenBounds.Top;
            }

            _lightweightPopup = new WpfPopup
            {
                Child = _lightweightContentHost,
                AllowsTransparency = true,
                StaysOpen = !CloseOnOutsideClick,
                Placement = PlacementMode.Absolute,
                HorizontalOffset = x,
                VerticalOffset = y
            };
        }
        else
        {
            // Existing logic for other anchor targets (unchanged)
            CoreRect? anchorRect = null;
            switch (AnchorTarget)
            {
                case CoreEnums.AnchorTarget.UiElement when AnchorElement != null:
                    var elem = AnchorElement;
                    var elemPoint = elem.PointToScreen(new System.Windows.Point(0, 0));
                    var elemSize = new System.Windows.Size(elem.ActualWidth, elem.ActualHeight);
                    anchorRect = new CoreRect(elemPoint.X, elemPoint.Y, elemSize.Width, elemSize.Height);
                    break;
                case CoreEnums.AnchorTarget.MouseCursor:
                    var mainWin = System.Windows.Application.Current.MainWindow;
                    var mousePos = Mouse.GetPosition(mainWin);
                    var mouseScreen = mainWin.PointToScreen(mousePos);
                    anchorRect = new CoreRect(mouseScreen.X, mouseScreen.Y, 1, 1);
                    break;
                case CoreEnums.AnchorTarget.CustomCoordinates:
                    anchorRect = new CoreRect(CustomX, CustomY, 1, 1);
                    break;
                default:
                    anchorRect = null;
                    break;
            }

            var screenBounds = GetScreenBounds();
            var bounds = (AnchorTarget == CoreEnums.AnchorTarget.ParentContainer) ? GetParentWindowBounds() : screenBounds;
            var popupSize = new CoreSize(popupWidth, popupHeight);
            var request = new PopupRequest
            {
                AnchorTarget = AnchorTarget,
                Alignment = Alignment,
                Offset = (OffsetX, OffsetY),
                AutoFlip = AutoFlip,
                CustomX = AnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? CustomX : null,
                CustomY = AnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? CustomY : null
            };

            var finalPos = PositionCalculator.CalculatePosition(request, anchorRect, popupSize, bounds);

            _lightweightPopup = new WpfPopup
            {
                Child = _lightweightContentHost,
                AllowsTransparency = true,
                StaysOpen = !CloseOnOutsideClick,
                Placement = PlacementMode.Custom,
                CustomPopupPlacementCallback = (popupSize, targetSize, offset) =>
                {
                    double x = finalPos.X;
                    double y = finalPos.Y;
                    if (AnchorTarget == CoreEnums.AnchorTarget.UiElement && AnchorElement != null)
                    {
                        var targetPoint = AnchorElement.PointToScreen(new System.Windows.Point(0, 0));
                        x -= targetPoint.X;
                        y -= targetPoint.Y;
                    }
                    return [new CustomPopupPlacement(new System.Windows.Point(x, y), PopupPrimaryAxis.None)];
                }
            };
        }

        _lightweightPopup.Closed += (s, e) => _ = CloseAsync();
        _lightweightPopup.IsOpen = true;
        IsOpen = true;

        await AnimateEnterAsync(_lightweightContentHost, EnterAnimation, EnterDuration, EnterEasing);
    }
}