using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Omen.Controls.Popup.WPF.Controls;  // <-- ADD THIS

namespace Omen.Controls.Popup.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TestPopup.Closed += (s, e) => StatusText.Text = "Popup closed at " + DateTime.Now.ToLongTimeString();
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape && TestPopup.IsOpen && TestPopup.CanCloseOnEscape)
                    _ = TestPopup.CloseAsync();
            };
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            // Apply close button settings
            TestPopup.ShowCloseButton = ShowCloseButtonCheckBox.IsChecked == true;
            if (CustomCloseButtonCheckBox.IsChecked == true)
                TestPopup.CloseButtonTemplate = (ControlTemplate)TestPopup.FindResource("CustomCloseButtonTemplate");
            else
                TestPopup.CloseButtonTemplate = (ControlTemplate)TestPopup.FindResource("DefaultCloseButtonTemplate");

            // Apply animation settings
            TestPopup.EnterAnimation = ParseAnimation(((ComboBoxItem)EnterAnimationCombo.SelectedItem).Tag.ToString());
            TestPopup.ExitAnimation = ParseAnimation(((ComboBoxItem)ExitAnimationCombo.SelectedItem).Tag.ToString());
            TestPopup.EnterDuration = (int)DurationSlider.Value;
            TestPopup.ExitDuration = (int)DurationSlider.Value;
            TestPopup.EnterEasing = ParseEasing(((ComboBoxItem)EasingCombo.SelectedItem).Tag.ToString());
            TestPopup.ExitEasing = ParseEasing(((ComboBoxItem)EasingCombo.SelectedItem).Tag.ToString());

            // Apply other settings
            TestPopup.CloseOnOverlayClick = OverlayClickCheckBox.IsChecked == true;
            TestPopup.CanCloseOnEscape = EscapeCheckBox.IsChecked == true;
            TestPopup.Content = $"Modal popup\n\n" +
                $"Overlay click: {TestPopup.CloseOnOverlayClick}\n" +
                $"Escape: {TestPopup.CanCloseOnEscape}\n" +
                $"Enter: {TestPopup.EnterAnimation}\n" +
                $"Exit: {TestPopup.ExitAnimation}\n" +
                $"Duration: {TestPopup.EnterDuration}ms";

            bool useAsync = AsyncCheckBox.IsChecked == true;
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
                        Debug.WriteLine($"Error: {t.Exception?.Message}");
                });
                StatusText.Text = "Popup shown (fire-and-forget). Will close when user action occurs.";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _ = TestPopup.CloseAsync();
            StatusText.Text = "Force close requested.";
        }

        private AnimationType ParseAnimation(string tag)
        {
            var parts = tag.Split(',');
            AnimationType result = AnimationType.None;
            foreach (var part in parts)
            {
                result |= part.Trim() switch
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
    }
}