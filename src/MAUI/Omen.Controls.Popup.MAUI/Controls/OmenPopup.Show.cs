using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.MAUI.Controls;

public partial class OmenPopup
{
    public async Task ShowAsync()
    {
        if (_host == null) return;
        if (IsOpen) return;

        Opening?.Invoke(this, EventArgs.Empty);

        var request = BuildRequest();

        if (IsModal)
        {
            await ShowModalAsync(request);
        }
        else
        {
            await ShowLightweightAsync(request);
        }
    }

    private async Task ShowModalAsync(PopupRequest request)
    {
        await _host!.ShowAsync(request);
        IsOpen = true;
        Opened?.Invoke(this, EventArgs.Empty);
    }

    private async Task ShowLightweightAsync(PopupRequest request)
    {
        // Lightweight popup - small floating popup without overlay
        // For now, treat like modal but with different styling
        await ShowModalAsync(request);
    }

    /// <summary>
    /// Applies animation to a view during popup entry
    /// </summary>
    public async Task ApplyEnterAnimationAsync(View target, int duration)
    {
        if (target == null) return;

        var tasks = new List<Task>();

        try
        {
            // Fade animation
            if ((EnterAnimation & AnimationType.Fade) != 0)
            {
                target.Opacity = 0;
                tasks.Add(target.FadeToAsync(1, (uint)duration));
            }

            // Scale animation
            if ((EnterAnimation & AnimationType.Scale) != 0)
            {
                target.Scale = 0.8;
                tasks.Add(target.ScaleToAsync(1, (uint)duration, GetEasingFunction(EnterEasing)));
            }

            // Slide animation
            if ((EnterAnimation & AnimationType.SlideLeft) != 0)
                tasks.Add(target.TranslateToAsync(-100, 0, (uint)duration, GetEasingFunction(EnterEasing)));
            else if ((EnterAnimation & AnimationType.SlideRight) != 0)
                tasks.Add(target.TranslateToAsync(100, 0, (uint)duration, GetEasingFunction(EnterEasing)));
            else if ((EnterAnimation & AnimationType.SlideTop) != 0)
                tasks.Add(target.TranslateToAsync(0, -100, (uint)duration, GetEasingFunction(EnterEasing)));
            else if ((EnterAnimation & AnimationType.SlideBottom) != 0)
                tasks.Add(target.TranslateToAsync(0, 100, (uint)duration, GetEasingFunction(EnterEasing)));

            if (tasks.Count > 0)
                await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex}");
        }
    }

    /// <summary>
    /// Applies animation to a view during popup exit
    /// </summary>
    public async Task ApplyExitAnimationAsync(View target, int duration)
    {
        if (target == null) return;

        var tasks = new List<Task>();

        try
        {
            // Fade animation
            if ((ExitAnimation & AnimationType.Fade) != 0)
                tasks.Add(target.FadeToAsync(0, (uint)duration));

            // Scale animation
            if ((ExitAnimation & AnimationType.Scale) != 0)
                tasks.Add(target.ScaleToAsync(0.8, (uint)duration, GetEasingFunction(ExitEasing)));

            // Slide animation
            if ((ExitAnimation & AnimationType.SlideLeft) != 0)
                tasks.Add(target.TranslateToAsync(-100, 0, (uint)duration, GetEasingFunction(ExitEasing)));
            else if ((ExitAnimation & AnimationType.SlideRight) != 0)
                tasks.Add(target.TranslateToAsync(100, 0, (uint)duration, GetEasingFunction(ExitEasing)));
            else if ((ExitAnimation & AnimationType.SlideTop) != 0)
                tasks.Add(target.TranslateToAsync(0, -100, (uint)duration, GetEasingFunction(ExitEasing)));
            else if ((ExitAnimation & AnimationType.SlideBottom) != 0)
                tasks.Add(target.TranslateToAsync(0, 100, (uint)duration, GetEasingFunction(ExitEasing)));

            if (tasks.Count > 0)
                await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Animation error: {ex}");
        }
    }

    private Easing GetEasingFunction(EasingType easingType)
    {
        return easingType switch
        {
            EasingType.Linear => Easing.Linear,
            EasingType.EaseIn => Easing.CubicIn,
            EasingType.EaseOut => Easing.CubicOut,
            EasingType.EaseInOut => Easing.CubicInOut,
            EasingType.CubicBezier => Easing.CubicInOut, // Fallback for cubic bezier
            _ => Easing.Linear
        };
    }

    private PopupRequest BuildRequest()
    {
        return new PopupRequest
        {
            Content = this.Content,
            IsModal = this.IsModal,
            CloseOnOutsideClick = this.CloseOnOutsideClick,
            OverlayBrush = this.OverlayBrush?.ToString(),  // TODO: change PopupRequest.OverlayBrush to Brush type
            CloseButtonTemplate = this.CloseButtonTemplate,
            EnterAnimation = this.EnterAnimation,
            ExitAnimation = this.ExitAnimation,
            EnterDuration = this.EnterDuration,
            ExitDuration = this.ExitDuration,
            EnterEasing = this.EnterEasing,
            ExitEasing = this.ExitEasing,
            AnchorTarget = this.AnchorTarget,
            AnchorElement = this.AnchorElement,
            Alignment = this.Alignment,
            Offset = (this.OffsetX, this.OffsetY),
            AutoFlip = this.AutoFlip,
            CustomX = this.CustomX,
            CustomY = this.CustomY,
            DialogButtons = this.DialogButtons,
            CustomButtonLabels = this.CustomButtonLabels
        };
    }
}