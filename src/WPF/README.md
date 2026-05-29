# Omen.Controls.Popup

A modern, cross‑platform popup control for WPF, MAUI, and Blazor.
This README focuses on the **WPF implementation**.

---

## Features

- **Modal & lightweight modes** – overlay (modal) or floating (lightweight) popups.
- **Full positioning system** – anchor to UI elements, mouse cursor, parent container, or custom coordinates.
- **9 alignment points** – TopLeft, TopCenter, TopRight, LeftCenter, MiddleCenter, RightCenter, BottomLeft, BottomCenter, BottomRight.
- **Offset & auto‑flip** – fine‑tune position and avoid off‑screen.
- **Animations** – Fade, Scale, Slide (top/bottom/left/right), configurable easing (Linear, EaseIn/Out, Cubic Bezier).
- **Close button** – customizable template (default ✖).
- **Built‑in dialog buttons** – OK, Cancel, Yes, No with custom labels.
- **Escape key handling** – configurable.
- **Focus trapping** – for modal popups (focus stays inside).
- **Cross‑platform architecture** – Core + Application layers are platform‑agnostic.

---

## Installation

### NuGet Packages

```powershell
Install-Package Omen.Controls.Popup.Core
Install-Package Omen.Controls.Popup.Application
Install-Package Omen.Controls.Popup.WPF
```

Or via .NET CLI:

```bash
dotnet add package Omen.Controls.Popup.WPF
```

(The Core and Application packages are pulled as dependencies.)

---

## Quick Start (WPF)

### 1. Add namespace to your XAML

```xml
xmlns:omen="clr-namespace:Omen.Controls.Popup.WPF.Controls;assembly=Omen.Controls.Popup.WPF"
```

### 2. Place the control in your window

```xml
<Grid>
    <!-- your UI -->
    <omen:OmenPopup x:Name="MyPopup" />
</Grid>
```

### 3. Show a simple modal popup

```csharp
MyPopup.Content = "Hello, world!";
MyPopup.IsModal = true;
await MyPopup.ShowAsync();
```

### 4. Show a lightweight (floating) popup anchored to a button

```csharp
MyPopup.Content = "Floating info";
MyPopup.IsModal = false;
MyPopup.AnchorElement = myButton;   // the button that triggers the popup
MyPopup.Alignment = PopupAlignment.BottomCenter;
await MyPopup.ShowAsync();
```

---

## Using Custom Controls as Content

The `Content` property accepts any `object`. You can put any `UserControl` or custom control inside the popup, giving you full control over the layout and interactivity.

### Example: Custom dialog with a text box

**Create a UserControl (e.g., `DialogContent.xaml`):**

```xml
<UserControl x:Class="MyApp.DialogContent"
             Background="White" BorderBrush="DarkGray" BorderThickness="1">
    <StackPanel Margin="10">
        <TextBlock Text="Enter your name:" />
        <TextBox x:Name="NameTextBox" />
        <Button Content="OK" Click="OkButton_Click" />
    </StackPanel>
</UserControl>
```

**Code‑behind:**

```csharp
public partial class DialogContent : UserControl
{
    public string EnteredName => NameTextBox.Text;

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        // Find the parent OmenPopup and close it
        var popup = FindParent<OmenPopup>(this);
        popup?.CloseAsync();
    }
}
```

**Show the popup with the custom control:**

```csharp
var dialog = new DialogContent();
MyPopup.Content = dialog;
MyPopup.IsModal = true;
await MyPopup.ShowAsync();

// After popup closes, you can read the name:
string name = dialog.EnteredName;
```

This pattern works for any `UserControl`, allowing complex forms, data entry, or any custom layout.

---

## API Reference

### Dependency Properties (XAML & code)

| Property | Type | Description |
|----------|------|-------------|
| `IsOpen` | `bool` | Gets or sets whether the popup is open. |
| `Content` | `object` | The content to display (string, UIElement, UserControl). |
| `IsModal` | `bool` | `true` = modal (overlay), `false` = lightweight (floating). |
| `CloseOnOutsideClick` | `bool` | For modal: overlay click closes; for lightweight: outside click closes. |
| `CanCloseOnEscape` | `bool` | Whether pressing Escape closes the popup. |
| `ShowCloseButton` | `bool` | Shows a close button (default ✖). |
| `CloseButtonTemplate` | `ControlTemplate` | Custom template for the close button. |
| `OverlayBrush` | `Brush` | Background brush for modal overlay. |

### Positioning

| Property | Type | Description |
|----------|------|-------------|
| `AnchorTarget` | `AnchorTarget` | Where the popup anchors (`ParentContainer`, `UiElement`, `MouseCursor`, `CustomCoordinates`). |
| `AnchorElement` | `FrameworkElement` | The UI element to anchor to (when `AnchorTarget = UiElement`). |
| `Alignment` | `PopupAlignment` | How the popup aligns relative to the anchor point. |
| `OffsetX`, `OffsetY` | `int` | Additional pixel offset from the anchor point. |
| `AutoFlip` | `bool` | Automatically flips popup to stay on‑screen. |
| `CustomX`, `CustomY` | `double` | Coordinates when `AnchorTarget = CustomCoordinates`. |

