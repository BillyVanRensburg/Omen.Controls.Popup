using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

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
        #region Dependency Properties

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

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(OmenPopup));
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
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

        #endregion

        public event EventHandler? Closed;

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

            // Ensure transforms exist immediately
            EnsureTransformGroup(ModalContentBorder);

            // --- Set initial start-state BEFORE making visible to avoid first-frame stutter ---
            // Initial opacity depends on Fade flag
            ModalContentBorder.Opacity = ((EnterAnimation & AnimationType.Fade) != 0) ? 0.0 : 1.0;

            // Get transforms
            var tg = ModalContentBorder.RenderTransform as TransformGroup;
            if (tg == null || tg.Children.Count < 2)
                throw new InvalidOperationException("ModalContentBorder must have a TransformGroup with ScaleTransform and TranslateTransform.");

            var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
            var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

            // Set initial scale
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

            // Compute start offsets using Actual or Desired size
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

            // Make visible after initial state is set
            OverlayGrid.Visibility = Visibility.Visible;

            // Force a layout/render pass so the renderer sees the start state as the first frame
            await Dispatcher.InvokeAsync(() =>
            {
                ModalContentBorder.UpdateLayout();
            }, DispatcherPriority.Render);

            // Mark open and focus, then animate
            IsOpen = true;
            Focusable = true;
            Focus();

            await EnsureTargetHasSize(ModalContentBorder);
            await AnimateEnterAsync(ModalContentBorder, EnterAnimation, EnterDuration, EnterEasing);
        }

        public async Task CloseAsync()
        {
            if (!IsOpen) return;

            EnsureTransformGroup(ModalContentBorder);

            await AnimateExitAsync(ModalContentBorder, ExitAnimation, ExitDuration, ExitEasing);
            ResetTransforms(ModalContentBorder);
            OverlayGrid.Visibility = Visibility.Collapsed;
            IsOpen = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }

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

            // Ensure layout and size
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Loaded);
            target.UpdateLayout();
            await EnsureTargetHasSize(target);

            var tg = target.RenderTransform as TransformGroup;
            if (tg == null || tg.Children.Count < 2)
                throw new InvalidOperationException("Target must have a TransformGroup with ScaleTransform and TranslateTransform. Call EnsureTransformGroup(target) first.");

            var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
            var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

            // Prepare animations with explicit From/To values
            var duration = new Duration(TimeSpan.FromMilliseconds(durationMs));
            var easingFunc = GetEasingFunction(easing);

            var tcs = new TaskCompletionSource<bool>();
            bool completionHooked = false;

            // Fade animation (From should match the start-state set in ShowAsync)
            if ((animation & AnimationType.Fade) != 0)
            {
                var fadeAnim = new DoubleAnimation
                {
                    From = target.Opacity,
                    To = 1.0,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                fadeAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                target.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }

            // Scale animations
            if ((animation & AnimationType.Scale) != 0)
            {
                var scaleXAnim = new DoubleAnimation
                {
                    From = scale.ScaleX,
                    To = 1.0,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                var scaleYAnim = new DoubleAnimation
                {
                    From = scale.ScaleY,
                    To = 1.0,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                scaleYAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
            }

            // Slide Y
            bool hasSlideY = (animation & (AnimationType.SlideFromTop | AnimationType.SlideFromBottom)) != 0;
            if (hasSlideY)
            {
                var slideYAnim = new DoubleAnimation
                {
                    From = translate.Y,
                    To = 0.0,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                slideYAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                translate.BeginAnimation(TranslateTransform.YProperty, slideYAnim);
            }

            // Slide X
            bool hasSlideX = (animation & (AnimationType.SlideFromLeft | AnimationType.SlideFromRight)) != 0;
            if (hasSlideX)
            {
                var slideXAnim = new DoubleAnimation
                {
                    From = translate.X,
                    To = 0.0,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                slideXAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                translate.BeginAnimation(TranslateTransform.XProperty, slideXAnim);
            }

            // If no animation hooked completion (shouldn't happen), fallback to delay
            var fallbackDelay = Task.Delay(durationMs + 50);
            var completed = await Task.WhenAny(tcs.Task, fallbackDelay);
            if (completed == fallbackDelay)
                tcs.TrySetResult(true);

            await tcs.Task;
        }

        private async Task AnimateExitAsync(FrameworkElement target, AnimationType animation, int durationMs, EasingType easing)
        {
            EnsureTransformGroup(target);

            if (animation == AnimationType.None || durationMs <= 0)
                return;

            await EnsureTargetHasSize(target);
            var tg = target.RenderTransform as TransformGroup;
            if (tg == null || tg.Children.Count < 2)
                throw new InvalidOperationException("Target must have a TransformGroup with ScaleTransform and TranslateTransform. Call EnsureTransformGroup(target) first.");

            var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
            var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

            var duration = new Duration(TimeSpan.FromMilliseconds(durationMs));
            var easingFunc = GetEasingFunction(easing);

            var tcs = new TaskCompletionSource<bool>();
            bool completionHooked = false;

            // Fade out
            if ((animation & AnimationType.Fade) != 0)
            {
                var fadeAnim = new DoubleAnimation
                {
                    From = target.Opacity,
                    To = 0.0,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                fadeAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                target.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
            }

            // Scale down
            if ((animation & AnimationType.Scale) != 0)
            {
                var scaleXAnim = new DoubleAnimation
                {
                    From = scale.ScaleX,
                    To = 0.8,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                var scaleYAnim = new DoubleAnimation
                {
                    From = scale.ScaleY,
                    To = 0.8,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                scaleYAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
            }

            // Slide out Y
            if ((animation & (AnimationType.SlideFromTop | AnimationType.SlideFromBottom)) != 0)
            {
                double to = (animation & AnimationType.SlideFromTop) != 0 ? -target.ActualHeight : target.ActualHeight;
                var slideAnim = new DoubleAnimation
                {
                    From = translate.Y,
                    To = to,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                slideAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                translate.BeginAnimation(TranslateTransform.YProperty, slideAnim);
            }

            // Slide out X
            if ((animation & (AnimationType.SlideFromLeft | AnimationType.SlideFromRight)) != 0)
            {
                double to = (animation & AnimationType.SlideFromLeft) != 0 ? -target.ActualWidth : target.ActualWidth;
                var slideAnim = new DoubleAnimation
                {
                    From = translate.X,
                    To = to,
                    Duration = duration,
                    EasingFunction = easingFunc
                };
                slideAnim.Completed += (s, e) =>
                {
                    if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); }
                };
                translate.BeginAnimation(TranslateTransform.XProperty, slideAnim);
            }

            var fallbackDelay = Task.Delay(durationMs + 50);
            var completed = await Task.WhenAny(tcs.Task, fallbackDelay);
            if (completed == fallbackDelay)
                tcs.TrySetResult(true);

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

            if (target.ActualWidth > 0 && target.ActualHeight > 0)
                return;

            await target.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    target.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    target.Arrange(new Rect(0, 0, target.DesiredSize.Width, target.DesiredSize.Height));
                    target.UpdateLayout();
                }
                catch
                {
                }
            }, DispatcherPriority.Loaded);

            if (target.ActualWidth > 0 && target.ActualHeight > 0)
                return;

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
            if (completed == delayTask)
            {
                target.SizeChanged -= handler;
            }
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
