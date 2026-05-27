using System.Windows;

namespace Omen.Controls.Popup.Sample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            bool canClose = EscapeCheckBox.IsChecked == true;
            TestPopup.Content = $"Modal popup\n\nEscape closing is {(canClose ? "ENABLED" : "DISABLED")}";
            TestPopup.CanCloseOnEscape = canClose;
            await TestPopup.ShowAsync();
        }
    }
}