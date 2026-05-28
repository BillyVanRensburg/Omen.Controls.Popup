using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Omen.Controls.Popup.WPF.Controls;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ModalAnchorTargetCombo.SelectionChanged += ModalAnchorTargetCombo_SelectionChanged;

            if (TestPopup != null)
                TestPopup.Closed += (s, e) => StatusText.Text = "Popup closed at " + DateTime.Now.ToLongTimeString();

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape && TestPopup != null && TestPopup.IsOpen && TestPopup.CanCloseOnEscape)
                    _ = TestPopup.CloseAsync();
            };
        }

        private void ModalAnchorTargetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = ((ComboBoxItem)ModalAnchorTargetCombo.SelectedItem)?.Tag as string;
            bool isUiElement = tag == "UiElement";
            bool isScreenEdge = tag == "ScreenEdge";
            bool isCustom = tag == "CustomCoordinates";

            ModalAnchorElementPanel.Visibility = isUiElement ? Visibility.Visible : Visibility.Collapsed;
            ModalScreenEdgePanel.Visibility = isScreenEdge ? Visibility.Visible : Visibility.Collapsed;
            ModalCustomCoordsPanel.Visibility = isCustom ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestPopup == null) return;

                bool isModal = ModalRadio.IsChecked == true;
                TestPopup.IsModal = isModal;

                // Lightweight anchor
                TestPopup.AnchorElement = (AnchorToButtonRadio?.IsChecked == true) ? ShowButton : null;

                // Stay open
                bool stayOpenOutside = StayOpenOutsideCheckBox?.IsChecked == true;
                if (isModal)
                    TestPopup.CloseOnOverlayClick = !stayOpenOutside;
                else
                    TestPopup.StaysOpenOnOutsideClick = stayOpenOutside;

                TestPopup.CanCloseOnEscape = EscapeCheckBox?.IsChecked == true;

                // Close button
                TestPopup.ShowCloseButton = ShowCloseButtonCheckBox?.IsChecked == true;
                if (CustomCloseButtonCheckBox?.IsChecked == true)
                {
                    var template = this.TryFindResource("CustomCloseButtonTemplate") as ControlTemplate;
                    TestPopup.CloseButtonTemplate = template;
                }
                else
                {
                    TestPopup.CloseButtonTemplate = null;
                }

                // Animations
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

                // Modal positioning
                if (isModal)
                {
                    string anchorTag = ((ComboBoxItem)ModalAnchorTargetCombo.SelectedItem)?.Tag as string ?? "ParentWindowCenter";
                    TestPopup.ModalAnchorTarget = ParseAnchorTarget(anchorTag);

                    if (TestPopup.ModalAnchorTarget == CoreEnums.AnchorTarget.UiElement)
                    {
                        var selected = ModalAnchorElementCombo.SelectedItem as ComboBoxItem;
                        TestPopup.ModalAnchorElement = selected?.Tag as FrameworkElement ?? ShowButton;
                    }
                    else
                    {
                        TestPopup.ModalAnchorElement = null;
                    }

                    if (TestPopup.ModalAnchorTarget == CoreEnums.AnchorTarget.ScreenEdge)
                    {
                        string edgeTag = ((ComboBoxItem)ModalScreenEdgeCombo.SelectedItem)?.Tag as string ?? "Top";
                        TestPopup.ModalScreenEdge = ParseScreenEdge(edgeTag);
                    }

                    if (TestPopup.ModalAnchorTarget == CoreEnums.AnchorTarget.CustomCoordinates)
                    {
                        double.TryParse(ModalCustomXBox.Text, out double cx);
                        double.TryParse(ModalCustomYBox.Text, out double cy);
                        TestPopup.ModalCustomX = cx;
                        TestPopup.ModalCustomY = cy;
                    }

                    string alignTag = ((ComboBoxItem)ModalAlignmentCombo.SelectedItem)?.Tag as string ?? "Center";
                    TestPopup.ModalAlignment = ParseAlignment(alignTag);

                    int offsetX = int.TryParse(ModalOffsetXBox.Text, out int ox) ? ox : 0;
                    int offsetY = int.TryParse(ModalOffsetYBox.Text, out int oy) ? oy : 0;
                    TestPopup.ModalOffsetX = offsetX;
                    TestPopup.ModalOffsetY = offsetY;

                    TestPopup.ModalAutoFlip = ModalAutoFlipCheckBox.IsChecked == true;
                }

                // Content
                if (DialogContentRadio?.IsChecked == true)
                    TestPopup.Content = new DialogContent();
                else
                    TestPopup.Content = TextContentBox?.Text ?? string.Empty;

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
                    "SlideFromTop" => AnimationType.SlideFromTop,
                    "SlideFromBottom" => AnimationType.SlideFromBottom,
                    "SlideFromLeft" => AnimationType.SlideFromLeft,
                    "SlideFromRight" => AnimationType.SlideFromRight,
                    _ => AnimationType.None
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
                _ => EasingType.Linear
            };
        }

        private CoreEnums.AnchorTarget ParseAnchorTarget(string tag)
        {
            return tag switch
            {
                "ParentWindowCenter" => CoreEnums.AnchorTarget.ParentWindowCenter,
                "UiElement" => CoreEnums.AnchorTarget.UiElement,
                "MouseCursor" => CoreEnums.AnchorTarget.MouseCursor,
                "ScreenEdge" => CoreEnums.AnchorTarget.ScreenEdge,
                "CustomCoordinates" => CoreEnums.AnchorTarget.CustomCoordinates,
                _ => CoreEnums.AnchorTarget.ParentWindowCenter
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
                "Center" => CoreEnums.PopupAlignment.Center,
                "RightCenter" => CoreEnums.PopupAlignment.RightCenter,
                "BottomLeft" => CoreEnums.PopupAlignment.BottomLeft,
                "BottomCenter" => CoreEnums.PopupAlignment.BottomCenter,
                "BottomRight" => CoreEnums.PopupAlignment.BottomRight,
                _ => CoreEnums.PopupAlignment.Center
            };
        }

        private CoreEnums.ScreenEdge ParseScreenEdge(string tag)
        {
            return tag switch
            {
                "Top" => CoreEnums.ScreenEdge.Top,
                "Bottom" => CoreEnums.ScreenEdge.Bottom,
                "Left" => CoreEnums.ScreenEdge.Left,
                "Right" => CoreEnums.ScreenEdge.Right,
                _ => CoreEnums.ScreenEdge.Top
            };
        }
    }
}