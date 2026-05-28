using System;
using System.Collections.Generic;
using System.Text;

namespace Omen.Controls.Popup.Application.Enums;

/// <summary>Defines the possible states of the popup lifecycle.</summary>
public enum PopupState
{
    /// <summary>Popup is not visible.</summary>
    Closed,

    /// <summary>Popup is starting to open (before animation).</summary>
    Opening,

    /// <summary>Popup is fully open and visible.</summary>
    Open,

    /// <summary>Popup is starting to close (before animation).</summary>
    Closing
}