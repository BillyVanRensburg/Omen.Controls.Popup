using Omen.Controls.Popup.Core.Interfaces;
using Omen.Controls.Popup.Core.Models;
using Omen.Controls.Popup.Core.Events;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.MAUI.Hosting;

public class MauiPopupHost : IPopupHost
{
    private Grid? _overlayGrid;
    private ContentPage? _parentPage;
    private PopupRequest? _currentRequest;
    private Border? _contentWrapper;
    private Grid? _rootGrid;           // Keep the wrapper grid
    private View? _originalContent;    // Keep original content reference

    public event Func<CancellableEventArgs, Task>? Opening;
    public event Func<Task>? Opened;
    public event Func<CancellableEventArgs, Task>? Closing;
    public event Func<CoreEnums.DialogAction?, object?, Task>? Closed;

    public async Task ShowAsync(PopupRequest request, CancellationToken cancellationToken = default)
    {
        _currentRequest = request;

        var args = new CancellableEventArgs();
        if (Opening != null) await Opening(args);
        if (args.Cancel) return;

        _parentPage = GetCurrentPage();
        if (_parentPage == null) return;

        // Ensure we have a root grid wrapper
        if (_parentPage.Content is Grid existingGrid && existingGrid != _rootGrid)
        {
            _rootGrid = existingGrid;
            _originalContent = null; // We cannot know original content; fallback
        }
        else if (_parentPage.Content is View originalView)
        {
            _originalContent = originalView;
            _rootGrid = new Grid();
            _rootGrid.Children.Add(_originalContent);
            _parentPage.Content = _rootGrid;
        }

        // Create overlay grid
        _overlayGrid = new Grid
        {
            BackgroundColor = ParseColorFromBrush(request.OverlayBrush?.ToString() ?? "#80000000"),
            Opacity = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            InputTransparent = false
        };

        if (request.CloseOnOutsideClick)
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) => await CloseAsync();
            _overlayGrid.GestureRecognizers.Add(tapGestureRecognizer);
        }

        _contentWrapper = new Border
        {
            Content = CreateContentView(request.Content),
            StrokeThickness = 0,
            Padding = new Thickness(15),
            Margin = new Thickness(20),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.White,
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Offset = new Point(2, 2),
                Radius = 8,
                Opacity = 0.3f
            }
        };

        var frameTap = new TapGestureRecognizer();
        frameTap.Tapped += (s, e) => { };
        _contentWrapper.GestureRecognizers.Add(frameTap);

        if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.Fade) != 0)
            _contentWrapper.Opacity = 0;

        _overlayGrid.Children.Add(_contentWrapper);
        _rootGrid?.Children.Add(_overlayGrid);

        await Task.Delay(50, cancellationToken);
        await _overlayGrid.FadeTo(1, 250);

        var contentTasks = new List<Task>();

        if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.Fade) != 0)
            contentTasks.Add(_contentWrapper.FadeTo(1, (uint)_currentRequest.EnterDuration));

        if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
        {
            _contentWrapper.Scale = 0.8;
            contentTasks.Add(_contentWrapper.ScaleTo(1, (uint)_currentRequest.EnterDuration, GetEasing(_currentRequest.EnterEasing)));
        }

        if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.SlideLeft) != 0)
            contentTasks.Add(_contentWrapper.TranslateTo(-100, 0, (uint)_currentRequest.EnterDuration, GetEasing(_currentRequest.EnterEasing)));
        else if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.SlideRight) != 0)
            contentTasks.Add(_contentWrapper.TranslateTo(100, 0, (uint)_currentRequest.EnterDuration, GetEasing(_currentRequest.EnterEasing)));
        else if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.SlideTop) != 0)
            contentTasks.Add(_contentWrapper.TranslateTo(0, -100, (uint)_currentRequest.EnterDuration, GetEasing(_currentRequest.EnterEasing)));
        else if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.SlideBottom) != 0)
            contentTasks.Add(_contentWrapper.TranslateTo(0, 100, (uint)_currentRequest.EnterDuration, GetEasing(_currentRequest.EnterEasing)));

        if (contentTasks.Count > 0)
            await Task.WhenAll(contentTasks);

        if (Opened != null) await Opened();
    }

    public async Task CloseAsync()
    {
        if (_overlayGrid == null || _rootGrid == null) return;

        var args = new CancellableEventArgs();
        if (Closing != null) await Closing(args);
        if (args.Cancel) return;

        // Exit animations
        var overlayTask = _overlayGrid.FadeTo(0, 200);
        var contentTasks = new List<Task>();

        if (_contentWrapper != null)
        {
            if ((_currentRequest?.ExitAnimation & CoreEnums.AnimationType.Fade) != 0)
                contentTasks.Add(_contentWrapper.FadeTo(0, (uint)(_currentRequest?.ExitDuration ?? 200)));

            if ((_currentRequest?.ExitAnimation & CoreEnums.AnimationType.Scale) != 0)
                contentTasks.Add(_contentWrapper.ScaleTo(0.8, (uint)(_currentRequest?.ExitDuration ?? 200), GetEasing(_currentRequest?.ExitEasing ?? CoreEnums.EasingType.EaseIn)));

            if ((_currentRequest?.ExitAnimation & CoreEnums.AnimationType.SlideLeft) != 0)
                contentTasks.Add(_contentWrapper.TranslateTo(-100, 0, (uint)(_currentRequest?.ExitDuration ?? 200), GetEasing(_currentRequest?.ExitEasing ?? CoreEnums.EasingType.EaseIn)));
            else if ((_currentRequest?.ExitAnimation & CoreEnums.AnimationType.SlideRight) != 0)
                contentTasks.Add(_contentWrapper.TranslateTo(100, 0, (uint)(_currentRequest?.ExitDuration ?? 200), GetEasing(_currentRequest?.ExitEasing ?? CoreEnums.EasingType.EaseIn)));
            else if ((_currentRequest?.ExitAnimation & CoreEnums.AnimationType.SlideTop) != 0)
                contentTasks.Add(_contentWrapper.TranslateTo(0, -100, (uint)(_currentRequest?.ExitDuration ?? 200), GetEasing(_currentRequest?.ExitEasing ?? CoreEnums.EasingType.EaseIn)));
            else if ((_currentRequest?.ExitAnimation & CoreEnums.AnimationType.SlideBottom) != 0)
                contentTasks.Add(_contentWrapper.TranslateTo(0, 100, (uint)(_currentRequest?.ExitDuration ?? 200), GetEasing(_currentRequest?.ExitEasing ?? CoreEnums.EasingType.EaseIn)));
        }

        await overlayTask;
        if (contentTasks.Count > 0)
            await Task.WhenAll(contentTasks);

        // Clean up
        if (_contentWrapper != null)
        {
            _contentWrapper.Content = null;
            _contentWrapper.GestureRecognizers.Clear();
        }
        _overlayGrid.Children.Clear();
        _overlayGrid.GestureRecognizers.Clear();

        // Remove overlay from root grid – no Content reassignment!
        if (_rootGrid.Children.Contains(_overlayGrid))
            _rootGrid.Children.Remove(_overlayGrid);

        _overlayGrid = null;
        _contentWrapper = null;

        if (Closed != null) await Closed(null, null);
    }

    public Task UpdatePositionAsync() => Task.CompletedTask;

    private View CreateContentView(object? content)
    {
        if (content is View view) return view;
        return new Label
        {
            Text = content?.ToString() ?? "",
            BackgroundColor = Colors.White,
            Padding = new Thickness(10)
        };
    }

    private ContentPage? GetCurrentPage()
    {
        var window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (window?.Page is Shell shell)
            return shell.CurrentPage as ContentPage;
        if (window?.Page is NavigationPage nav)
            return nav.CurrentPage as ContentPage;
        return window?.Page as ContentPage;
    }

    private Color ParseColorFromBrush(string brushString)
    {
        if (brushString.StartsWith("#") && brushString.Length == 9)
            return Color.FromArgb(brushString);
        return Colors.Black;
    }

    private Easing GetEasing(CoreEnums.EasingType? easingType)
    {
        return easingType switch
        {
            CoreEnums.EasingType.Linear => Easing.Linear,
            CoreEnums.EasingType.EaseIn => Easing.CubicIn,
            CoreEnums.EasingType.EaseOut => Easing.CubicOut,
            CoreEnums.EasingType.EaseInOut => Easing.CubicInOut,
            _ => Easing.Linear
        };
    }
}