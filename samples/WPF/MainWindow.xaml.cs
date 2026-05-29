using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Sample
{
    public partial class MainWindow : Window
    {
        private DialogAction? _lastDialogResult = null;

        public MainWindow()
        {
            InitializeComponent();

            AnchorTargetCombo.SelectionChanged += AnchorTargetCombo_SelectionChanged;
            OverlayBrushCombo.SelectionChanged += OverlayBrushCombo_SelectionChanged;
            EasingCombo.SelectionChanged += EasingCombo_SelectionChanged;

            if (AnchorElementCombo.Items.Count > 0)
                AnchorElementCombo.SelectedIndex = 0;

            if (TestPopup != null)
            {
                // Store the result when a built‑in dialog button is clicked
                TestPopup.DialogClosed += (s, result) =>
                {
                    _lastDialogResult = result;
                };

                // Unified closed handler
                TestPopup.Closed += (s, e) =>
                {
                    StatusText.Text = "Popup closed at " + DateTime.Now.ToLongTimeString();

                    string message = null;

                    // Check if the custom control (DialogContent) was used
                    if (TestPopup.Content is DialogContent dialog)
                    {
                        string name = dialog.EnteredName;
                        if (!string.IsNullOrWhiteSpace(name))
                            message = $"Name entered: {name}";
                    }
                    // Otherwise, check if a built‑in button result was stored
                    else if (_lastDialogResult.HasValue)
                    {
                        message = $"You clicked: {_lastDialogResult.Value}";
                        _lastDialogResult = null; // reset
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        Dispatcher.Invoke(() =>
                            MessageBox.Show(message, "Popup Result", MessageBoxButton.OK, MessageBoxImage.Information)
                        );
                    }
                };
            }

            PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape && TestPopup != null && TestPopup.IsOpen && TestPopup.CanCloseOnEscape)
                    _ = TestPopup.CloseAsync();
            };
        }

        private void OverlayBrushCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)OverlayBrushCombo.SelectedItem)?.Tag as string;
            CustomOverlayPanel.Visibility = tag == "Custom" ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AnchorTargetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)AnchorTargetCombo.SelectedItem)?.Tag as string;
            bool isUiElement = tag == "UiElement";
            bool isCustom = tag == "CustomCoordinates";

            AnchorElementPanel.Visibility = isUiElement ? Visibility.Visible : Visibility.Collapsed;
            CustomCoordsPanel.Visibility = isCustom ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EasingCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EasingCombo.SelectedItem is ComboBoxItem item)
            {
                var tag = item.Tag as string;
                if (CubicBezierPanel != null)
                    CubicBezierPanel.Visibility = tag == "CubicBezier" ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestPopup == null) return;

                if (TestPopup.IsOpen)
                    await TestPopup.CloseAsync();

                await Task.Delay(50);

                bool isModal = ModalRadio.IsChecked == true;
                TestPopup.IsModal = isModal;

                string anchorTag = ((ComboBoxItem)AnchorTargetCombo.SelectedItem)?.Tag as string ?? "ParentContainer";
                TestPopup.AnchorTarget = ParseAnchorTarget(anchorTag);

                if (TestPopup.AnchorTarget == CoreEnums.AnchorTarget.UiElement)
                {
                    var selected = AnchorElementCombo.SelectedItem as ComboBoxItem;
                    TestPopup.AnchorElement = selected?.Tag as FrameworkElement ?? LeftElement;
                }
                else
                {
                    TestPopup.AnchorElement = null;
                }

                if (TestPopup.AnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates)
                {
                    double.TryParse(CustomXBox.Text, out double cx);
                    double.TryParse(CustomYBox.Text, out double cy);
                    TestPopup.CustomX = cx;
                    TestPopup.CustomY = cy;
                }

                string alignTag = ((ComboBoxItem)AlignmentCombo.SelectedItem)?.Tag as string ?? "MiddleCenter";
                TestPopup.Alignment = ParseAlignment(alignTag);

                int offsetX = int.TryParse(OffsetXBox.Text, out int ox) ? ox : 0;
                int offsetY = int.TryParse(OffsetYBox.Text, out int oy) ? oy : 0;
                TestPopup.OffsetX = offsetX;
                TestPopup.OffsetY = offsetY;

                TestPopup.AutoFlip = AutoFlipCheckBox.IsChecked == true;

                TestPopup.CloseOnOutsideClick = CloseOnOutsideClickCheckBox.IsChecked == true;
                TestPopup.CanCloseOnEscape = EscapeCheckBox.IsChecked == true;

                TestPopup.ShowCloseButton = ShowCloseButtonCheckBox.IsChecked == true;
                TestPopup.CloseButtonTemplate = CustomCloseButtonCheckBox.IsChecked == true
                    ? this.TryFindResource("CustomCloseButtonTemplate") as ControlTemplate
                    : null;

                var selectedOverlay = ((ComboBoxItem)OverlayBrushCombo.SelectedItem)?.Tag as string;
                TestPopup.OverlayBrush = selectedOverlay == "Custom"
                    ? (Brush)new BrushConverter().ConvertFromString(CustomOverlayBrushBox.Text)
                    : (Brush)new BrushConverter().ConvertFromString(selectedOverlay ?? "#80000000");

                string enterTag = (EnterAnimationCombo?.SelectedItem as ComboBoxItem)?.Tag as string ?? "None";
                string exitTag = (ExitAnimationCombo?.SelectedItem as ComboBoxItem)?.Tag as string ?? "None";
                TestPopup.EnterAnimation = ParseAnimation(enterTag);
                TestPopup.ExitAnimation = ParseAnimation(exitTag);

                int duration = (int)Math.Round(DurationSlider?.Value ?? 200);
                TestPopup.EnterDuration = duration;
                TestPopup.ExitDuration = duration;

                string easingTag = (EasingCombo?.SelectedItem as ComboBoxItem)?.Tag as string ?? "Linear";
                TestPopup.EnterEasing = ParseEasing(easingTag);
                TestPopup.ExitEasing = ParseEasing(easingTag);

                if (easingTag == "CubicBezier")
                {
                    TestPopup.EnterCubicBezierPoints = CubicBezierPointsBox.Text;
                    TestPopup.ExitCubicBezierPoints = CubicBezierPointsBox.Text;
                }
                else
                {
                    TestPopup.EnterCubicBezierPoints = null;
                    TestPopup.ExitCubicBezierPoints = null;
                }

                // Dialog buttons (only for modal)
                if (isModal)
                {
                    DialogAction buttons = DialogAction.None;
                    if (ChkOK.IsChecked == true) buttons |= DialogAction.OK;
                    if (ChkCancel.IsChecked == true) buttons |= DialogAction.Cancel;
                    if (ChkYes.IsChecked == true) buttons |= DialogAction.Yes;
                    if (ChkNo.IsChecked == true) buttons |= DialogAction.No;
                    TestPopup.DialogButtons = buttons;

                    var labels = new Dictionary<string, string>();
                    if (ChkOK.IsChecked == true) labels["OK"] = TxtOK.Text;
                    if (ChkCancel.IsChecked == true) labels["Cancel"] = TxtCancel.Text;
                    if (ChkYes.IsChecked == true) labels["Yes"] = TxtYes.Text;
                    if (ChkNo.IsChecked == true) labels["No"] = TxtNo.Text;
                    TestPopup.CustomButtonLabels = labels;
                }
                else
                {
                    TestPopup.DialogButtons = DialogAction.None;
                    TestPopup.CustomButtonLabels = null;
                }

                // Content
                TestPopup.Content = CustomContentRadio.IsChecked == true
                    ? new DialogContent()
                    : (TextContentBox?.Text ?? string.Empty);

                bool useAsync = AsyncCheckBox?.IsChecked == true;
                StatusText.Text = $"Showing popup... (using {(useAsync ? "async/await" : "fire-and-forget")})";

                if (useAsync)
                {
                    if (isModal)
                    {
                        // Use ShowDialogAsync to get the button result
                        DialogAction result = await TestPopup.ShowDialogAsync();
                        StatusText.Text = $"Popup closed (after await) with result: {result} at {DateTime.Now.ToLongTimeString()}";
                    }
                    else
                    {
                        // Lightweight: use ShowAsync (no result)
                        await TestPopup.ShowAsync();
                        StatusText.Text = $"Popup closed (after await) at {DateTime.Now.ToLongTimeString()}";
                    }
                }
                else
                {
                    // Fire-and-forget: no result captured
                    _ = TestPopup.ShowAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            var msg = t.Exception?.Flatten().InnerException?.Message ?? t.Exception?.Message ?? "Unknown error";
                            Debug.WriteLine($"Error (fire-and-forget): {msg}");
                            Dispatcher.Invoke(() => StatusText.Text = "Popup show failed (fire-and-forget).");
                        }
                    });
                    StatusText.Text = "Popup shown (fire-and-forget). Will close when user action occurs. No result captured.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ShowButton_Click failed: {ex}");
                MessageBox.Show($"Error preparing popup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (TestPopup == null) return;
            _ = TestPopup.CloseAsync();
            StatusText.Text = "Force close requested.";
        }

        private static AnimationType ParseAnimation(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return AnimationType.None;
            var parts = tag.Split(',');
            AnimationType result = AnimationType.None;
            foreach (var raw in parts)
            {
                var part = raw.Trim();
                result |= part switch
                {
                    "Fade" => AnimationType.Fade,
                    "Scale" => AnimationType.Scale,
                    "SlideTop" => AnimationType.SlideTop,
                    "SlideBottom" => AnimationType.SlideBottom,
                    "SlideLeft" => AnimationType.SlideLeft,
                    "SlideRight" => AnimationType.SlideRight,
                    _ => AnimationType.None,
                };
            }
            return result;
        }

        private static EasingType ParseEasing(string tag)
        {
            return tag switch
            {
                "Linear" => EasingType.Linear,
                "EaseIn" => EasingType.EaseIn,
                "EaseOut" => EasingType.EaseOut,
                "EaseInOut" => EasingType.EaseInOut,
                "CubicBezier" => EasingType.CubicBezier,
                _ => EasingType.Linear,
            };
        }

        private static CoreEnums.AnchorTarget ParseAnchorTarget(string tag)
        {
            return tag switch
            {
                "ParentContainer" => CoreEnums.AnchorTarget.ParentContainer,
                "UiElement" => CoreEnums.AnchorTarget.UiElement,
                "MouseCursor" => CoreEnums.AnchorTarget.MouseCursor,
                "CustomCoordinates" => CoreEnums.AnchorTarget.CustomCoordinates,
                _ => CoreEnums.AnchorTarget.ParentContainer,
            };
        }

        private static CoreEnums.PopupAlignment ParseAlignment(string tag)
        {
            return tag switch
            {
                "TopLeft" => CoreEnums.PopupAlignment.TopLeft,
                "TopCenter" => CoreEnums.PopupAlignment.TopCenter,
                "TopRight" => CoreEnums.PopupAlignment.TopRight,
                "LeftCenter" => CoreEnums.PopupAlignment.LeftCenter,
                "MiddleCenter" => CoreEnums.PopupAlignment.MiddleCenter,
                "RightCenter" => CoreEnums.PopupAlignment.RightCenter,
                "BottomLeft" => CoreEnums.PopupAlignment.BottomLeft,
                "BottomCenter" => CoreEnums.PopupAlignment.BottomCenter,
                "BottomRight" => CoreEnums.PopupAlignment.BottomRight,
                _ => CoreEnums.PopupAlignment.MiddleCenter,
            };
        }
    }
}