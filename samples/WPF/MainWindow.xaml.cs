using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Omen.Controls.Popup.WPF.Controls;

namespace Omen.Controls.Popup.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (TestPopup != null)
                TestPopup.Closed += (s, e) => StatusText.Text = "Popup closed at " + DateTime.Now.ToLongTimeString();

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape && TestPopup != null && TestPopup.IsOpen && TestPopup.CanCloseOnEscape)
                    _ = TestPopup.CloseAsync();
            };
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TestPopup == null)
                {
                    StatusText.Text = "TestPopup reference is null.";
                    return;
                }

                // Mode
                bool isModal = ModalRadio.IsChecked == true;
                TestPopup.IsModal = isModal;

                // Lightweight anchor
                TestPopup.AnchorElement = (AnchorToButtonRadio?.IsChecked == true) ? ShowButton : null;

                // Unified "Stay open on outside click"
                bool stayOpenOutside = StayOpenOutsideCheckBox?.IsChecked == true;
                if (isModal)
                {
                    // Modal: "stay open" means overlay click does NOT close; otherwise it does close.
                    TestPopup.CloseOnOverlayClick = !stayOpenOutside;
                    // For modal, StaysOpenOnOutsideClick is not used (only lightweight uses it).
                }
                else
                {
                    // Lightweight: "stay open" directly controls the popup's StaysOpen property.
                    TestPopup.StaysOpenOnOutsideClick = stayOpenOutside;
                }

                // Escape key
                TestPopup.CanCloseOnEscape = EscapeCheckBox?.IsChecked == true;

                // Close button settings
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

                // Animation settings
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

                // Content
                if (DialogContentRadio?.IsChecked == true)
                    TestPopup.Content = new DialogContent();
                else
                    TestPopup.Content = TextContentBox?.Text ?? string.Empty;

                // Async mode
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
            var parts = tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
            if (string.IsNullOrWhiteSpace(tag)) return EasingType.Linear;
            return tag switch
            {
                "Linear" => EasingType.Linear,
                "EaseIn" => EasingType.EaseIn,
                "EaseOut" => EasingType.EaseOut,
                "EaseInOut" => EasingType.EaseInOut,
                _ => EasingType.Linear
            };
        }
    }
}