using System.Windows;
using System.Windows.Input;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing the constructor and basic event handlers.
/// </summary>
public partial class OmenPopup
{
    /// <summary>Initializes a new instance of the <see cref="OmenPopup"/> control.</summary>
    public OmenPopup()
    {
        InitializeComponent();
        OverlayGrid.MouseLeftButtonDown += OverlayGrid_MouseLeftButtonDown;
        Loaded += OmenPopup_Loaded;
    }

    private void OmenPopup_Loaded(object? sender, RoutedEventArgs e)
    {
        EnsureTransformGroup(ModalContentBorder);
    }

    private void OverlayGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsOpen && CloseOnOutsideClick)
            _ = CloseAsync();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _ = CloseAsync();
    }
}