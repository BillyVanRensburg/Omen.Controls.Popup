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
/// This file handles all logic for showing the popup, including positioning, animations, and UI construction.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Shows the popup asynchronously. The behavior (modal or lightweight) is determined by <see cref="IsModal"/>.
    /// </summary>
    /// <returns>A task that completes when the popup is fully open and any enter animation has finished.</returns>
    public async Task ShowAsync()
    {
        if (IsOpen) return;
        if (IsModal) await ShowModalAsync();
        else await ShowLightweightAsync();
    }

    // ------------------------------------------------------------------------
    // Common positioning helper – returns screen coordinates
    // ------------------------------------------------------------------------

    /// <summary>
    /// Calculates the screen coordinates where the popup should be placed,
    /// based on the current positioning properties (<see cref="AnchorTarget"/>,
    /// <see cref="AnchorElement"/>, <see cref="Alignment"/>, <see cref="OffsetX"/>,
    /// <see cref="OffsetY"/>, <see cref="AutoFlip"/>, <see cref="CustomX"/>, <see cref="CustomY"/>).
    /// </summary>
    /// <param name="popupWidth">The width of the popup (after layout).</param>
    /// <param name="popupHeight">The height of the popup (after layout).</param>
    /// <returns>The top‑left screen coordinates for the popup.</returns>
    /// <remarks>
    /// For <see cref="CoreEnums.AnchorTarget.ParentContainer"/>, this method returns screen coordinates
    /// relative to the parent window's content area. For other anchor targets, it uses <see cref="PositionCalculator"/>.
    /// The anchor element is forcibly updated to ensure its size is correct.
    /// </remarks>
    private async Task<CorePoint> GetPositionInScreenCoordinates(double popupWidth, double popupHeight)
    {
        // Ensure anchor element has proper layout
        if (AnchorElement != null)
        {
            AnchorElement.UpdateLayout();
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Loaded);
        }

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
            case CoreEnums.AnchorTarget.ParentContainer:
                // Handled separately below
                break;
            default:
                anchorRect = null;
                break;
        }

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

        if (AnchorTarget == CoreEnums.AnchorTarget.ParentContainer)
        {
            // For ParentContainer, we position relative to the parent window's content area.
            var window = Window.GetWindow(this);
            if (window == null) return new CorePoint(0, 0);
            var windowPos = window.PointToScreen(new System.Windows.Point(0, 0));
            double windowWidth = window.ActualWidth;
            double windowHeight = window.ActualHeight;

            double x = 0, y = 0;
            switch (Alignment)
            {
                case CoreEnums.PopupAlignment.TopLeft: x = 0; y = 0; break;
                case CoreEnums.PopupAlignment.TopCenter: x = (windowWidth - popupWidth) / 2; y = 0; break;
                case CoreEnums.PopupAlignment.TopRight: x = windowWidth - popupWidth; y = 0; break;
                case CoreEnums.PopupAlignment.LeftCenter: x = 0; y = (windowHeight - popupHeight) / 2; break;
                case CoreEnums.PopupAlignment.MiddleCenter: x = (windowWidth - popupWidth) / 2; y = (windowHeight - popupHeight) / 2; break;
                case CoreEnums.PopupAlignment.RightCenter: x = windowWidth - popupWidth; y = (windowHeight - popupHeight) / 2; break;
                case CoreEnums.PopupAlignment.BottomLeft: x = 0; y = windowHeight - popupHeight; break;
                case CoreEnums.PopupAlignment.BottomCenter: x = (windowWidth - popupWidth) / 2; y = windowHeight - popupHeight; break;
                case CoreEnums.PopupAlignment.BottomRight: x = windowWidth - popupWidth; y = windowHeight - popupHeight; break;
                default: x = 0; y = 0; break;
            }
            x += OffsetX;
            y += OffsetY;

            // Auto‑flip (keep inside window)
            if (AutoFlip)
            {
                if (x + popupWidth > windowWidth) x = windowWidth - popupWidth;
                if (x < 0) x = 0;
                if (y + popupHeight > windowHeight) y = windowHeight - popupHeight;
                if (y < 0) y = 0;
            }

            // Convert to screen coordinates by adding the window's screen position.
            return new CorePoint(windowPos.X + x, windowPos.Y + y);
        }
        else
        {
            // For other anchor targets, use the PositionCalculator with screen bounds.
            var screenBounds = GetScreenBounds();
            var finalPos = PositionCalculator.CalculatePosition(request, anchorRect, popupSize, screenBounds);
            return finalPos;
        }
    }

    // ------------------------------------------------------------------------
    // Modal (overlay) popup
    // ------------------------------------------------------------------------

    /// <summary>
    /// Displays a modal popup with a full‑window overlay.
    /// The position is calculated using the common positioning properties.
    /// </summary>
    /// <remarks>
    /// This method also handles the initial transform states for the enter animation (fade, scale, slide)
    /// and ensures the overlay is visible before the animation starts.
    /// Focus is trapped inside the popup: the previous focused element is stored and focus is moved to the popup.
    /// </remarks>
    private async Task ShowModalAsync()
    {
        // Ensure the content border has the necessary transforms for animations.
        EnsureTransformGroup(ModalContentBorder);

        // Set initial opacity: 0 if fade is requested, otherwise 1.
        ModalContentBorder.Opacity = ((EnterAnimation & CoreEnums.AnimationType.Fade) != 0) ? 0.0 : 1.0;

        // Verify that the render transform is a TransformGroup with at least two children (scale + translate).
        if (ModalContentBorder.RenderTransform is not TransformGroup { Children.Count: >= 2 } tg)
            throw new InvalidOperationException("ModalContentBorder must have a TransformGroup with ScaleTransform and TranslateTransform.");

        var scaleTransform = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
        var translateTransform = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

        // Set initial scale for the Scale animation.
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

        // Compute initial slide offsets based on the current or desired size of the content.
        double width = ModalContentBorder.ActualWidth > 0 ? ModalContentBorder.ActualWidth : ModalContentBorder.DesiredSize.Width;
        double height = ModalContentBorder.ActualHeight > 0 ? ModalContentBorder.ActualHeight : ModalContentBorder.DesiredSize.Height;
        double startX = 0, startY = 0;
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

        // Ensure the popup has a known size (for positioning calculations).
        await EnsureTargetHasSize(ModalContentBorder);

        // Determine the actual popup size (fallback to desired size or sensible defaults).
        double popupWidth = ModalContentBorder.ActualWidth;
        double popupHeight = ModalContentBorder.ActualHeight;
        if (popupWidth <= 0) popupWidth = ModalContentBorder.DesiredSize.Width;
        if (popupHeight <= 0) popupHeight = ModalContentBorder.DesiredSize.Height;
        if (popupWidth <= 0) popupWidth = 200;
        if (popupHeight <= 0) popupHeight = 100;

        if (AnchorTarget == CoreEnums.AnchorTarget.ParentContainer)
        {
            // For ParentContainer, we position relative to the overlay client area.
            await Dispatcher.InvokeAsync(() => OverlayGrid.UpdateLayout(), DispatcherPriority.Render);
            double clientWidth = OverlayGrid.ActualWidth;
            double clientHeight = OverlayGrid.ActualHeight;
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
                    clientWidth = 800;
                    clientHeight = 600;
                }
            }

            double x = 0, y = 0;
            switch (Alignment)
            {
                case CoreEnums.PopupAlignment.TopLeft: x = 0; y = 0; break;
                case CoreEnums.PopupAlignment.TopCenter: x = (clientWidth - popupWidth) / 2; y = 0; break;
                case CoreEnums.PopupAlignment.TopRight: x = clientWidth - popupWidth; y = 0; break;
                case CoreEnums.PopupAlignment.LeftCenter: x = 0; y = (clientHeight - popupHeight) / 2; break;
                case CoreEnums.PopupAlignment.MiddleCenter: x = (clientWidth - popupWidth) / 2; y = (clientHeight - popupHeight) / 2; break;
                case CoreEnums.PopupAlignment.RightCenter: x = clientWidth - popupWidth; y = (clientHeight - popupHeight) / 2; break;
                case CoreEnums.PopupAlignment.BottomLeft: x = 0; y = clientHeight - popupHeight; break;
                case CoreEnums.PopupAlignment.BottomCenter: x = (clientWidth - popupWidth) / 2; y = clientHeight - popupHeight; break;
                case CoreEnums.PopupAlignment.BottomRight: x = clientWidth - popupWidth; y = clientHeight - popupHeight; break;
                default: x = 0; y = 0; break;
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
        }
        else
        {
            // For other anchor targets, get screen coordinates and convert to client coordinates.
            var screenPos = await GetPositionInScreenCoordinates(popupWidth, popupHeight);
            var window = Window.GetWindow(this);
            if (window != null)
            {
                var windowPos = window.PointToScreen(new System.Windows.Point(0, 0));
                double clientX = screenPos.X - windowPos.X;
                double clientY = screenPos.Y - windowPos.Y;
                ModalContentBorder.Margin = new Thickness(clientX, clientY, 0, 0);
            }
            else
            {
                ModalContentBorder.Margin = new Thickness(screenPos.X, screenPos.Y, 0, 0);
            }
        }

        // Align margin‑based positioning by resetting alignment to top‑left.
        ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Left;
        ModalContentBorder.VerticalAlignment = VerticalAlignment.Top;

        // Show the overlay and force a render pass to avoid visual glitches.
        OverlayGrid.Visibility = Visibility.Visible;
        await Dispatcher.InvokeAsync(() => ModalContentBorder.UpdateLayout(), DispatcherPriority.Render);

        // Mark as open and trap focus.
        IsOpen = true;
        Focusable = true;

        // Store the currently focused element and move focus to the popup content.
        _previousFocus = Keyboard.FocusedElement;
        ModalContentBorder.Focus();

        // Run the enter animation.
        await EnsureTargetHasSize(ModalContentBorder);
        await AnimateEnterAsync(ModalContentBorder, EnterAnimation, EnterDuration, EnterEasing);
    }

    // ------------------------------------------------------------------------
    // Lightweight (floating) popup
    // ------------------------------------------------------------------------

    /// <summary>
    /// Displays a lightweight (non‑modal) popup that floats above the UI.
    /// Positioning uses the same common properties as modal, and the popup is created using
    /// a WPF <see cref="Popup"/> control with absolute placement.
    /// </summary>
    /// <remarks>
    /// The popup can be anchored to a UI element, the mouse cursor, the parent container, or custom coordinates.
    /// The <see cref="CloseOnOutsideClick"/> property controls whether clicking outside closes the popup.
    /// Focus is also trapped inside the lightweight popup content.
    /// </remarks>
    private async Task ShowLightweightAsync()
    {
        // Create a Border that mimics the visual appearance of the modal content border.
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

        // Grid to hold the close button (if enabled) and the actual content.
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
        _lightweightContentHost.Focusable = true;
        _lightweightContentHost.InvalidateMeasure();
        _lightweightContentHost.InvalidateArrange();
        await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Loaded);

        // Force layout to ensure the content is measured.
        _lightweightContentHost.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double desiredWidth = _lightweightContentHost.DesiredSize.Width;
        double desiredHeight = _lightweightContentHost.DesiredSize.Height;
        if (desiredWidth <= 0) desiredWidth = 200;
        if (desiredHeight <= 0) desiredHeight = 100;
        _lightweightContentHost.Arrange(new Rect(0, 0, desiredWidth, desiredHeight));
        _lightweightContentHost.UpdateLayout();

        // Set initial animation states.
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

        // Ensure the popup has a known size (for positioning calculations).
        await EnsureTargetHasSize(_lightweightContentHost);
        double popupWidth = _lightweightContentHost.ActualWidth;
        double popupHeight = _lightweightContentHost.ActualHeight;
        if (popupWidth <= 0) popupWidth = _lightweightContentHost.DesiredSize.Width;
        if (popupHeight <= 0) popupHeight = _lightweightContentHost.DesiredSize.Height;
        if (popupWidth <= 0) popupWidth = 200;
        if (popupHeight <= 0) popupHeight = 100;

        // Obtain screen coordinates.
        var screenPos = await GetPositionInScreenCoordinates(popupWidth, popupHeight);

        _lightweightPopup = new WpfPopup
        {
            Child = _lightweightContentHost,
            AllowsTransparency = true,
            StaysOpen = !CloseOnOutsideClick,
            Placement = PlacementMode.Absolute,
            HorizontalOffset = screenPos.X,
            VerticalOffset = screenPos.Y
        };

        _lightweightPopup.Closed += (s, e) => _ = CloseAsync();

        // Focus trapping for lightweight
        _previousFocus = Keyboard.FocusedElement;
        _lightweightContentHost.Focus();
        _lightweightContentHost.PreviewLostKeyboardFocus += OnLightweightPreviewLostKeyboardFocus;

        _lightweightPopup.IsOpen = true;
        IsOpen = true;

        // Run the enter animation.
        await AnimateEnterAsync(_lightweightContentHost, EnterAnimation, EnterDuration, EnterEasing);
    }

    // ------------------------------------------------------------------------
    // Lightweight focus trapping helpers
    // ------------------------------------------------------------------------

    /// <summary>
    /// Prevents keyboard focus from leaving the lightweight popup when it is open.
    /// </summary>
    private void OnLightweightPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (!IsModal && IsOpen && _lightweightContentHost != null)
        {
            if (!IsChildOfLightweightPopup(e.NewFocus))
            {
                _lightweightContentHost.Focus();
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Determines whether a given element is a descendant of the lightweight popup content.
    /// </summary>
    private bool IsChildOfLightweightPopup(IInputElement? element)
    {
        if (element == null) return false;
        var fe = element as FrameworkElement;
        while (fe != null)
        {
            if (fe == _lightweightContentHost)
                return true;
            fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
        }
        return false;
    }
}