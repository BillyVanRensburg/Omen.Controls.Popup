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
    private Border? _popupContainer;
    private Grid? _rootGrid;
    private View? _originalContent;
    private TaskCompletionSource<CoreEnums.DialogAction?>? _dialogTcs;

    public event Func<CancellableEventArgs, Task>? Opening;
    public event Func<Task>? Opened;
    public event Func<CancellableEventArgs, Task>? Closing;
    public event Func<CoreEnums.DialogAction?, object?, Task>? Closed;

    public async Task CloseAsync()
    {
        await CloseAsync(null, null);
    }

    public async Task CloseAsync(CoreEnums.DialogAction? dialogResult, object? userData = null)
    {
        if (_overlayGrid == null || _rootGrid == null) return;

        var args = new CancellableEventArgs();
        if (Closing != null) await Closing(args);
        if (args.Cancel) return;

        await AnimateExitAsync(_popupContainer, _currentRequest);
        if (_currentRequest?.IsModal == true)
            await _overlayGrid.FadeToAsync(0, 200);
        else
            await _overlayGrid.FadeToAsync(0, 100);

        if (_popupContainer != null)
        {
            _popupContainer.Content = null;
            _popupContainer.GestureRecognizers.Clear();
        }
        _overlayGrid.Children.Clear();
        _overlayGrid.GestureRecognizers.Clear();
        if (_rootGrid.Children.Contains(_overlayGrid))
            _rootGrid.Children.Remove(_overlayGrid);

        _overlayGrid = null;
        _popupContainer = null;

        _dialogTcs?.TrySetResult(dialogResult);
        _dialogTcs = null;

        if (Closed != null) await Closed(dialogResult, userData);
    }

    public async Task ShowAsync(PopupRequest request, CancellationToken cancellationToken = default)
    {
        _currentRequest = request;

        var args = new CancellableEventArgs();
        if (Opening != null) await Opening(args);
        if (args.Cancel) return;

        _parentPage = GetCurrentPage();
        if (_parentPage == null) return;

        if (_parentPage.Content is Grid grid && grid != _rootGrid)
        {
            _rootGrid = grid;
            _originalContent = null;
        }
        else if (_parentPage.Content is View originalView)
        {
            _originalContent = originalView;
            _rootGrid = new Grid();
            _rootGrid.Children.Add(_originalContent);
            _parentPage.Content = _rootGrid;
        }

        _overlayGrid = new Grid
        {
            BackgroundColor = request.IsModal ? ParseColorFromBrush(request.OverlayBrush?.ToString() ?? "#80000000") : Colors.Transparent,
            Opacity = request.IsModal ? 0 : 1,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            InputTransparent = !request.IsModal
        };

        if (request.CloseOnOutsideClick && request.IsModal)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) => await CloseAsync();
            _overlayGrid.GestureRecognizers.Add(tapGesture);
        }

        var contentWithButtons = BuildContentWithButtons(request);
        _popupContainer = new Border
        {
            Content = contentWithButtons,
            StrokeThickness = 0,
            Padding = new Thickness(0),
            Margin = new Thickness(20),
            BackgroundColor = Colors.White,
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Offset = new Point(2, 2),
                Radius = 8,
                Opacity = 0.3f
            }
        };

        var blockTap = new TapGestureRecognizer();
        blockTap.Tapped += (s, e) => { };
        _popupContainer.GestureRecognizers.Add(blockTap);

        if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.Fade) != 0)
            _popupContainer.Opacity = 0;
        if ((_currentRequest.EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
            _popupContainer.Scale = 0.8;

        if (_currentRequest.AnchorTarget != CoreEnums.AnchorTarget.ParentContainer)
            await PositionPopupAsync(_popupContainer, _currentRequest);

        _overlayGrid.Children.Add(_popupContainer);
        _rootGrid?.Children.Add(_overlayGrid);
        await Task.Delay(50, cancellationToken);

        if (request.IsModal)
            await _overlayGrid.FadeToAsync(1, 250);

        await AnimateEntryAsync(_popupContainer, _currentRequest);

        if (Opened != null) await Opened();
    }

    public async Task<CoreEnums.DialogAction?> ShowDialogAsync(PopupRequest request, CancellationToken cancellationToken = default)
    {
        _dialogTcs = new TaskCompletionSource<CoreEnums.DialogAction?>();
        await ShowAsync(request, cancellationToken);
        return await _dialogTcs.Task;
    }

    public Task UpdatePositionAsync() => Task.CompletedTask;

    // ----------------------------------------------------------------------
    // Private helpers
    // ----------------------------------------------------------------------

    private View BuildContentWithButtons(PopupRequest request)
    {
        var mainLayout = new VerticalStackLayout { Spacing = 10, Padding = new Thickness(15) };

        if (request.Content is View userView)
            mainLayout.Children.Add(userView);
        else if (request.Content != null)
            mainLayout.Children.Add(new Label { Text = request.Content.ToString() });

        if (request.ShowCloseButton && request.CloseButtonTemplate != null)
        {
            // CloseButtonTemplate is stored as object? but should be ControlTemplate
            if (request.CloseButtonTemplate is ControlTemplate template)
            {
                var content = template.CreateContent();
                if (content is Button btn)
                {
                    btn.Clicked += async (s, e) => await CloseAsync();
                    mainLayout.Children.Add(btn);
                }
                else
                {
                    // Fallback
                    var fallbackBtn = new Button { Text = "✖", BackgroundColor = Colors.Transparent, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.End };
                    fallbackBtn.Clicked += async (s, e) => await CloseAsync();
                    mainLayout.Children.Add(fallbackBtn);
                }
            }
            else
            {
                // Fallback if not a ControlTemplate
                var fallbackBtn = new Button { Text = "✖", BackgroundColor = Colors.Transparent, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.End };
                fallbackBtn.Clicked += async (s, e) => await CloseAsync();
                mainLayout.Children.Add(fallbackBtn);
            }
        }
        else if (request.ShowCloseButton)
        {
            // Default close button
            var closeBtn = new Button { Text = "✖", BackgroundColor = Colors.Transparent, TextColor = Colors.Gray, HorizontalOptions = LayoutOptions.End };
            closeBtn.Clicked += async (s, e) => await CloseAsync();
            mainLayout.Children.Add(closeBtn);
        }

        var buttons = GetDialogButtons(request.DialogButtons, request.CustomButtonLabels);
        if (buttons.Count > 0)
        {
            var buttonPanel = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 10, 0, 0) };
            foreach (var (action, label) in buttons)
            {
                var btn = new Button { Text = label, BackgroundColor = Colors.LightGray, TextColor = Colors.Black };
                btn.Clicked += (s, e) => CloseAsync(action);
                buttonPanel.Children.Add(btn);
            }
            mainLayout.Children.Add(buttonPanel);
        }

        return mainLayout;
    }

    private List<(CoreEnums.DialogAction, string)> GetDialogButtons(CoreEnums.DialogAction actions, Dictionary<string, string>? customLabels)
    {
        var list = new List<(CoreEnums.DialogAction, string)>();
        if (actions.HasFlag(CoreEnums.DialogAction.OK))
        {
            string label = (customLabels != null && customLabels.TryGetValue("OK", out var okLabel)) ? okLabel : "OK";
            list.Add((CoreEnums.DialogAction.OK, label));
        }
        if (actions.HasFlag(CoreEnums.DialogAction.Cancel))
        {
            string label = (customLabels != null && customLabels.TryGetValue("Cancel", out var cancelLabel)) ? cancelLabel : "Cancel";
            list.Add((CoreEnums.DialogAction.Cancel, label));
        }
        if (actions.HasFlag(CoreEnums.DialogAction.Yes))
        {
            string label = (customLabels != null && customLabels.TryGetValue("Yes", out var yesLabel)) ? yesLabel : "Yes";
            list.Add((CoreEnums.DialogAction.Yes, label));
        }
        if (actions.HasFlag(CoreEnums.DialogAction.No))
        {
            string label = (customLabels != null && customLabels.TryGetValue("No", out var noLabel)) ? noLabel : "No";
            list.Add((CoreEnums.DialogAction.No, label));
        }
        return list;
    }

    private async Task PositionPopupAsync(Border popup, PopupRequest request)
    {
        await Task.Delay(20);
        var pageBounds = GetPageBounds();
        var anchorBounds = GetAnchorBounds(request);
        double popupWidth = popup.Width > 0 ? popup.Width : 200;
        double popupHeight = popup.Height > 0 ? popup.Height : 150;

        double x = 0, y = 0;

        // Use alignment names that match the Core enum (adjust if yours uses "Middle" instead of "Center")
        switch (request.Alignment)
        {
            case CoreEnums.PopupAlignment.TopLeft:
                x = anchorBounds.X; y = anchorBounds.Y;
                break;
            case CoreEnums.PopupAlignment.TopCenter:
                x = anchorBounds.X + (anchorBounds.Width - popupWidth) / 2;
                y = anchorBounds.Y;
                break;
            case CoreEnums.PopupAlignment.TopRight:
                x = anchorBounds.X + anchorBounds.Width - popupWidth;
                y = anchorBounds.Y;
                break;
            case CoreEnums.PopupAlignment.LeftCenter: 
                x = anchorBounds.X;
                y = anchorBounds.Y + (anchorBounds.Height - popupHeight) / 2;
                break;
            case CoreEnums.PopupAlignment.MiddleCenter:      
                x = anchorBounds.X + (anchorBounds.Width - popupWidth) / 2;
                y = anchorBounds.Y + (anchorBounds.Height - popupHeight) / 2;
                break;
            case CoreEnums.PopupAlignment.RightCenter: 
                x = anchorBounds.X + anchorBounds.Width - popupWidth;
                y = anchorBounds.Y + (anchorBounds.Height - popupHeight) / 2;
                break;
            case CoreEnums.PopupAlignment.BottomLeft:
                x = anchorBounds.X;
                y = anchorBounds.Y + anchorBounds.Height;
                break;
            case CoreEnums.PopupAlignment.BottomCenter:
                x = anchorBounds.X + (anchorBounds.Width - popupWidth) / 2;
                y = anchorBounds.Y + anchorBounds.Height;
                break;
            case CoreEnums.PopupAlignment.BottomRight:
                x = anchorBounds.X + anchorBounds.Width - popupWidth;
                y = anchorBounds.Y + anchorBounds.Height;
                break;
            default:
                // Fallback to center
                x = anchorBounds.X + (anchorBounds.Width - popupWidth) / 2;
                y = anchorBounds.Y + (anchorBounds.Height - popupHeight) / 2;
                break;
        }

        var (offsetX, offsetY) = request.Offset;
        x += offsetX;
        y += offsetY;

        if (request.AutoFlip)
        {
            if (x + popupWidth > pageBounds.Width) x = pageBounds.Width - popupWidth - 10;
            if (x < 0) x = 10;
            if (y + popupHeight > pageBounds.Height) y = pageBounds.Height - popupHeight - 10;
            if (y < 0) y = 10;
        }

        popup.HorizontalOptions = LayoutOptions.Start;
        popup.VerticalOptions = LayoutOptions.Start;
        popup.TranslationX = x;
        popup.TranslationY = y;
    }

    private Rect GetAnchorBounds(PopupRequest request)
    {
        if (request.AnchorTarget == CoreEnums.AnchorTarget.UiElement && request.AnchorElement is VisualElement visualElement)
        {
            return GetElementBounds(visualElement);
        }
        else if (request.AnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates)
        {
            double x = request.CustomX ?? 0;
            double y = request.CustomY ?? 0;
            return new Rect(x, y, 0, 0);
        }
        else if (request.AnchorTarget == CoreEnums.AnchorTarget.MouseCursor)
        {
            var pageBounds = GetPageBounds();
            return new Rect(pageBounds.Width / 2, pageBounds.Height / 2, 0, 0);
        }
        var pageBoundsCenter = GetPageBounds();
        return new Rect(pageBoundsCenter.Width / 2, pageBoundsCenter.Height / 2, 0, 0);
    }

    private Rect GetElementBounds(VisualElement element)
    {
        double x = 0, y = 0;
        Element? current = element;
        while (current != null && !(current is ContentPage))
        {
            if (current is VisualElement ve)
            {
                x += ve.X;
                y += ve.Y;
            }
            current = current.Parent;
        }
        return new Rect(x, y, element.Width, element.Height);
    }

    private Rect GetPageBounds()
    {
        var page = GetCurrentPage();
        if (page == null) return new Rect(0, 0, 800, 600);
        return new Rect(0, 0, page.Width, page.Height);
    }

    private async Task AnimateEntryAsync(Border target, PopupRequest request)
    {
        var tasks = new List<Task>();
        if ((request.EnterAnimation & CoreEnums.AnimationType.Fade) != 0)
            tasks.Add(target.FadeToAsync(1, (uint)request.EnterDuration, GetEasing(request.EnterEasing)));
        if ((request.EnterAnimation & CoreEnums.AnimationType.Scale) != 0)
            tasks.Add(target.ScaleToAsync(1, (uint)request.EnterDuration, GetEasing(request.EnterEasing)));
        if ((request.EnterAnimation & CoreEnums.AnimationType.SlideLeft) != 0 ||
            (request.EnterAnimation & CoreEnums.AnimationType.SlideRight) != 0 ||
            (request.EnterAnimation & CoreEnums.AnimationType.SlideTop) != 0 ||
            (request.EnterAnimation & CoreEnums.AnimationType.SlideBottom) != 0)
            tasks.Add(target.TranslateToAsync(0, 0, (uint)request.EnterDuration, GetEasing(request.EnterEasing)));
        if (tasks.Count > 0) await Task.WhenAll(tasks);
    }

    private async Task AnimateExitAsync(Border? target, PopupRequest? request)
    {
        if (target == null || request == null) return;
        var tasks = new List<Task>();
        if ((request.ExitAnimation & CoreEnums.AnimationType.Fade) != 0)
            tasks.Add(target.FadeToAsync(0, (uint)request.ExitDuration, GetEasing(request.ExitEasing)));
        if ((request.ExitAnimation & CoreEnums.AnimationType.Scale) != 0)
            tasks.Add(target.ScaleToAsync(0.8, (uint)request.ExitDuration, GetEasing(request.ExitEasing)));
        if ((request.ExitAnimation & CoreEnums.AnimationType.SlideLeft) != 0)
            tasks.Add(target.TranslateToAsync(-100, 0, (uint)request.ExitDuration, GetEasing(request.ExitEasing)));
        else if ((request.ExitAnimation & CoreEnums.AnimationType.SlideRight) != 0)
            tasks.Add(target.TranslateToAsync(100, 0, (uint)request.ExitDuration, GetEasing(request.ExitEasing)));
        else if ((request.ExitAnimation & CoreEnums.AnimationType.SlideTop) != 0)
            tasks.Add(target.TranslateToAsync(0, -100, (uint)request.ExitDuration, GetEasing(request.ExitEasing)));
        else if ((request.ExitAnimation & CoreEnums.AnimationType.SlideBottom) != 0)
            tasks.Add(target.TranslateToAsync(0, 100, (uint)request.ExitDuration, GetEasing(request.ExitEasing)));
        if (tasks.Count > 0) await Task.WhenAll(tasks);
    }

    private ContentPage? GetCurrentPage()
    {
        var window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (window?.Page is Shell shell) return shell.CurrentPage as ContentPage;
        if (window?.Page is NavigationPage nav) return nav.CurrentPage as ContentPage;
        return window?.Page as ContentPage;
    }

    private Color ParseColorFromBrush(string brushString)
    {
        if (brushString.StartsWith('#') && brushString.Length == 9)
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