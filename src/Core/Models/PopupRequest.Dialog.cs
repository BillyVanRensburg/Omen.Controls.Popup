using System.Collections.Generic;
using Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.Core.Models;

/// <summary>
/// Represents the configuration and content for showing a popup.
/// This partial class contains properties specific to modal dialogs.
/// </summary>
public partial class PopupRequest
{
    /// <summary>
    /// Gets or sets the buttons to display in the dialog (e.g., OK, Cancel, Yes, No).
    /// This is a flags enumeration; combine values using bitwise OR (e.g., <c>DialogAction.OK | DialogAction.Cancel</c>).
    /// Only used when <see cref="IsModal"/> is <c>true</c> and the host supports dialog buttons.
    /// </summary>
    public DialogAction DialogButtons { get; set; } = DialogAction.None;

    /// <summary>
    /// Gets or sets custom labels for dialog buttons. The key is the button action (e.g., "OK", "Cancel", "Yes", "No")
    /// and the value is the displayed text. If a button is not present in this dictionary, the default label is used.
    /// </summary>
    public Dictionary<string, string>? CustomButtonLabels { get; set; }
}