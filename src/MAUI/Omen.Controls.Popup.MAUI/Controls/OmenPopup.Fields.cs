using System;
using System.Threading.Tasks;
using Omen.Controls.Popup.MAUI.Hosting;
using CoreEnums = Omen.Controls.Popup.Core.Enums;
using Microsoft.Maui.Controls;

namespace Omen.Controls.Popup.MAUI.Controls;

// Event args for Opening event (supports cancellation)
public class OpeningCancelEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}

// Event args for Closing event (supports cancellation)
public class ClosingCancelEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}

// Event args for DialogClosed event
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

    // Events – updated to support cancellation where appropriate
    public event EventHandler<OpeningCancelEventArgs>? Opening;
    public event EventHandler? Opened;
    public event EventHandler<ClosingCancelEventArgs>? Closing;
    public event EventHandler<DialogClosedEventArgs>? DialogClosed;
}