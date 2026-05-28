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
    /// Closes the popup asynchronously.
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
        }
        else
        {
            if (_lightweightContentHost != null)
                await AnimateExitAsync(_lightweightContentHost, ExitAnimation, ExitDuration, ExitEasing);
            if (_lightweightPopup != null)
            {
                _lightweightPopup.IsOpen = false;
                _lightweightPopup = null;
            }
            _lightweightContentHost = null;
        }
        IsOpen = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
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