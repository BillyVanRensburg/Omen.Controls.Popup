using Microsoft.Maui.Controls;
using System;

namespace Omen.Controls.Popup.MAUI.Controls;

public partial class OmenPopup : ContentView
{
    public OmenPopup()
    {
        // Initialize the host
        _host = new Hosting.MauiPopupHost();
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        // Any post‑load initialization (none needed currently)
    }
}