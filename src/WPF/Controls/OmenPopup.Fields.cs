using System.Windows;
using System.Windows.Input;
using WpfPopup = System.Windows.Controls.Primitives.Popup;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing private fields used by the OmenPopup control.
/// </summary>
public partial class OmenPopup
{
    private WpfPopup? _lightweightPopup;           // The lightweight popup control
    private FrameworkElement? _lightweightContentHost; // Visual root for lightweight popup
    private IInputElement? _previousFocus;         // Stores the element that had focus before the popup opened
}