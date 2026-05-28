using System.Windows;
using WpfPopup = System.Windows.Controls.Primitives.Popup;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing private fields used by the lightweight popup implementation.
/// </summary>
public partial class OmenPopup
{
    private WpfPopup? _lightweightPopup;
    private FrameworkElement? _lightweightContentHost;
}