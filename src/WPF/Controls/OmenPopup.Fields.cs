using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CoreEnums = Omen.Controls.Popup.Core.Enums;
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
    private TaskCompletionSource<CoreEnums.DialogAction>? _dialogTcs; // Used to return result from ShowDialogAsync
}