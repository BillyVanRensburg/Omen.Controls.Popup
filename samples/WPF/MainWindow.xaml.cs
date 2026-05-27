using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            // Apply other settings
            TestPopup.CloseOnOverlayClick = OverlayClickCheckBox.IsChecked == true;
            TestPopup.CanCloseOnEscape = EscapeCheckBox.IsChecked == true;
            TestPopup.Content = $"Modal popup\n\nOverlay click: {TestPopup.CloseOnOverlayClick}\nEscape: {TestPopup.CanCloseOnEscape}\nClose button: {(TestPopup.ShowCloseButton ? "visible" : "hidden")}";

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
    }
}