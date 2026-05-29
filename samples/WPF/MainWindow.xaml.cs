using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.WPF.Controls;
using System;
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
        public MainWindow()
        {
            InitializeComponent();

            AnchorTargetCombo.SelectionChanged += AnchorTargetCombo_SelectionChanged;
            OverlayBrushCombo.SelectionChanged += OverlayBrushCombo_SelectionChanged;

            if (AnchorElementCombo.Items.Count > 0)
                AnchorElementCombo.SelectedIndex = 0;

            if (TestPopup != null)
                TestPopup.Closed += (s, e) => StatusText.Text = "Popup closed at " + DateTime.Now.ToLongTimeString();

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape && TestPopup != null && TestPopup.IsOpen && TestPopup.CanCloseOnEscape)
                    _ = TestPopup.CloseAsync();
            };
        }

        private void OverlayBrushCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)OverlayBrushCombo.SelectedItem)?.Tag as string;
            CustomOverlayPanel.Visibility = (tag == "Custom") ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AnchorTargetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)AnchorTargetCombo.SelectedItem)?.Tag as string;
            bool isUiElement = tag == "UiElement";
            bool isCustom = tag == "CustomCoordinates";

            AnchorElementPanel.Visibility = isUiElement ? Visibility.Visible : Visibility.Collapsed;
            CustomCoordsPanel.Visibility = isCustom ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestPopup == null) return;

                // Ensure any previously open popup is closed before we reconfigure
                if (TestPopup.IsOpen)
                    await TestPopup.CloseAsync();

                // Small delay to allow clean-up (helps with layout)
                await Task.Delay(50);

                bool isModal = ModalRadio.IsChecked == true;
                TestPopup.IsModal = isModal;

                // Positioning properties (common)
                string anchorTag = ((ComboBoxItem)AnchorTargetCombo.SelectedItem)?.Tag as string ?? "ParentContainer";
                TestPopup.AnchorTarget = ParseAnchorTarget(anchorTag);

                if (TestPopup.AnchorTarget == CoreEnums.AnchorTarget.UiElement)
                {
                    var selected = AnchorElementCombo.SelectedItem as ComboBoxItem;
                    if (selected?.Tag is FrameworkElement element)
                        TestPopup.AnchorElement = element;
                    else
                        TestPopup.AnchorElement = LeftElement;
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
                if (CustomCloseButtonCheckBox.IsChecked == true)
                {
                    var template = this.TryFindResource("CustomCloseButtonTemplate") as ControlTemplate;
                    TestPopup.CloseButtonTemplate = template;
                }
                else
                {
                    TestPopup.CloseButtonTemplate = null;
                }

                var selectedOverlay = ((ComboBoxItem)OverlayBrushCombo.SelectedItem)?.Tag as string;
                if (selectedOverlay == "Custom")
                    TestPopup.OverlayBrush = (Brush)new BrushConverter().ConvertFromString(CustomOverlayBrushBox.Text);
                else
                    TestPopup.OverlayBrush = (Brush)new BrushConverter().ConvertFromString(selectedOverlay ?? "#80000000");

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

                // Content – always create a fresh instance for dialog
                if (DialogContentRadio.IsChecked == true)
                {
                    // Clear old content and force layout to avoid reuse issues
                    TestPopup.Content = null;
                    await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Loaded);
                    TestPopup.Content = new DialogContent();
                }
                else
                {
                    TestPopup.Content = TextContentBox?.Text ?? string.Empty;
                }

                bool useAsync = AsyncCheckBox?.IsChecked == true;
                StatusText.Text = "Showing popup... (using " + (useAsync ? "async/await" : "fire-and-forget") + ")";

                if (useAsync)
                {
                    await TestPopup.ShowAsync();
                    StatusText.Text = "Popup closed (after await) at " + DateTime.Now.ToLongTimeString();
                }
                else
                {
                    _ = TestPopup.ShowAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            var msg = t.Exception?.Flatten().InnerException?.Message ?? t.Exception?.Message ?? "Unknown error";
                            Debug.WriteLine($"Error (fire-and-forget): {msg}");
                            Dispatcher.Invoke(() => StatusText.Text = "Popup show failed (fire-and-forget).");
                        }
                    });
                    StatusText.Text = "Popup shown (fire-and-forget). Will close when user action occurs.";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ShowButton_Click failed: " + ex);
                MessageBox.Show("Error preparing popup: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (TestPopup == null) return;
            _ = TestPopup.CloseAsync();
            StatusText.Text = "Force close requested.";
        }

        private AnimationType ParseAnimation(string tag)
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

        private EasingType ParseEasing(string tag)
        {
            return tag switch
            {
                "Linear" => EasingType.Linear,
                "EaseIn" => EasingType.EaseIn,
                "EaseOut" => EasingType.EaseOut,
                "EaseInOut" => EasingType.EaseInOut,
                _ => EasingType.Linear,
            };
        }

        private CoreEnums.AnchorTarget ParseAnchorTarget(string tag)
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

        private CoreEnums.PopupAlignment ParseAlignment(string tag)
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