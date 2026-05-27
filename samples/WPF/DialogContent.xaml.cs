using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Omen.Controls.Popup.Sample;

public partial class DialogContent : UserControl
{
    public DialogContent()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        // Find the parent OmenPopup and close it
        var popup = FindParent<Omen.Controls.Popup.WPF.Controls.OmenPopup>(this);
        popup?.CloseAsync();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        var popup = FindParent<Omen.Controls.Popup.WPF.Controls.OmenPopup>(this);
        popup?.CloseAsync();
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null && parent is not T)
            parent = VisualTreeHelper.GetParent(parent);
        return parent as T;
    }
}