using System;
using System.Collections.Generic;
using System.Text;

namespace Omen.Controls.Popup.Core.Enums;

/// <summary>
/// Represents the actions that can be taken in a modal dialog.
/// This enum is used in two ways:
/// <list type="bullet">
///   <item><description>As a <c>[Flags]</c> value, it defines which action buttons are displayed in the dialog (e.g., <c>OK | Cancel</c>).</description></item>
///   <item><description>As a single value, it indicates the action chosen by the user (e.g., <c>OK</c> or <c>Cancel</c>).</description></item>
/// </list>
/// </summary>
[Flags]
public enum DialogAction
{
    /// <summary>No action / no button.</summary>
    None = 0,

    /// <summary>The affirmative action (e.g., OK, Continue).</summary>
    OK = 1,

    /// <summary>The negative action (e.g., Cancel, Abort).</summary>
    Cancel = 2,

    /// <summary>The affirmative response to a yes/no question.</summary>
    Yes = 4,

    /// <summary>The negative response to a yes/no question.</summary>
    No = 8
}
