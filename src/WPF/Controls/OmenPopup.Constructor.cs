using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media; // For VisualTreeHelper

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing the constructor, loaded event, basic UI event handlers,
/// and focus trapping logic for **modal** popups.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OmenPopup"/> control.
    /// Sets up event handlers for overlay click, loaded, and focus trapping.
    /// </summary>
    public OmenPopup()
    {
        InitializeComponent();
        OverlayGrid.MouseLeftButtonDown += OverlayGrid_MouseLeftButtonDown;
        Loaded += OmenPopup_Loaded;

        // Attach focus trapping handler for modal popups
        this.PreviewLostKeyboardFocus += OnModalPreviewLostKeyboardFocus;
    }

    /// <summary>
    /// Handles the <see cref="FrameworkElement.Loaded"/> event.
    /// Ensures the modal content border has the required transform group for animations.
    /// </summary>
    /// <param name="sender">The source of the event (the popup control).</param>
    /// <param name="e">Unused event arguments.</param>
    private void OmenPopup_Loaded(object? sender, RoutedEventArgs e)
    {
        EnsureTransformGroup(ModalContentBorder);
    }

    /// <summary>
    /// Handles the <see cref="UIElement.MouseLeftButtonDown"/> event on the overlay grid.
    /// Closes the popup if <see cref="CloseOnOutsideClick"/> is <c>true</c>.
    /// </summary>
    /// <param name="sender">The overlay grid.</param>
    /// <param name="e">Mouse event arguments.</param>
    private void OverlayGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsOpen && CloseOnOutsideClick)
            _ = CloseAsync();
    }

    /// <summary>
    /// Handles the <see cref="ButtonBase.Click"/> event for the close button.
    /// Asynchronously closes the popup.
    /// </summary>
    /// <param name="sender">The close button.</param>
    /// <param name="e">Unused event arguments.</param>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _ = CloseAsync();
    }

    /// <summary>
    /// Prevents keyboard focus from leaving a **modal** popup when it is open.
    /// If focus is about to move to an element outside the popup, it is redirected back
    /// to the modal content border.
    /// </summary>
    /// <param name="sender">The popup control.</param>
    /// <param name="e">Focus change event arguments.</param>
    private void OnModalPreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        // Only trap focus when the popup is modal, open, and the content border exists.
        if (IsModal && IsOpen && ModalContentBorder != null)
        {
            // If the new focus target is not a child of the popup, redirect focus back.
            if (!IsChildOfModalPopup(e.NewFocus))
            {
                ModalContentBorder.Focus();
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Determines whether a given <see cref="IInputElement"/> is a descendant of the modal popup.
    /// </summary>
    /// <param name="element">The element to test.</param>
    /// <returns>
    /// <c>true</c> if the element is the modal content border, the popup itself,
    /// or a visual child of either; otherwise <c>false</c>.
    /// </returns>
    private bool IsChildOfModalPopup(IInputElement? element)
    {
        if (element == null) return false;
        var fe = element as FrameworkElement;
        while (fe != null)
        {
            if (fe == ModalContentBorder || fe == this)
                return true;
            fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
        }
        return false;
    }
}