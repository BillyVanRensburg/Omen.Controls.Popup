using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace Omen.Controls.Popup.Sample;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}