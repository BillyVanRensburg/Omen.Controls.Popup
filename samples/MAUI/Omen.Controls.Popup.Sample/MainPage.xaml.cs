using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.MAUI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Omen.Controls.Popup.Sample;

public partial class MainPage : ContentPage
{
    private DialogAction? _lastDialogResult = null;

    public MainPage()
    {
        InitializeComponent();

        OverlayBrushPicker.SelectedIndexChanged += OverlayBrushPicker_SelectionChanged;
        AnchorTargetPicker.SelectedIndexChanged += AnchorTargetPicker_SelectionChanged;
        EasingPicker.SelectedIndexChanged += EasingPicker_SelectionChanged;
        DurationSlider.ValueChanged += (s, e) => DurationLabel.Text = $"{e.NewValue:F0} ms";

        AnchorElementPicker.SelectedIndex = 0;

        TestPopup.DialogClosed += (s, args) => _lastDialogResult = args.Result;
        TestPopup.DialogClosed += OnPopupClosed;  // use the same event
    }

    private void OverlayBrushPicker_SelectionChanged(object sender, EventArgs e)
    {
        CustomOverlayPanel.IsVisible = OverlayBrushPicker.SelectedIndex == 3;
    }

    private void AnchorTargetPicker_SelectionChanged(object sender, EventArgs e)
    {
        var selected = AnchorTargetPicker.SelectedItem?.ToString();
        AnchorElementPanel.IsVisible = selected == "UI Element";
        CustomCoordsPanel.IsVisible = selected == "Custom Coordinates";
    }

    private void EasingPicker_SelectionChanged(object sender, EventArgs e)
    {
        CubicBezierPanel.IsVisible = EasingPicker.SelectedItem?.ToString() == "Cubic Bezier";
    }

    private async void ShowButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (TestPopup.IsOpen)
                await TestPopup.CloseAsync();

            await Task.Delay(50);

            bool isModal = ModalRadio.IsChecked;
            TestPopup.IsModal = isModal;

            // Anchor target
            TestPopup.AnchorTarget = ParseAnchorTarget(AnchorTargetPicker.SelectedItem?.ToString());
            if (TestPopup.AnchorTarget == AnchorTarget.UiElement)
            {
                TestPopup.AnchorElement = AnchorElementPicker.SelectedItem?.ToString() == "Left UI Element" ? LeftElement : RightElement;
            }
            else
            {
                TestPopup.AnchorElement = null;
            }

            if (TestPopup.AnchorTarget == AnchorTarget.CustomCoordinates)
            {
                double.TryParse(CustomXBox.Text, out double cx);
                double.TryParse(CustomYBox.Text, out double cy);
                TestPopup.CustomX = cx;
                TestPopup.CustomY = cy;
            }

            // Alignment
            TestPopup.Alignment = ParseAlignment(AlignmentPicker.SelectedItem?.ToString());

            // Offsets
            int offsetX = int.TryParse(OffsetXBox.Text, out int ox) ? ox : 0;
            int offsetY = int.TryParse(OffsetYBox.Text, out int oy) ? oy : 0;
            TestPopup.OffsetX = offsetX;
            TestPopup.OffsetY = offsetY;

            TestPopup.AutoFlip = AutoFlipCheckBox.IsChecked;
            TestPopup.CloseOnOutsideClick = CloseOnOutsideClickCheckBox.IsChecked;
            TestPopup.CanCloseOnEscape = EscapeCheckBox.IsChecked;
            TestPopup.ShowCloseButton = ShowCloseButtonCheckBox.IsChecked;
            TestPopup.CloseButtonTemplate = CustomCloseButtonCheckBox.IsChecked ? (ControlTemplate)Resources["CustomCloseButtonTemplate"] : null;

            // Overlay brush
            string selectedBrush = OverlayBrushPicker.SelectedItem?.ToString();
            if (selectedBrush == "Custom")
                TestPopup.OverlayBrush = new SolidColorBrush(Color.FromArgb(CustomOverlayBrushBox.Text));
            else
            {
                var brushColor = selectedBrush switch
                {
                    "Black 50%" => "#80000000",
                    "Black 70%" => "#B3000000",
                    "White 50%" => "#80FFFFFF",
                    _ => "#80000000"
                };
                TestPopup.OverlayBrush = new SolidColorBrush(Color.FromArgb(brushColor));
            }

            // Animations
            TestPopup.EnterAnimation = ParseAnimation(EnterAnimationPicker.SelectedItem?.ToString());
            TestPopup.ExitAnimation = ParseAnimation(ExitAnimationPicker.SelectedItem?.ToString());
            int duration = (int)DurationSlider.Value;
            TestPopup.EnterDuration = duration;
            TestPopup.ExitDuration = duration;

            var easingText = EasingPicker.SelectedItem?.ToString();
            TestPopup.EnterEasing = ParseEasing(easingText);
            TestPopup.ExitEasing = ParseEasing(easingText);

            if (easingText == "Cubic Bezier")
            {
                TestPopup.EnterCubicBezierPoints = CubicBezierPointsBox.Text;
                TestPopup.ExitCubicBezierPoints = CubicBezierPointsBox.Text;
            }
            else
            {
                TestPopup.EnterCubicBezierPoints = null;
                TestPopup.ExitCubicBezierPoints = null;
            }

