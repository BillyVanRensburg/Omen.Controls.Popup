using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WpfPopup = System.Windows.Controls.Primitives.Popup;
using CorePoint = Omen.Controls.Popup.Core.Primitives.Point;
using CoreSize = Omen.Controls.Popup.Core.Primitives.Size;
using CoreRect = Omen.Controls.Popup.Core.Primitives.Rectangle;
using Omen.Controls.Popup.Application.Positioning;
using Omen.Controls.Popup.Core.Models;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.WPF.Controls
{
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

    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    public partial class OmenPopup : UserControl
    {
        #region Dependency Properties (Core)

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(OmenPopup),
                new PropertyMetadata(false, OnIsOpenChanged));
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        private static async void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (OmenPopup)d;
            if ((bool)e.NewValue)
                await popup.ShowAsync();
            else
                await popup.CloseAsync();
        }

        public static new readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(OmenPopup));
        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty IsModalProperty =
            DependencyProperty.Register(nameof(IsModal), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));
        public bool IsModal
        {
            get => (bool)GetValue(IsModalProperty);
            set => SetValue(IsModalProperty, value);
        }

        public static readonly DependencyProperty CanCloseOnEscapeProperty =
            DependencyProperty.Register(nameof(CanCloseOnEscape), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));
        public bool CanCloseOnEscape
        {
            get => (bool)GetValue(CanCloseOnEscapeProperty);
            set => SetValue(CanCloseOnEscapeProperty, value);
        }

        public static readonly DependencyProperty CloseOnOverlayClickProperty =
            DependencyProperty.Register(nameof(CloseOnOverlayClick), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));
        public bool CloseOnOverlayClick
        {
            get => (bool)GetValue(CloseOnOverlayClickProperty);
            set => SetValue(CloseOnOverlayClickProperty, value);
        }

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));
        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        public static readonly DependencyProperty CloseButtonTemplateProperty =
            DependencyProperty.Register(nameof(CloseButtonTemplate), typeof(ControlTemplate), typeof(OmenPopup));
        public ControlTemplate CloseButtonTemplate
        {
            get => (ControlTemplate)GetValue(CloseButtonTemplateProperty);
            set => SetValue(CloseButtonTemplateProperty, value);
        }

        public static readonly DependencyProperty EnterAnimationProperty =
            DependencyProperty.Register(nameof(EnterAnimation), typeof(AnimationType), typeof(OmenPopup), new PropertyMetadata(AnimationType.Fade));
        public AnimationType EnterAnimation
        {
            get => (AnimationType)GetValue(EnterAnimationProperty);
            set => SetValue(EnterAnimationProperty, value);
        }

        public static readonly DependencyProperty ExitAnimationProperty =
            DependencyProperty.Register(nameof(ExitAnimation), typeof(AnimationType), typeof(OmenPopup), new PropertyMetadata(AnimationType.Fade));
        public AnimationType ExitAnimation
        {
            get => (AnimationType)GetValue(ExitAnimationProperty);
            set => SetValue(ExitAnimationProperty, value);
        }

        public static readonly DependencyProperty EnterDurationProperty =
            DependencyProperty.Register(nameof(EnterDuration), typeof(int), typeof(OmenPopup), new PropertyMetadata(200));
        public int EnterDuration
        {
            get => (int)GetValue(EnterDurationProperty);
            set => SetValue(EnterDurationProperty, value);
        }

        public static readonly DependencyProperty ExitDurationProperty =
            DependencyProperty.Register(nameof(ExitDuration), typeof(int), typeof(OmenPopup), new PropertyMetadata(200));
        public int ExitDuration
        {
            get => (int)GetValue(ExitDurationProperty);
            set => SetValue(ExitDurationProperty, value);
        }

        public static readonly DependencyProperty EnterEasingProperty =
            DependencyProperty.Register(nameof(EnterEasing), typeof(EasingType), typeof(OmenPopup), new PropertyMetadata(EasingType.EaseOut));
        public EasingType EnterEasing
        {
            get => (EasingType)GetValue(EnterEasingProperty);
            set => SetValue(EnterEasingProperty, value);
        }

        public static readonly DependencyProperty ExitEasingProperty =
            DependencyProperty.Register(nameof(ExitEasing), typeof(EasingType), typeof(OmenPopup), new PropertyMetadata(EasingType.EaseIn));
        public EasingType ExitEasing
        {
            get => (EasingType)GetValue(ExitEasingProperty);
            set => SetValue(ExitEasingProperty, value);
        }

        // Lightweight specific
        public static readonly DependencyProperty StaysOpenOnOutsideClickProperty =
            DependencyProperty.Register(nameof(StaysOpenOnOutsideClick), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));
        public bool StaysOpenOnOutsideClick
        {
            get => (bool)GetValue(StaysOpenOnOutsideClickProperty);
            set => SetValue(StaysOpenOnOutsideClickProperty, value);
        }

        public static readonly DependencyProperty AnchorElementProperty =
            DependencyProperty.Register(nameof(AnchorElement), typeof(FrameworkElement), typeof(OmenPopup));
        public FrameworkElement? AnchorElement
        {
            get => (FrameworkElement?)GetValue(AnchorElementProperty);
            set => SetValue(AnchorElementProperty, value);
        }

        #endregion

        #region Modal Positioning Properties

        public static readonly DependencyProperty ModalAnchorTargetProperty =
            DependencyProperty.Register(nameof(ModalAnchorTarget), typeof(CoreEnums.AnchorTarget), typeof(OmenPopup), new PropertyMetadata(CoreEnums.AnchorTarget.ParentWindowCenter));

        public CoreEnums.AnchorTarget ModalAnchorTarget
        {
            get => (CoreEnums.AnchorTarget)GetValue(ModalAnchorTargetProperty);
            set => SetValue(ModalAnchorTargetProperty, value);
        }

        public static readonly DependencyProperty ModalAnchorElementProperty =
            DependencyProperty.Register(nameof(ModalAnchorElement), typeof(FrameworkElement), typeof(OmenPopup));

        public FrameworkElement? ModalAnchorElement
        {
            get => (FrameworkElement?)GetValue(ModalAnchorElementProperty);
            set => SetValue(ModalAnchorElementProperty, value);
        }

        public static readonly DependencyProperty ModalAlignmentProperty =
            DependencyProperty.Register(nameof(ModalAlignment), typeof(CoreEnums.PopupAlignment), typeof(OmenPopup), new PropertyMetadata(CoreEnums.PopupAlignment.Center));

        public CoreEnums.PopupAlignment ModalAlignment
        {
            get => (CoreEnums.PopupAlignment)GetValue(ModalAlignmentProperty);
            set => SetValue(ModalAlignmentProperty, value);
        }

        public static readonly DependencyProperty ModalOffsetXProperty =
            DependencyProperty.Register(nameof(ModalOffsetX), typeof(int), typeof(OmenPopup), new PropertyMetadata(0));

        public int ModalOffsetX
        {
            get => (int)GetValue(ModalOffsetXProperty);
            set => SetValue(ModalOffsetXProperty, value);
        }

        public static readonly DependencyProperty ModalOffsetYProperty =
            DependencyProperty.Register(nameof(ModalOffsetY), typeof(int), typeof(OmenPopup), new PropertyMetadata(0));

        public int ModalOffsetY
        {
            get => (int)GetValue(ModalOffsetYProperty);
            set => SetValue(ModalOffsetYProperty, value);
        }

        public static readonly DependencyProperty ModalAutoFlipProperty =
            DependencyProperty.Register(nameof(ModalAutoFlip), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

        public bool ModalAutoFlip
        {
            get => (bool)GetValue(ModalAutoFlipProperty);
            set => SetValue(ModalAutoFlipProperty, value);
        }

        public static readonly DependencyProperty ModalScreenEdgeProperty =
            DependencyProperty.Register(nameof(ModalScreenEdge), typeof(CoreEnums.ScreenEdge), typeof(OmenPopup), new PropertyMetadata(CoreEnums.ScreenEdge.Top));

        public CoreEnums.ScreenEdge ModalScreenEdge
        {
            get => (CoreEnums.ScreenEdge)GetValue(ModalScreenEdgeProperty);
            set => SetValue(ModalScreenEdgeProperty, value);
        }

        public static readonly DependencyProperty ModalCustomXProperty =
            DependencyProperty.Register(nameof(ModalCustomX), typeof(double), typeof(OmenPopup), new PropertyMetadata(0.0));

        public double ModalCustomX
        {
            get => (double)GetValue(ModalCustomXProperty);
            set => SetValue(ModalCustomXProperty, value);
        }

        public static readonly DependencyProperty ModalCustomYProperty =
            DependencyProperty.Register(nameof(ModalCustomY), typeof(double), typeof(OmenPopup), new PropertyMetadata(0.0));

        public double ModalCustomY
        {
            get => (double)GetValue(ModalCustomYProperty);
            set => SetValue(ModalCustomYProperty, value);
        }

        #endregion

        public event EventHandler? Closed;

        private WpfPopup? _lightweightPopup;
        private FrameworkElement? _lightweightContentHost;

        public OmenPopup()
        {
            InitializeComponent();
            OverlayGrid.MouseLeftButtonDown += OverlayGrid_MouseLeftButtonDown;
            Loaded += OmenPopup_Loaded;
        }

        private void OmenPopup_Loaded(object? sender, RoutedEventArgs e)
        {
            EnsureTransformGroup(ModalContentBorder);
        }

        private void OverlayGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOpen && CloseOnOverlayClick)
                _ = CloseAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _ = CloseAsync();
        }

        public async Task ShowAsync()
        {
            if (IsOpen) return;

            if (IsModal)
                await ShowModalAsync();
            else
                await ShowLightweightAsync();
        }

        // ------------------------------------------------------------
        // Modal with positioning (mouse cursor fixed)
        // ------------------------------------------------------------
        private async Task ShowModalAsync()
        {
            EnsureTransformGroup(ModalContentBorder);

            // Animation start state
            ModalContentBorder.Opacity = ((EnterAnimation & AnimationType.Fade) != 0) ? 0.0 : 1.0;

            var tg = ModalContentBorder.RenderTransform as TransformGroup;
            if (tg == null || tg.Children.Count < 2)
                throw new InvalidOperationException("ModalContentBorder must have a TransformGroup with ScaleTransform and TranslateTransform.");

            var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
            var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

            if ((EnterAnimation & AnimationType.Scale) != 0)
            {
                scale.ScaleX = 0.8;
                scale.ScaleY = 0.8;
            }
            else
            {
                scale.ScaleX = 1;
                scale.ScaleY = 1;
            }

            double startX = 0, startY = 0;
            double width = ModalContentBorder.ActualWidth > 0 ? ModalContentBorder.ActualWidth : ModalContentBorder.DesiredSize.Width;
            double height = ModalContentBorder.ActualHeight > 0 ? ModalContentBorder.ActualHeight : ModalContentBorder.DesiredSize.Height;

            if ((EnterAnimation & AnimationType.SlideFromLeft) != 0)
                startX = -Math.Max(1, width);
            else if ((EnterAnimation & AnimationType.SlideFromRight) != 0)
                startX = Math.Max(1, width);
            if ((EnterAnimation & AnimationType.SlideFromTop) != 0)
                startY = -Math.Max(1, height);
            else if ((EnterAnimation & AnimationType.SlideFromBottom) != 0)
                startY = Math.Max(1, height);
            translate.X = startX;
            translate.Y = startY;

            await EnsureTargetHasSize(ModalContentBorder);

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
                    case CoreEnums.PopupAlignment.Center: x -= popupWidth / 2; y -= popupHeight / 2; break;
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
                    case CoreEnums.AnchorTarget.ScreenEdge:
                        anchorRect = null;
                        break;
                    case CoreEnums.AnchorTarget.CustomCoordinates:
                        anchorRect = new CoreRect(ModalCustomX, ModalCustomY, 1, 1);
                        break;
                    case CoreEnums.AnchorTarget.ParentWindowCenter:
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
                    ScreenEdge = ModalAnchorTarget == CoreEnums.AnchorTarget.ScreenEdge ? ModalScreenEdge : null,
                    CustomX = ModalAnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? ModalCustomX : null,
                    CustomY = ModalAnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates ? ModalCustomY : null
                };

                var finalPos = PositionCalculator.CalculatePosition(request, anchorRect, popupSize, screenBounds);
                ModalContentBorder.Margin = new Thickness(finalPos.X, finalPos.Y, 0, 0);
            }

            ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Left;
            ModalContentBorder.VerticalAlignment = VerticalAlignment.Top;

            OverlayGrid.Visibility = Visibility.Visible;
            await Dispatcher.InvokeAsync(() => ModalContentBorder.UpdateLayout(), DispatcherPriority.Render);

            IsOpen = true;
            Focusable = true;
            Focus();

            await EnsureTargetHasSize(ModalContentBorder);
            await AnimateEnterAsync(ModalContentBorder, EnterAnimation, EnterDuration, EnterEasing);
        }

        // ------------------------------------------------------------
        // Lightweight mode (unchanged, uses mouse placement built‑in)
        // ------------------------------------------------------------
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

            // Animation initial states
            if ((EnterAnimation & AnimationType.Fade) != 0)
                _lightweightContentHost.Opacity = 0;
            else
                _lightweightContentHost.Opacity = 1;

            if ((EnterAnimation & AnimationType.Scale) != 0)
            {
                var scale = (ScaleTransform)((TransformGroup)_lightweightContentHost.RenderTransform).Children[0];
                scale.ScaleX = scale.ScaleY = 0.8;
            }

            double startX = 0, startY = 0;
            var size = MeasureContentSize(_lightweightContentHost);
            if ((EnterAnimation & AnimationType.SlideFromLeft) != 0)
                startX = -size.Width;
            else if ((EnterAnimation & AnimationType.SlideFromRight) != 0)
                startX = size.Width;
            if ((EnterAnimation & AnimationType.SlideFromTop) != 0)
                startY = -size.Height;
            else if ((EnterAnimation & AnimationType.SlideFromBottom) != 0)
                startY = size.Height;
            var translate = (TranslateTransform)((TransformGroup)_lightweightContentHost.RenderTransform).Children[1];
            translate.X = startX;
            translate.Y = startY;

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

            await AnimateEnterAsync(_lightweightContentHost, EnterAnimation, EnterDuration, EnterEasing);
        }

        private CustomPopupPlacement[] OnCustomPopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            var anchorRect = GetAnchorRect();
            var screenBounds = GetScreenBounds();
            var corePopupSize = new CoreSize(popupSize.Width, popupSize.Height);
            CoreRect? coreAnchorRect = anchorRect.HasValue ? new CoreRect(anchorRect.Value.X, anchorRect.Value.Y, anchorRect.Value.Width, anchorRect.Value.Height) : null;

            var request = new PopupRequest
            {
                AnchorTarget = AnchorElement != null ? CoreEnums.AnchorTarget.UiElement : CoreEnums.AnchorTarget.MouseCursor,
                Alignment = CoreEnums.PopupAlignment.BottomCenter,
                Offset = (5, 5),
                AutoFlip = true
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
            return new[] { new CustomPopupPlacement(new System.Windows.Point(x, y), PopupPrimaryAxis.None) };
        }

        private Size MeasureContentSize(FrameworkElement element)
        {
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return element.DesiredSize;
        }

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

        private CoreRect GetScreenBounds()
        {
            var width = SystemParameters.PrimaryScreenWidth;
            var height = SystemParameters.PrimaryScreenHeight;
            return new CoreRect(0, 0, width, height);
        }

        public async Task CloseAsync()
        {
            if (!IsOpen) return;

            if (IsModal)
            {
                await AnimateExitAsync(ModalContentBorder, ExitAnimation, ExitDuration, ExitEasing);
                ResetTransforms(ModalContentBorder);
                ModalContentBorder.Margin = new Thickness(0);
                ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Center;
                ModalContentBorder.VerticalAlignment = VerticalAlignment.Center;
                OverlayGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (_lightweightContentHost != null)
                    await AnimateExitAsync(_lightweightContentHost, ExitAnimation, ExitDuration, ExitEasing);
                if (_lightweightPopup != null)
                {
                    _lightweightPopup.IsOpen = false;
                    _lightweightPopup = null;
                }
                _lightweightContentHost = null;
            }
            IsOpen = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        // ------------------------------------------------------------
        // Animation helpers (unchanged)
        // ------------------------------------------------------------
        private void ResetTransforms(FrameworkElement target)
        {
            if (target == null) return;
            var tg = target.RenderTransform as TransformGroup;
            if (tg != null && tg.Children.Count >= 2)
            {
                if (tg.Children[0] is ScaleTransform st)
                {
                    st.ScaleX = 1;
                    st.ScaleY = 1;
                    st.BeginAnimation(ScaleTransform.ScaleXProperty, null);
                    st.BeginAnimation(ScaleTransform.ScaleYProperty, null);
                }
                if (tg.Children[1] is TranslateTransform tt)
                {
                    tt.X = 0;
                    tt.Y = 0;
                    tt.BeginAnimation(TranslateTransform.XProperty, null);
                    tt.BeginAnimation(TranslateTransform.YProperty, null);
                }
            }
            target.BeginAnimation(UIElement.OpacityProperty, null);
            target.Opacity = 1;
        }

        private async Task AnimateEnterAsync(FrameworkElement target, AnimationType animation, int durationMs, EasingType easing)
        {
            EnsureTransformGroup(target);
            if (animation == AnimationType.None || durationMs <= 0)
            {
                target.Opacity = 1;
                return;
            }

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Loaded);
            target.UpdateLayout();
            await EnsureTargetHasSize(target);

            var tg = target.RenderTransform as TransformGroup;
            if (tg == null || tg.Children.Count < 2)
                throw new InvalidOperationException("Target must have a TransformGroup with ScaleTransform and TranslateTransform.");
            var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
            var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

            var duration = new Duration(TimeSpan.FromMilliseconds(durationMs));
            var easingFunc = GetEasingFunction(easing);
            var tcs = new TaskCompletionSource<bool>();
            bool completionHooked = false;

            if ((animation & AnimationType.Fade) != 0)
            {
                var fadeAnim = new DoubleAnimation { From = target.Opacity, To = 1.0, Duration = duration, EasingFunction = easingFunc };
                fadeAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                target.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }

            if ((animation & AnimationType.Scale) != 0)
            {
                var scaleXAnim = new DoubleAnimation { From = scale.ScaleX, To = 1.0, Duration = duration, EasingFunction = easingFunc };
                var scaleYAnim = new DoubleAnimation { From = scale.ScaleY, To = 1.0, Duration = duration, EasingFunction = easingFunc };
                scaleYAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
            }

            if ((animation & (AnimationType.SlideFromTop | AnimationType.SlideFromBottom)) != 0)
            {
                var slideYAnim = new DoubleAnimation { From = translate.Y, To = 0.0, Duration = duration, EasingFunction = easingFunc };
                slideYAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                translate.BeginAnimation(TranslateTransform.YProperty, slideYAnim);
            }

            if ((animation & (AnimationType.SlideFromLeft | AnimationType.SlideFromRight)) != 0)
            {
                var slideXAnim = new DoubleAnimation { From = translate.X, To = 0.0, Duration = duration, EasingFunction = easingFunc };
                slideXAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                translate.BeginAnimation(TranslateTransform.XProperty, slideXAnim);
            }

            var fallbackDelay = Task.Delay(durationMs + 50);
            var completed = await Task.WhenAny(tcs.Task, fallbackDelay);
            if (completed == fallbackDelay) tcs.TrySetResult(true);
            await tcs.Task;
        }

        private async Task AnimateExitAsync(FrameworkElement target, AnimationType animation, int durationMs, EasingType easing)
        {
            EnsureTransformGroup(target);
            if (animation == AnimationType.None || durationMs <= 0) return;

            await EnsureTargetHasSize(target);
            var tg = target.RenderTransform as TransformGroup;
            if (tg == null || tg.Children.Count < 2)
                throw new InvalidOperationException("Target must have a TransformGroup with ScaleTransform and TranslateTransform.");
            var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
            var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

            var duration = new Duration(TimeSpan.FromMilliseconds(durationMs));
            var easingFunc = GetEasingFunction(easing);
            var tcs = new TaskCompletionSource<bool>();
            bool completionHooked = false;

            if ((animation & AnimationType.Fade) != 0)
            {
                var fadeAnim = new DoubleAnimation { From = target.Opacity, To = 0.0, Duration = duration, EasingFunction = easingFunc };
                fadeAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                target.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }

            if ((animation & AnimationType.Scale) != 0)
            {
                var scaleXAnim = new DoubleAnimation { From = scale.ScaleX, To = 0.8, Duration = duration, EasingFunction = easingFunc };
                var scaleYAnim = new DoubleAnimation { From = scale.ScaleY, To = 0.8, Duration = duration, EasingFunction = easingFunc };
                scaleYAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
            }

            if ((animation & (AnimationType.SlideFromTop | AnimationType.SlideFromBottom)) != 0)
            {
                double to = (animation & AnimationType.SlideFromTop) != 0 ? -target.ActualHeight : target.ActualHeight;
                var slideAnim = new DoubleAnimation { From = translate.Y, To = to, Duration = duration, EasingFunction = easingFunc };
                slideAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                translate.BeginAnimation(TranslateTransform.YProperty, slideAnim);
            }

            if ((animation & (AnimationType.SlideFromLeft | AnimationType.SlideFromRight)) != 0)
            {
                double to = (animation & AnimationType.SlideFromLeft) != 0 ? -target.ActualWidth : target.ActualWidth;
                var slideAnim = new DoubleAnimation { From = translate.X, To = to, Duration = duration, EasingFunction = easingFunc };
                slideAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
                translate.BeginAnimation(TranslateTransform.XProperty, slideAnim);
            }

            var fallbackDelay = Task.Delay(durationMs + 50);
            var completed = await Task.WhenAny(tcs.Task, fallbackDelay);
            if (completed == fallbackDelay) tcs.TrySetResult(true);
            await tcs.Task;
        }

        private void EnsureTransformGroup(FrameworkElement target)
        {
            if (target == null) return;
            if (!(target.RenderTransform is TransformGroup))
            {
                target.RenderTransform = new TransformGroup
                {
                    Children = new TransformCollection
                    {
                        new ScaleTransform(1, 1),
                        new TranslateTransform(0, 0)
                    }
                };
            }
            target.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        private async Task EnsureTargetHasSize(FrameworkElement target, int timeoutMs = 500)
        {
            if (target == null) return;
            if (target.ActualWidth > 0 && target.ActualHeight > 0) return;
            await target.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    target.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    target.Arrange(new Rect(0, 0, target.DesiredSize.Width, target.DesiredSize.Height));
                    target.UpdateLayout();
                }
                catch { }
            }, DispatcherPriority.Loaded);
            if (target.ActualWidth > 0 && target.ActualHeight > 0) return;

            var tcs = new TaskCompletionSource<bool>();
            SizeChangedEventHandler? handler = null;
            handler = (s, e) =>
            {
                if (target.ActualWidth > 0 && target.ActualHeight > 0)
                {
                    target.SizeChanged -= handler;
                    tcs.TrySetResult(true);
                }
            };
            target.SizeChanged += handler;
            var delayTask = Task.Delay(timeoutMs);
            var completed = await Task.WhenAny(tcs.Task, delayTask);
            if (completed == delayTask) target.SizeChanged -= handler;
        }

        private IEasingFunction? GetEasingFunction(EasingType type)
        {
            return type switch
            {
                EasingType.Linear => null,
                EasingType.EaseIn => new QuadraticEase { EasingMode = EasingMode.EaseIn },
                EasingType.EaseOut => new QuadraticEase { EasingMode = EasingMode.EaseOut },
                EasingType.EaseInOut => new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                _ => null
            };
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsOpen && CanCloseOnEscape)
            {
                _ = CloseAsync();
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}