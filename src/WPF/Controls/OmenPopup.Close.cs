using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing the CloseAsync method and keyboard event handling.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Occurs when the popup is closed.
    /// </summary>
    public event EventHandler? Closed;

    /// <summary>
    /// Closes the popup asynchronously. For modal popups, restores focus to the previously focused element.
    /// For lightweight popups, also detaches the focus trapping handler before restoring focus.
    /// </summary>
    public async Task CloseAsync()
    {
        if (!IsOpen) return;

        if (IsModal)
        {
            await AnimateExitAsync(ModalContentBorder, ExitAnimation, ExitDuration, ExitEasing);
            ResetTransforms(ModalContentBorder);
            ModalContentBorder.Margin = new Thickness(0);
            ModalContentBorder.HorizontalAlignment = HorizontalAlignment.Center;
            ModalContentBorder.VerticalAlignment = VerticalAlignment.Center;
            OverlayGrid.Visibility = Visibility.Collapsed;

            // Restore previous focus (modal)
            if (_previousFocus != null && _previousFocus is FrameworkElement fe && fe.IsVisible && fe.IsEnabled)
                fe.Focus();
            _previousFocus = null;
        }
        else
        {
            if (_lightweightContentHost != null)
            {
                await AnimateExitAsync(_lightweightContentHost, ExitAnimation, ExitDuration, ExitEasing);
                // Detach focus trapping handler for lightweight popup
                _lightweightContentHost.PreviewLostKeyboardFocus -= OnLightweightPreviewLostKeyboardFocus;
            }
            if (_lightweightPopup != null)
            {
                _lightweightPopup.IsOpen = false;
                _lightweightPopup = null;
            }
            _lightweightContentHost = null;

            // Restore previous focus (lightweight)
            if (_previousFocus != null && _previousFocus is FrameworkElement fe && fe.IsVisible && fe.IsEnabled)
                fe.Focus();
            _previousFocus = null;
        }
        IsOpen = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handles the Escape key to close the popup if <see cref="CanCloseOnEscape"/> is <c>true</c>.
    /// </summary>
    /// <param name="e">Key event arguments.</param>
    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && IsOpen && CanCloseOnEscape)
        {
            _ = CloseAsync();
            e.Handled = true;
        }
        base.OnPreviewKeyDown(e);
    }
}