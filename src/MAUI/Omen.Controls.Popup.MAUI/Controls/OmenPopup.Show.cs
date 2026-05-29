using Microsoft.Maui.Controls;
using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Events;
using Omen.Controls.Popup.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Omen.Controls.Popup.MAUI.Controls;

public partial class OmenPopup
{
    public async Task ShowAsync()
    {
        if (_host == null) return;
        if (IsOpen) return;

        // Raise Opening event with cancellation support
        var openingArgs = new OpeningCancelEventArgs();
        Opening?.Invoke(this, openingArgs);
        if (openingArgs.Cancel) return;

        var request = BuildRequest();

        // Wire host events to control events (if not already wired)
        _host.Opening -= OnHostOpening;
        _host.Opening += OnHostOpening;
        _host.Opened -= OnHostOpened;
        _host.Opened += OnHostOpened;
        _host.Closing -= OnHostClosing;
        _host.Closing += OnHostClosing;
        _host.Closed -= OnHostClosed;
        _host.Closed += OnHostClosed;

        // Show the popup (host handles modal/lightweight via request.IsModal)
        await _host.ShowAsync(request);
        IsOpen = true;
        Opened?.Invoke(this, EventArgs.Empty);
    }

    public async Task<DialogClosedEventArgs> ShowDialogAsync()
    {
        _dialogTcs = new TaskCompletionSource<DialogAction>();
        await ShowAsync();
        var result = await _dialogTcs.Task;
        return new DialogClosedEventArgs { Result = result };
    }

    private Task OnHostOpening(CancellableEventArgs args)
    {
        var cancelArgs = new OpeningCancelEventArgs();
        Opening?.Invoke(this, cancelArgs);
        if (cancelArgs.Cancel) args.Cancel = true;
        return Task.CompletedTask;
    }

    private Task OnHostOpened()
    {
        Opened?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    private Task OnHostClosing(CancellableEventArgs args)
    {
        var cancelArgs = new ClosingCancelEventArgs();
        Closing?.Invoke(this, cancelArgs);
        if (cancelArgs.Cancel) args.Cancel = true;
        return Task.CompletedTask;
    }

    private Task OnHostClosed(DialogAction? result, object? userData)
    {
        IsOpen = false;
        var dialogArgs = new DialogClosedEventArgs { Result = result, UserData = userData };
        DialogClosed?.Invoke(this, dialogArgs);
        _dialogTcs?.TrySetResult(result ?? DialogAction.None);
        return Task.CompletedTask;
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
            if ((EnterAnimation & AnimationType.Fade) != 0)
            {
                target.Opacity = 0;
                tasks.Add(target.FadeToAsync(1, (uint)duration));
            }

            if ((EnterAnimation & AnimationType.Scale) != 0)
            {
                target.Scale = 0.8;
                tasks.Add(target.ScaleToAsync(1, (uint)duration, GetEasingFunction(EnterEasing)));
            }

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
            if ((ExitAnimation & AnimationType.Fade) != 0)
                tasks.Add(target.FadeToAsync(0, (uint)duration));

            if ((ExitAnimation & AnimationType.Scale) != 0)
                tasks.Add(target.ScaleToAsync(0.8, (uint)duration, GetEasingFunction(ExitEasing)));

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
            EasingType.CubicBezier => Easing.CubicInOut,
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
            ShowCloseButton = this.ShowCloseButton,          // Added
            CloseButtonTemplate = this.CloseButtonTemplate,
            OverlayBrush = this.OverlayBrush?.ToString(),
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