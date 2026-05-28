using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing all animation helper methods (enter/exit, transforms, size ensuring).
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Resets the render transforms (scale and translation) and opacity of a target element.
    /// Stops any ongoing animations and sets the element to its default visual state.
    /// </summary>
    /// <param name="target">The element to reset.</param>
    private static void ResetTransforms(FrameworkElement target)
    {
        if (target is null) return;
        if (target.RenderTransform is TransformGroup { Children.Count: >= 2 } tg)
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

#pragma warning disable CA1822 // Mark members as static - these methods use instance Dispatcher
    /// <summary>
    /// Animates the target element for the popup entering (opening) transition.
    /// </summary>
    /// <param name="target">The element to animate (usually <see cref="ModalContentBorder"/> or a lightweight popup root).</param>
    /// <param name="animation">The animation type(s) to apply (fade, scale, slide).</param>
    /// <param name="durationMs">Duration in milliseconds.</param>
    /// <param name="easing">The easing function to use.</param>
    private async Task AnimateEnterAsync(FrameworkElement target, CoreEnums.AnimationType animation, int durationMs, CoreEnums.EasingType easing)
    {
        EnsureTransformGroup(target);
        if (animation == CoreEnums.AnimationType.None || durationMs <= 0)
        {
            target.Opacity = 1;
            return;
        }

        await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Loaded);
        target.UpdateLayout();
        await EnsureTargetHasSize(target);

        if (target.RenderTransform is not TransformGroup tg || tg.Children.Count < 2)
            throw new InvalidOperationException("Target must have a TransformGroup with ScaleTransform and TranslateTransform.");
        var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
        var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

        var duration = new Duration(TimeSpan.FromMilliseconds(durationMs));
        var easingFunc = GetEasingFunction(easing);
        var tcs = new TaskCompletionSource<bool>();
        bool completionHooked = false;

        if ((animation & CoreEnums.AnimationType.Fade) != 0)
        {
            var fadeAnim = new DoubleAnimation { From = target.Opacity, To = 1.0, Duration = duration, EasingFunction = easingFunc };
            fadeAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            target.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
        }

        if ((animation & CoreEnums.AnimationType.Scale) != 0)
        {
            var scaleXAnim = new DoubleAnimation { From = scale.ScaleX, To = 1.0, Duration = duration, EasingFunction = easingFunc };
            var scaleYAnim = new DoubleAnimation { From = scale.ScaleY, To = 1.0, Duration = duration, EasingFunction = easingFunc };
            scaleYAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        }

        if ((animation & (CoreEnums.AnimationType.SlideTop | CoreEnums.AnimationType.SlideBottom)) != 0)
        {
            var slideYAnim = new DoubleAnimation { From = translate.Y, To = 0.0, Duration = duration, EasingFunction = easingFunc };
            slideYAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            translate.BeginAnimation(TranslateTransform.YProperty, slideYAnim);
        }

        if ((animation & (CoreEnums.AnimationType.SlideLeft | CoreEnums.AnimationType.SlideRight)) != 0)
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

    /// <summary>
    /// Animates the target element for the popup exiting (closing) transition.
    /// </summary>
    /// <param name="target">The element to animate.</param>
    /// <param name="animation">The animation type(s) to apply.</param>
    /// <param name="durationMs">Duration in milliseconds.</param>
    /// <param name="easing">The easing function to use.</param>
    private async Task AnimateExitAsync(FrameworkElement target, CoreEnums.AnimationType animation, int durationMs, CoreEnums.EasingType easing)
    {
        EnsureTransformGroup(target);
        if (animation == CoreEnums.AnimationType.None || durationMs <= 0) return;

        await EnsureTargetHasSize(target);
        if (target.RenderTransform is not TransformGroup tg || tg.Children.Count < 2)
            throw new InvalidOperationException("Target must have a TransformGroup with ScaleTransform and TranslateTransform.");
        var scale = tg.Children[0] as ScaleTransform ?? new ScaleTransform(1, 1);
        var translate = tg.Children[1] as TranslateTransform ?? new TranslateTransform(0, 0);

        var duration = new Duration(TimeSpan.FromMilliseconds(durationMs));
        var easingFunc = GetEasingFunction(easing);
        var tcs = new TaskCompletionSource<bool>();
        bool completionHooked = false;

        if ((animation & CoreEnums.AnimationType.Fade) != 0)
        {
            var fadeAnim = new DoubleAnimation { From = target.Opacity, To = 0.0, Duration = duration, EasingFunction = easingFunc };
            fadeAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            target.BeginAnimation(UIElement.OpacityProperty, fadeAnim);
        }

        if ((animation & CoreEnums.AnimationType.Scale) != 0)
        {
            var scaleXAnim = new DoubleAnimation { From = scale.ScaleX, To = 0.8, Duration = duration, EasingFunction = easingFunc };
            var scaleYAnim = new DoubleAnimation { From = scale.ScaleY, To = 0.8, Duration = duration, EasingFunction = easingFunc };
            scaleYAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        }

        if ((animation & (CoreEnums.AnimationType.SlideTop | CoreEnums.AnimationType.SlideBottom)) != 0)
        {
            double to = (animation & CoreEnums.AnimationType.SlideTop) != 0 ? -target.ActualHeight : target.ActualHeight;
            var slideAnim = new DoubleAnimation { From = translate.Y, To = to, Duration = duration, EasingFunction = easingFunc };
            slideAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            translate.BeginAnimation(TranslateTransform.YProperty, slideAnim);
        }

        if ((animation & (CoreEnums.AnimationType.SlideLeft | CoreEnums.AnimationType.SlideRight)) != 0)
        {
            double to = (animation & CoreEnums.AnimationType.SlideLeft) != 0 ? -target.ActualWidth : target.ActualWidth;
            var slideAnim = new DoubleAnimation { From = translate.X, To = to, Duration = duration, EasingFunction = easingFunc };
            slideAnim.Completed += (s, e) => { if (!completionHooked) { completionHooked = true; tcs.TrySetResult(true); } };
            translate.BeginAnimation(TranslateTransform.XProperty, slideAnim);
        }

        var fallbackDelay = Task.Delay(durationMs + 50);
        var completed = await Task.WhenAny(tcs.Task, fallbackDelay);
        if (completed == fallbackDelay) tcs.TrySetResult(true);
        await tcs.Task;
    }
#pragma warning restore CA1822

    /// <summary>
    /// Ensures that the target element has a TransformGroup with a ScaleTransform and a TranslateTransform,
    /// and that its RenderTransformOrigin is set to (0.5,0.5).
    /// </summary>
    /// <param name="target">The element to check/initialize.</param>
    private static void EnsureTransformGroup(FrameworkElement target)
    {
        if (target is null) return;
        if (target.RenderTransform is not TransformGroup)
        {
            target.RenderTransform = new TransformGroup
            {
                // Collection expression for simplified initialization (C# 12)
                Children = [new ScaleTransform(1, 1), new TranslateTransform(0, 0)]
            };
        }
        target.RenderTransformOrigin = new Point(0.5, 0.5);
    }

    /// <summary>
    /// Ensures that the target element has a non‑zero width and height.
    /// If not, it forces a layout pass and optionally waits for a SizeChanged event up to a timeout.
    /// </summary>
    /// <param name="target">The element to check.</param>
    /// <param name="timeoutMs">Timeout in milliseconds (default 500).</param>
    private static async Task EnsureTargetHasSize(FrameworkElement target, int timeoutMs = 500)
    {
        if (target is null) return;
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

        void OnSizeChanged(object? _, SizeChangedEventArgs __)
        {
            if (target.ActualWidth > 0 && target.ActualHeight > 0)
            {
                target.SizeChanged -= OnSizeChanged;
                tcs.TrySetResult(true);
            }
        }

        target.SizeChanged += OnSizeChanged;
        var delayTask = Task.Delay(timeoutMs);
        var completed = await Task.WhenAny(tcs.Task, delayTask);
        if (completed == delayTask) target.SizeChanged -= OnSizeChanged;
    }

    /// <summary>
    /// Returns a concrete <see cref="QuadraticEase"/> easing function for the given easing type,
    /// or <c>null</c> for linear or cubic Bézier (which requires additional handling).
    /// </summary>
    /// <param name="type">The easing type.</param>
    /// <returns>A <see cref="QuadraticEase"/> instance or <c>null</c>.</returns>
    private static QuadraticEase? GetEasingFunction(CoreEnums.EasingType type)
    {
        return type switch
        {
            CoreEnums.EasingType.Linear => null,
            CoreEnums.EasingType.EaseIn => new QuadraticEase { EasingMode = EasingMode.EaseIn },
            CoreEnums.EasingType.EaseOut => new QuadraticEase { EasingMode = EasingMode.EaseOut },
            CoreEnums.EasingType.EaseInOut => new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            CoreEnums.EasingType.CubicBezier => null,
            _ => null
        };
    }
}