### Animations

| Property | Type | Description |
|----------|------|-------------|
| `EnterAnimation`, `ExitAnimation` | `AnimationType` | Flags: `Fade`, `Scale`, `SlideTop`, `SlideBottom`, `SlideLeft`, `SlideRight`. |
| `EnterDuration`, `ExitDuration` | `int` | Duration in milliseconds. |
| `EnterEasing`, `ExitEasing` | `EasingType` | `Linear`, `EaseIn`, `EaseOut`, `EaseInOut`, `CubicBezier`. |
| `EnterCubicBezierPoints`, `ExitCubicBezierPoints` | `string` | Four comma‑separated values (e.g., `"0.25,0.1,0.25,1.0"`). |

### Dialog Buttons (built‑in)

| Property | Type | Description |
|----------|------|-------------|
| `DialogButtons` | `DialogAction` | Flags: `OK`, `Cancel`, `Yes`, `No`. |
| `CustomButtonLabels` | `Dictionary<string, string>` | Override button text (e.g., `{"OK","Proceed"}`). |
| `DialogClosed` | `event` | Raised when a built‑in button is clicked. |

### Methods

| Method | Return | Description |
|--------|--------|-------------|
| `ShowAsync()` | `Task` | Shows the popup (modal or lightweight). |
| `ShowDialogAsync()` | `Task<DialogAction>` | Shows modal popup and returns clicked button result. |
| `CloseAsync()` | `Task` | Closes the popup. |

---

## Advanced Examples

### Modal dialog with OK/Cancel buttons

```csharp
MyPopup.Content = "Are you sure?";
MyPopup.IsModal = true;
MyPopup.DialogButtons = DialogAction.OK | DialogAction.Cancel;

DialogAction result = await MyPopup.ShowDialogAsync();
if (result == DialogAction.OK)
{
    // user clicked OK
}
```

### Custom close button template (XAML)

```xml
<omen:OmenPopup>
    <omen:OmenPopup.CloseButtonTemplate>
        <ControlTemplate TargetType="Button">
            <Border Background="Red" Width="30" Height="30" CornerRadius="15">
                <TextBlock Text="✖" Foreground="White" />
            </Border>
        </ControlTemplate>
    </omen:OmenPopup.CloseButtonTemplate>
</omen:OmenPopup>
```

### Attach popup to an element

```csharp
MyPopup.AnchorTarget = AnchorTarget.UiElement;
MyPopup.AnchorElement = myButton;
MyPopup.Alignment = PopupAlignment.BottomCenter;
MyPopup.OffsetX = 5;
MyPopup.OffsetY = 5;
```

### Custom easing (Cubic Bezier)

```csharp
MyPopup.EnterEasing = EasingType.CubicBezier;
MyPopup.EnterCubicBezierPoints = "0.25,0.1,0.25,1.0";
```

---

## Sample Application

The GitHub repository includes a `samples/WPF` project that demonstrates:
- Modal and lightweight popups
- Positioning (anchoring, alignment, offset, auto‑flip)
- Animations (enter/exit, easing, cubic Bezier)
- Built‑in dialog buttons (OK/Cancel/Yes/No with custom labels)
- Custom control (name entry) with result handling
- All configuration via UI (checkboxes, comboboxes, sliders)

---

## Architecture

The solution follows Clean Architecture:

- **Core** – Domain layer (enums, interfaces, models, primitives). No UI dependencies.
- **Application** – Use cases, state machine, positioning calculator. Depends only on Core.
- **WPF** – Infrastructure (platform‑specific implementation). Implements `IPopupHost` and the `OmenPopup` control.

This design allows porting to MAUI or Blazor by implementing the same Core interfaces.

---

## Building from Source

1. Clone the repository:  
   `git clone https://github.com/BillyVanRensburg/Omen.Controls.Popup.git`
2. Open `Omen.Controls.Popup.sln` in Visual Studio 2026 (or later with .NET 10 SDK).
3. Restore NuGet packages and build.

The solution uses GitVersion for automatic versioning based on GitFlow branches (`develop` → `alpha`, `master` → stable).

---

## License

MIT License – see [LICENSE](LICENSE) file.

---

## Contributing

Issues and pull requests are welcome. Please follow the existing coding style and document public members.

---

## Credits

Developed by Billy van Rensburg.

Special thanks to the Omen project.

---

## Future Plans

- MAUI and Blazor adapters
- Full `ShowDialogAsync` result support (already implemented in this version)
- Unit tests
- GitHub Actions CI/CD

---

Enjoy using `Omen.Controls.Popup`! 😊