using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class that implements built‑in dialog button generation for OmenPopup.
/// </summary>
public partial class OmenPopup
{
    /// <summary>
    /// Occurs when a built‑in dialog button is clicked, providing the result.
    /// </summary>
    public event EventHandler<CoreEnums.DialogAction>? DialogClosed;

    /// <summary>
    /// Called when <see cref="DialogButtons"/> or <see cref="CustomButtonLabels"/> changes.
    /// Updates the button panel visibility and button items.
    /// </summary>
    partial void OnDialogButtonsChanged()
    {
        UpdateDialogButtons();
    }

    /// <summary>
    /// Updates the <see cref="ButtonItems"/> collection based on the current <see cref="DialogButtons"/> flags
    /// and <see cref="CustomButtonLabels"/> dictionary. Also shows/hides the dialog buttons panel.
    /// </summary>
    private void UpdateDialogButtons()
    {
        if (!IsModal || DialogButtons == CoreEnums.DialogAction.None)
        {
            if (DialogButtonsPanel != null)
                DialogButtonsPanel.Visibility = Visibility.Collapsed;
            ButtonItems = null;
            return;
        }

        var items = new ObservableCollection<ButtonItem>();
        var flags = DialogButtons;

        void AddButton(string defaultLabel, CoreEnums.DialogAction action)
        {
            if ((flags & action) == 0) return;
            string label = defaultLabel;
            if (CustomButtonLabels != null && CustomButtonLabels.TryGetValue(defaultLabel, out string? custom) && !string.IsNullOrWhiteSpace(custom))
                label = custom;
            items.Add(new ButtonItem
            {
                Text = label,
                Command = new RelayCommand(() => CloseWithResult(action))
            });
        }

        AddButton("OK", CoreEnums.DialogAction.OK);
        AddButton("Cancel", CoreEnums.DialogAction.Cancel);
        AddButton("Yes", CoreEnums.DialogAction.Yes);
        AddButton("No", CoreEnums.DialogAction.No);

        ButtonItems = items;
        if (DialogButtonsPanel != null)
            DialogButtonsPanel.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Closes the popup and raises the <see cref="DialogClosed"/> event with the selected result,
    /// and completes the <see cref="_dialogTcs"/> if it exists.
    /// </summary>
    /// <param name="result">The dialog result to return.</param>
    private void CloseWithResult(CoreEnums.DialogAction result)
    {
        DialogClosed?.Invoke(this, result);
        if (_dialogTcs != null && !_dialogTcs.Task.IsCompleted)
            _dialogTcs.SetResult(result);
        _ = CloseAsync();
    }

    /// <summary>
    /// Simple RelayCommand implementation for the button commands.
    /// </summary>
    private class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
    }
}