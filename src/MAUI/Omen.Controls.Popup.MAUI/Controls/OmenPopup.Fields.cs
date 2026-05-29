using System.Threading.Tasks;
using Omen.Controls.Popup.MAUI.Hosting;
using CoreEnums = Omen.Controls.Popup.Core.Enums;
using Microsoft.Maui.Controls;

namespace Omen.Controls.Popup.MAUI.Controls;

public class DialogClosedEventArgs : EventArgs
{
    public CoreEnums.DialogAction? Result { get; set; }
    public object? UserData { get; set; }
}

public partial class OmenPopup
{
    private MauiPopupHost? _host;               // Platform host for showing the popup
    private ContentView? _lightweightContentHost;      // For lightweight popups (floating)
    private VisualElement? _previousFocus;      // Focus tracking (optional, not fully implemented in MAUI)
    private TaskCompletionSource<CoreEnums.DialogAction>? _dialogTcs; // For ShowDialogAsync result

    // Events
    public event EventHandler? Opening;
    public event EventHandler? Opened;
    public event EventHandler? Closing;
    public event EventHandler<DialogClosedEventArgs>? DialogClosed;
}
