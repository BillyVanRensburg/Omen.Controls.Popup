using Omen.Controls.Popup.Application.Enums;
using Omen.Controls.Popup.Core.Enums;
using Omen.Controls.Popup.Core.Events;
using Omen.Controls.Popup.Core.Models;

namespace Omen.Controls.Popup.Application.State;

/// <summary>
/// Manages the lifecycle of a popup (closed → opening → open → closing → closed).
/// Partial class; events and async methods are in separate files.
/// </summary>
public partial class PopupStateMachine
{
    private PopupRequest? _currentRequest;

    /// <summary>Current state of the popup.</summary>
    public PopupState CurrentState { get; private set; } = PopupState.Closed;

    /// <summary>The active request (configuration) when open or opening.</summary>
    public PopupRequest? CurrentRequest => _currentRequest;

    /// <summary>Whether the popup is modal.</summary>
    public bool IsModal => _currentRequest?.IsModal ?? false;

    /// <summary>
    /// Attempts to transition to a new state.
    /// </summary>
    /// <returns>True if transition succeeded, false if invalid.</returns>
    private bool TryTransition(PopupState newState)
    {
        // Define allowed transitions
        bool isValid = (CurrentState, newState) switch
        {
            (PopupState.Closed, PopupState.Opening) => true,
            (PopupState.Opening, PopupState.Open) => true,
            (PopupState.Open, PopupState.Closing) => true,
            (PopupState.Closing, PopupState.Closed) => true,
            _ => false
        };

        if (!isValid) return false;

        CurrentState = newState;
        return true;
    }

    /// <summary>
    /// Initiates the opening sequence. Does not perform UI work; only state and events.
    /// </summary>
    /// <param name="request">The popup configuration.</param>
    /// <returns>True if opening can proceed, false if cancelled or invalid state.</returns>
    public async Task<bool> StartOpeningAsync(PopupRequest request)
    {
        if (!TryTransition(PopupState.Opening))
            return false;

        _currentRequest = request;

        var args = new CancellableEventArgs();
        await OnOpeningAsync(args);
        if (args.Cancel)
        {
            // Revert state
            CurrentState = PopupState.Closed;
            _currentRequest = null;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Completes the opening (called after UI has shown the popup).
    /// </summary>
    public async Task CompleteOpeningAsync()
    {
        if (CurrentState != PopupState.Opening)
            return;

        if (!TryTransition(PopupState.Open))
            return;

        await OnOpenedAsync();
    }

    /// <summary>
    /// Initiates the closing sequence.
    /// </summary>
    /// <returns>True if closing can proceed, false if cancelled or invalid state.</returns>
    public async Task<bool> StartClosingAsync()
    {
        if (!TryTransition(PopupState.Closing))
            return false;

        var args = new CancellableEventArgs();
        await OnClosingAsync(args);
        if (args.Cancel)
        {
            // Revert to open
            CurrentState = PopupState.Open;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Completes the closing (called after UI has closed the popup).
    /// </summary>
    public async Task CompleteClosingAsync()
    {
        if (CurrentState != PopupState.Closing)
            return;

        if (!TryTransition(PopupState.Closed))
            return;

        _currentRequest = null;
        await OnClosedAsync();
    }
}