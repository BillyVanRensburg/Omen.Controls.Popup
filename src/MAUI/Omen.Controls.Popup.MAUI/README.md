# OmenPopup - .NET MAUI Control

A fully-featured, cross-platform popup control for .NET MAUI applications with support for animations, custom positioning, modal/lightweight modes, and dialog buttons.

## Features

### Core Features
- ✅ **Modal & Lightweight Modes** - Display popups with or without overlay
- ✅ **Async Operations** - All operations are fully async-compatible
- ✅ **Cross-Platform** - Works on Android, iOS, macOS (Catalyst), and Windows
- ✅ **Customizable Content** - Display any View, text, or rich layouts
- ✅ **Gesture Handling** - Tap outside to close, frame tap interception
- ✅ **Event System** - Opening, Opened, Closing, DialogClosed events

### Animation Features
- ✅ **Multiple Animation Types**
  - Fade (opacity animation)
  - Scale (zoom in/out)
  - Slide (directional slide: Top, Bottom, Left, Right)
- ✅ **Configurable Duration** - Enter/Exit animations with custom durations
- ✅ **Easing Functions** - Linear, EaseIn, EaseOut, EaseInOut, CubicBezier support

### Positioning Features
- ✅ **Anchor Targets**
  - ParentContainer (relative to page/window)
  - UIElement (anchor to specific control)
  - MouseCursor (follow cursor)
  - CustomCoordinates (absolute screen position)
- ✅ **Alignment Options** - 9 alignment positions (TopLeft, TopCenter, etc.)
- ✅ **Offset Control** - Fine-tune position with X/Y offsets
- ✅ **Auto-Flip** - Automatically adjust position if popup would go off-screen

### Dialog Features
- ✅ **Built-in Dialog Buttons** - OK, Cancel, Yes, No combinations
- ✅ **Custom Button Labels** - Override default button text
- ✅ **Dialog Result Handling** - Get user's choice via events or tasks
- ✅ **Awaitable Dialog** - Use ShowDialogAsync() for sequential dialogs

### Appearance Customization
- ✅ **Custom Overlay** - Configure overlay color and transparency
- ✅ **Close Button** - Configurable visibility and custom templates
- ✅ **Content Wrapping** - Auto-wraps content in styled frame with shadow

## Installation

```xml
<ItemGroup>
	<ProjectReference Include="path\to\Omen.Controls.Popup.MAUI\Omen.Controls.Popup.MAUI.csproj" />
</ItemGroup>
```

## Quick Start

### XAML Usage

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
			 xmlns:local="clr-namespace:Omen.Controls.Popup.MAUI.Controls;assembly=Omen.Controls.Popup.MAUI"
			 x:Class="MyApp.MainPage">
	<Grid>
		<!-- Your page content -->
		<Button Text="Show Popup" Clicked="OnShowPopupClicked" />

		<!-- Popup Control -->
		<local:OmenPopup x:Name="myPopup" />
	</Grid>
</ContentPage>
```

### C# Code-Behind

```csharp
private async void OnShowPopupClicked(object sender, EventArgs e)
{
	myPopup.IsModal = true;
	myPopup.CloseOnOutsideClick = true;
	myPopup.ShowCloseButton = true;
	myPopup.EnterAnimation = AnimationType.Fade | AnimationType.Scale;
	myPopup.EnterDuration = 300;

	myPopup.Content = new VerticalStackLayout
	{
		Padding = 20,
		Spacing = 15,
		Children =
		{
			new Label { Text = "Hello MAUI!", FontSize = 24, FontAttributes = FontAttributes.Bold },
			new Button { Text = "Close", Clicked = (s,e) => myPopup.CloseAsync() }
		}
	};

	await myPopup.ShowAsync();
}
```

## Bindable Properties

### Core Properties
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsOpen` | bool | false | Gets or sets whether the popup is open |
| `Content` | object | null | Gets or sets the content to display |
| `IsModal` | bool | true | Gets or sets whether the popup is modal (with overlay) |
| `CanCloseOnEscape` | bool | true | Gets or sets whether Escape key closes popup |
| `CloseOnOutsideClick` | bool | true | Gets or sets whether clicking outside closes popup |
| `ShowCloseButton` | bool | true | Gets or sets whether the close button is visible |
| `CloseButtonTemplate` | ControlTemplate | null | Gets or sets the close button template |
| `OverlayBrush` | Brush | #80000000 | Gets or sets the overlay color/transparency |

### Animation Properties
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `EnterAnimation` | AnimationType | Fade | Gets or sets the entrance animation type |
| `ExitAnimation` | AnimationType | Fade | Gets or sets the exit animation type |
| `EnterDuration` | int | 200 | Gets or sets the enter animation duration (ms) |
| `ExitDuration` | int | 200 | Gets or sets the exit animation duration (ms) |
| `EnterEasing` | EasingType | EaseOut | Gets or sets the enter animation easing |
| `ExitEasing` | EasingType | EaseIn | Gets or sets the exit animation easing |

