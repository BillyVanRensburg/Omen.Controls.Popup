using Omen.Controls.Popup.Application.State;
using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Interfaces;
using Omen.Controls.Popup.Core.Models;

namespace Omen.Controls.Popup.Application.UseCases;

/// <summary>
/// Orchestrates showing a popup: state machine, positioning, and host interaction.
/// </summary>
public class ShowPopupUseCase
{
    private readonly IPopupHost _host;
    private readonly PopupStateMachine _stateMachine;

    public ShowPopupUseCase(IPopupHost host)
    {
        _host = host;
        _stateMachine = new PopupStateMachine();

        // Subscribe to state machine events to update host (but don't invoke host events)
        _stateMachine.Opening += async args =>
        {
            // Host can react to opening, but we don't invoke host's event here.
            // The host will have its own mechanism to raise events.
            // For now, just let state machine manage the flow.
            await Task.CompletedTask;
        };
        // Similarly for other events (optional, can be removed entirely)
    }

    public async Task ExecuteAsync(PopupRequest request, CancellationToken cancellationToken = default)
    {
        await _host.ShowAsync(request, cancellationToken);
        await _stateMachine.NotifyOpened();
    }

    public async Task CloseAsync(DialogResult result = DialogResult.None)
    {
        await _stateMachine.CloseAsync(result);
        await _host.CloseAsync();
    }
}