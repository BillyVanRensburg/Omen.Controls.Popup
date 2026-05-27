using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Models;

namespace Omen.Controls.Popup.Application.State;

public partial class PopupStateMachine
{
    private TaskCompletionSource<bool>? _showTcs;
    private TaskCompletionSource<DialogResult>? _dialogTcs;

    /// <summary>
    /// Shows the popup as a non‑modal lightweight popup.
    /// Returns a task that completes when the popup is closed.
    /// </summary>
    public async Task ShowAsync(PopupRequest request)
    {
        if (CurrentState != PopupState.Closed)
            return;

        _showTcs = new TaskCompletionSource<bool>();

        if (!await StartOpeningAsync(request))
            return;

        // After this, the UI will call CompleteOpeningAsync() when visible.
        // Then we wait for close.
        await _showTcs.Task;
    }

    /// <summary>
    /// Shows the popup as a modal dialog, returning a result.
    /// </summary>
    public async Task<DialogResult> ShowDialogAsync(PopupRequest request)
    {
        if (CurrentState != PopupState.Closed)
            return DialogResult.None;

        request.IsModal = true;
        _dialogTcs = new TaskCompletionSource<DialogResult>();

        if (!await StartOpeningAsync(request))
            return DialogResult.None;

        return await _dialogTcs.Task;
    }

    /// <summary>
    /// Closes the popup (can be called from UI or externally).
    /// </summary>
    /// <param name="result">Result for modal dialogs; ignored for non‑modal.</param>
    public async Task CloseAsync(DialogResult result = DialogResult.None)
    {
        if (CurrentState != PopupState.Open)
            return;

        if (!await StartClosingAsync())
            return;

        // UI will call CompleteClosingAsync() after animation.

        if (_dialogTcs != null && !_dialogTcs.Task.IsCompleted)
            _dialogTcs.SetResult(result);
        else if (_showTcs != null && !_showTcs.Task.IsCompleted)
            _showTcs.SetResult(true);
    }

    /// <summary>
    /// Called by the UI when the popup is fully open.
    /// </summary>
    public async Task NotifyOpened()
    {
        await CompleteOpeningAsync();
    }

    /// <summary>
    /// Called by the UI when the popup is fully closed.
    /// </summary>
    public async Task NotifyClosed()
    {
        await CompleteClosingAsync();
    }
}