### Positioning Properties
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AnchorTarget` | AnchorTarget | ParentContainer | Gets or sets the anchor point for positioning |
| `AnchorElement` | View | null | Gets or sets the UI element to anchor to |
| `Alignment` | PopupAlignment | MiddleCenter | Gets or sets the alignment relative to anchor |
| `OffsetX` | int | 0 | Gets or sets the horizontal offset (pixels) |
| `OffsetY` | int | 0 | Gets or sets the vertical offset (pixels) |
| `AutoFlip` | bool | true | Gets or sets whether to auto-flip if off-screen |
| `CustomX` | double | 0.0 | Gets or sets the custom X coordinate |
| `CustomY` | double | 0.0 | Gets or sets the custom Y coordinate |

### Dialog Properties
| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DialogButtons` | DialogAction | None | Gets or sets which dialog buttons to show |
| `CustomButtonLabels` | Dictionary | null | Gets or sets custom labels for dialog buttons |

## Events

```csharp
myPopup.Opening += (s, e) => Debug.WriteLine("Popup opening...");
myPopup.Opened += (s, e) => Debug.WriteLine("Popup opened");
myPopup.Closing += (s, e) => Debug.WriteLine("Popup closing...");
myPopup.DialogClosed += (s, e) => 
{
	Debug.WriteLine($"Dialog result: {e.Result}");
};
```

## Methods

### ShowAsync()
Shows the popup asynchronously. The behavior (modal or lightweight) is determined by the `IsModal` property.

```csharp
await myPopup.ShowAsync();
```

### CloseAsync()
Closes the popup asynchronously.

```csharp
await myPopup.CloseAsync();
```

### ShowDialogAsync()
Shows the popup as a dialog and waits for the user to make a choice.

```csharp
var result = await myPopup.ShowDialogAsync();
if (result.Result == DialogAction.OK)
{
	// User clicked OK
}
```

## Animation Types

```csharp
public enum AnimationType
{
	None = 0,
	Fade = 1,
	Scale = 2,
	SlideLeft = 4,
	SlideRight = 8,
	SlideTop = 16,
	SlideBottom = 32
}
```

Use bitwise OR to combine animations:
```csharp
myPopup.EnterAnimation = AnimationType.Fade | AnimationType.Scale;
```

## Easing Types

```csharp
public enum EasingType
{
	Linear,
	EaseIn,
	EaseOut,
	EaseInOut,
	CubicBezier
}
```

## Positioning Examples

### Center on Screen (Default)
```csharp
myPopup.AnchorTarget = AnchorTarget.ParentContainer;
myPopup.Alignment = PopupAlignment.MiddleCenter;
```

### Anchor to Button
```csharp
myPopup.AnchorTarget = AnchorTarget.UiElement;
myPopup.AnchorElement = myButton;
myPopup.Alignment = PopupAlignment.BottomCenter;
myPopup.OffsetY = 10; // 10px below button
```

### Follow Mouse Cursor
```csharp
myPopup.AnchorTarget = AnchorTarget.MouseCursor;
myPopup.Alignment = PopupAlignment.BottomRight;
```

### Custom Screen Coordinates
```csharp
myPopup.AnchorTarget = AnchorTarget.CustomCoordinates;
myPopup.CustomX = 100;
myPopup.CustomY = 200;
```

## Dialog Example

```csharp
private async void OnShowDialogClicked(object sender, EventArgs e)
{
	myPopup.DialogButtons = DialogAction.Yes | DialogAction.No | DialogAction.Cancel;
	myPopup.CustomButtonLabels = new Dictionary<string, string>
	{
		{ "Yes", "I Agree" },
		{ "No", "Decline" },
		{ "Cancel", "Ask Later" }
	};

	myPopup.Content = new Label 
	{ 
		Text = "Do you want to continue?",
		Padding = 20
	};

	var result = await myPopup.ShowDialogAsync();

	string message = result.Result switch
	{
		DialogAction.Yes => "You agreed!",
		DialogAction.No => "You declined.",
		DialogAction.Cancel => "You'll decide later.",
		_ => "No selection"
	};

	await DisplayAlert("Result", message, "OK");
}
```

## Sample Application

See `samples/MAUI/Omen.Controls.Popup.Sample/` for a complete working example with:
- Animation configuration
- Multiple content types
- Overlay customization
- Interactive buttons
- Real-time status updates

## Platform Notes

- **Android**: Fully supported
- **iOS**: Fully supported
- **macOS (Catalyst)**: Fully supported
- **Windows**: Fully supported

## License

MIT License - See LICENSE file for details

## Version

Built for .NET 10 and MAUI framework

---

**Note**: This control provides feature parity with the WPF OmenPopup control while adapting to MAUI's architectural patterns.
