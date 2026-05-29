using Microsoft.Maui.Controls;

namespace Omen.Controls.Popup.Sample;

public partial class DialogContent : ContentView
{
    public string EnteredName => NameEntry.Text;

    public DialogContent()
    {
        InitializeComponent();
    }

    private async void SubmitButton_Clicked(object sender, System.EventArgs e)
    {
        // Find the parent OmenPopup and close it
        var popup = this.FindParent<Omen.Controls.Popup.MAUI.Controls.OmenPopup>();
        if (popup != null)
            await popup.CloseAsync();
    }
}

// Helper extension to find parent of a specific type
public static class VisualTreeHelper
{
    public static T? FindParent<T>(this Element element) where T : Element
    {
        var parent = element.Parent;
        while (parent != null)
        {
            if (parent is T t)
                return t;
            parent = parent.Parent;
        }
        return null;
    }
}