            // Dialog buttons (only modal)
            if (isModal)
            {
                DialogAction buttons = DialogAction.None;
                if (ChkOK.IsChecked) buttons |= DialogAction.OK;
                if (ChkCancel.IsChecked) buttons |= DialogAction.Cancel;
                if (ChkYes.IsChecked) buttons |= DialogAction.Yes;
                if (ChkNo.IsChecked) buttons |= DialogAction.No;
                TestPopup.DialogButtons = buttons;

                var labels = new Dictionary<string, string>();
                if (ChkOK.IsChecked) labels["OK"] = TxtOK.Text;
                if (ChkCancel.IsChecked) labels["Cancel"] = TxtCancel.Text;
                if (ChkYes.IsChecked) labels["Yes"] = TxtYes.Text;
                if (ChkNo.IsChecked) labels["No"] = TxtNo.Text;
                TestPopup.CustomButtonLabels = labels;
            }
            else
            {
                TestPopup.DialogButtons = DialogAction.None;
                TestPopup.CustomButtonLabels = null;
            }

            // Content
            TestPopup.Content = CustomContentRadio.IsChecked ? new DialogContent() : TextContentBox.Text;

            bool useAsync = AsyncCheckBox.IsChecked;
            StatusText.Text = $"Showing popup... (using {(useAsync ? "async/await" : "fire-and-forget")})";

            if (useAsync)
            {
                if (isModal)
                {
                    var resultArgs = await TestPopup.ShowDialogAsync();
                    DialogAction result = resultArgs.Result ?? DialogAction.None;
                    StatusText.Text = $"Popup closed (after await) with result: {result} at {DateTime.Now:HH:mm:ss}";
                }
                else
                {
                    await TestPopup.ShowAsync();
                    StatusText.Text = $"Popup closed (after await) at {DateTime.Now:HH:mm:ss}";
                }
            }
            else
            {
                _ = TestPopup.ShowAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        var msg = t.Exception?.Flatten().InnerException?.Message ?? "Unknown error";
                        Debug.WriteLine($"Error: {msg}");
                        Dispatcher.Dispatch(() => StatusText.Text = "Popup show failed (fire-and-forget).");
                    }
                });
                StatusText.Text = "Popup shown (fire-and-forget). No result captured.";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ShowButton_Clicked failed: {ex}");
            await DisplayAlert("Error", $"Error preparing popup: {ex.Message}", "OK");
        }
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await TestPopup.CloseAsync();
        StatusText.Text = "Force close requested.";
    }

    private void OnPopupClosed(object? sender, DialogClosedEventArgs args)
    {
        StatusText.Text = "Popup closed at " + DateTime.Now.ToLongTimeString();

        string message = null;
        if (TestPopup.Content is DialogContent dialog)
        {
            string name = dialog.EnteredName;
            if (!string.IsNullOrWhiteSpace(name))
                message = $"Name entered: {name}";
        }
        else if (_lastDialogResult.HasValue)
        {
            message = $"You clicked: {_lastDialogResult.Value}";
            _lastDialogResult = null;
        }

        if (!string.IsNullOrEmpty(message))
        {
            Dispatcher.Dispatch(async () =>
                await DisplayAlert("Popup Result", message, "OK")
            );
        }
    }

    // Helper parsing methods
    private AnchorTarget ParseAnchorTarget(string text) =>
        text switch
        {
            "Parent Container" => AnchorTarget.ParentContainer,
            "UI Element" => AnchorTarget.UiElement,
            "Mouse Cursor" => AnchorTarget.MouseCursor,
            "Custom Coordinates" => AnchorTarget.CustomCoordinates,
            _ => AnchorTarget.ParentContainer
        };

    private PopupAlignment ParseAlignment(string text) =>
        text switch
        {
            "Top Left" => PopupAlignment.TopLeft,
            "Top Center" => PopupAlignment.TopCenter,
            "Top Right" => PopupAlignment.TopRight,
            "Left Center" => PopupAlignment.LeftCenter,
            "Middle Center" => PopupAlignment.MiddleCenter,
            "Right Center" => PopupAlignment.RightCenter,
            "Bottom Left" => PopupAlignment.BottomLeft,
            "Bottom Center" => PopupAlignment.BottomCenter,
            "Bottom Right" => PopupAlignment.BottomRight,
            _ => PopupAlignment.MiddleCenter
        };

    private AnimationType ParseAnimation(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return AnimationType.None;
        var parts = text.Split('+');
        AnimationType result = AnimationType.None;
        foreach (var part in parts)
        {
            result |= part.Trim() switch
            {
                "Fade" => AnimationType.Fade,
                "Scale" => AnimationType.Scale,
                "Slide Top" => AnimationType.SlideTop,
                "Slide Bottom" => AnimationType.SlideBottom,
                "Slide Left" => AnimationType.SlideLeft,
                "Slide Right" => AnimationType.SlideRight,
                _ => AnimationType.None
            };
        }
        return result;
    }

    private EasingType ParseEasing(string text) =>
        text switch
        {
            "Linear" => EasingType.Linear,
            "Ease In" => EasingType.EaseIn,
            "Ease Out" => EasingType.EaseOut,
            "Ease In Out" => EasingType.EaseInOut,
            "Cubic Bezier" => EasingType.CubicBezier,
            _ => EasingType.Linear
        };
}