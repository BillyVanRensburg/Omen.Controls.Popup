using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Omen.Controls.Popup.MAUI.Controls;

public partial class OmenPopup
{
    public async Task CloseAsync()
    {
        if (!IsOpen) return;

        var args = new ClosingCancelEventArgs();
        Closing?.Invoke(this, args);
        if (args.Cancel) return;

        if (_host != null)
            await _host.CloseAsync();

        IsOpen = false;
    }

    /// <summary>
    /// Closes the popup with a dialog result (used by dialog buttons)
    /// </summary>
    internal void CloseWithResult(Core.Enums.DialogAction result)
    {
        _dialogTcs?.TrySetResult(result);
        _ = CloseAsync();
    }
}