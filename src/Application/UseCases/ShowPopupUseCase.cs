using Omen.Controls.Popup.Application.State;
using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Interfaces;
using Omen.Controls.Popup.Core.Models;

namespace Omen.Controls.Popup.Application.UseCases;

/// <summary>
/// Orchestrates showing a popup: state machine, positioning, and host interaction.
/// This use case acts as a mediator between the UI (or a higher‑level controller) and the popup infrastructure.
/// </summary>
/// <param name="host">The platform‑specific popup host (e.g., WPF, MAUI, Blazor).</param>
public class ShowPopupUseCase(IPopupHost host)
{
    private readonly IPopupHost _host = host;
    private readonly PopupStateMachine _stateMachine = new();

    /// <summary>
    /// Executes the popup show operation asynchronously.
    /// </summary>
    /// <param name="request">The popup configuration and content.</param>
    /// <param name="cancellationToken">Token to cancel the operation (if supported by the host).</param>
    public async Task ExecuteAsync(PopupRequest request, CancellationToken cancellationToken = default)
    {
        await _host.ShowAsync(request, cancellationToken);
        await _stateMachine.NotifyOpened();
    }

    /// <summary>
    /// Closes the popup asynchronously, optionally passing a result for modal dialogs.
    /// </summary>
    /// <param name="result">The result of the dialog (e.g., OK, Cancel). Default is <see cref="DialogAction.None"/>.</param>
    public async Task CloseAsync(DialogAction result = DialogAction.None)
    {
        await _stateMachine.CloseAsync(result);
        await _host.CloseAsync();
    }
}