using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Omen.Controls.Popup.MAUI.Controls;

public partial class OmenPopup
{
    public async Task CloseAsync()
    {
        if (!IsOpen) return;

        Closing?.Invoke(this, EventArgs.Empty);

        if (_host != null)
            await _host.CloseAsync();

        IsOpen = false;
    }

    /// <summary>
    /// Shows the popup as a dialog and waits for the user to make a choice
    /// </summary>
    public async Task<DialogClosedEventArgs> ShowDialogAsync()
    {
        _dialogTcs = new TaskCompletionSource<Core.Enums.DialogAction>();

        var request = BuildRequest();
        if (IsModal)
        {
            await ShowModalAsync(request);
        }
        else
        {
            await ShowLightweightAsync(request);
        }

        var result = await _dialogTcs.Task;
        var args = new DialogClosedEventArgs { Result = result };
        DialogClosed?.Invoke(this, args);

        return args;
    }
}