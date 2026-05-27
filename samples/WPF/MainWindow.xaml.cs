using System;
using System.Diagnostics;
using System.Windows;
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
                    TestPopup.CloseAsync();
            };
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            // Set properties from checkboxes
            TestPopup.CloseOnOverlayClick = OverlayClickCheckBox.IsChecked == true;
            TestPopup.CanCloseOnEscape = EscapeCheckBox.IsChecked == true;
            TestPopup.Content = $"Modal popup\n\nOverlay click: {TestPopup.CloseOnOverlayClick}\nEscape: {TestPopup.CanCloseOnEscape}";

            bool useAsync = AsyncCheckBox.IsChecked == true;
            StatusText.Text = "Showing popup... (using " + (useAsync ? "async/await" : "fire-and-forget") + ")";

            if (useAsync)
            {
                await TestPopup.ShowAsync();
                StatusText.Text = "Popup closed (after await) at " + DateTime.Now.ToLongTimeString();
            }
            else
            {
                // Fire-and-forget
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