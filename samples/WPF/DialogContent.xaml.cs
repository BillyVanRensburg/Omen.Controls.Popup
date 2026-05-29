using System.Windows;
using System.Windows.Controls;

namespace Omen.Controls.Popup.Sample
{
    public partial class DialogContent : UserControl
    {
        /// <summary>Gets the name entered by the user.</summary>
        public string EnteredName => NameTextBox.Text;

        public DialogContent()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Find the parent OmenPopup and close it
            var popup = FindParent<Omen.Controls.Popup.WPF.Controls.OmenPopup>(this);
            popup?.CloseAsync();
        }

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && parent is not T)
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            return parent as T;
        }
    }
}