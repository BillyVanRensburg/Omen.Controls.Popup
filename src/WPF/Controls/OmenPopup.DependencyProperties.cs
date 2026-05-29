using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.WPF.Controls;

/// <summary>
/// Partial class containing all dependency properties for the OmenPopup control.
/// </summary>
public partial class OmenPopup
{
    #region Core Properties

    /// <summary>Identifies the <see cref="IsOpen"/> dependency property.</summary>
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(OmenPopup),
            new PropertyMetadata(false, OnIsOpenChanged));

    /// <summary>Gets or sets whether the popup is open.</summary>
    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    private static async void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var popup = (OmenPopup)d;
        if ((bool)e.NewValue)
            await popup.ShowAsync();
        else
            await popup.CloseAsync();
    }

    /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
    public static new readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(OmenPopup));

    /// <summary>Gets or sets the content to display.</summary>
    public new object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>Identifies the <see cref="IsModal"/> dependency property.</summary>
    public static readonly DependencyProperty IsModalProperty =
        DependencyProperty.Register(nameof(IsModal), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether the popup is modal (shows overlay, blocks parent).</summary>
    public bool IsModal
    {
        get => (bool)GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    /// <summary>Identifies the <see cref="CanCloseOnEscape"/> dependency property.</summary>
    public static readonly DependencyProperty CanCloseOnEscapeProperty =
        DependencyProperty.Register(nameof(CanCloseOnEscape), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether pressing Escape closes the popup.</summary>
    public bool CanCloseOnEscape
    {
        get => (bool)GetValue(CanCloseOnEscapeProperty);
        set => SetValue(CanCloseOnEscapeProperty, value);
    }

    /// <summary>Identifies the <see cref="CloseOnOutsideClick"/> dependency property.</summary>
    public static readonly DependencyProperty CloseOnOutsideClickProperty =
        DependencyProperty.Register(nameof(CloseOnOutsideClick), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether clicking outside the popup closes it (works for both modal and lightweight).</summary>
    public bool CloseOnOutsideClick
    {
        get => (bool)GetValue(CloseOnOutsideClickProperty);
        set => SetValue(CloseOnOutsideClickProperty, value);
    }

    /// <summary>Identifies the <see cref="ShowCloseButton"/> dependency property.</summary>
    public static readonly DependencyProperty ShowCloseButtonProperty =
        DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether the close button is visible.</summary>
    public bool ShowCloseButton
    {
        get => (bool)GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, value);
    }

    /// <summary>Identifies the <see cref="CloseButtonTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty CloseButtonTemplateProperty =
        DependencyProperty.Register(nameof(CloseButtonTemplate), typeof(ControlTemplate), typeof(OmenPopup));

    /// <summary>Gets or sets the template for the close button.</summary>
    public ControlTemplate CloseButtonTemplate
    {
        get => (ControlTemplate)GetValue(CloseButtonTemplateProperty);
        set => SetValue(CloseButtonTemplateProperty, value);
    }

    /// <summary>Identifies the <see cref="OverlayBrush"/> dependency property.</summary>
    public static readonly DependencyProperty OverlayBrushProperty =
        DependencyProperty.Register(nameof(OverlayBrush), typeof(Brush), typeof(OmenPopup), new PropertyMetadata(Brushes.Transparent));

    /// <summary>Gets or sets the overlay brush (modal only).</summary>
    public Brush OverlayBrush
    {
        get => (Brush)GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    /// <summary>Identifies the <see cref="EnterAnimation"/> dependency property.</summary>
    public static readonly DependencyProperty EnterAnimationProperty =
        DependencyProperty.Register(nameof(EnterAnimation), typeof(CoreEnums.AnimationType), typeof(OmenPopup), new PropertyMetadata(CoreEnums.AnimationType.Fade));

    /// <summary>Gets or sets the animation type for opening.</summary>
    public CoreEnums.AnimationType EnterAnimation
    {
        get => (CoreEnums.AnimationType)GetValue(EnterAnimationProperty);
        set => SetValue(EnterAnimationProperty, value);
    }

    /// <summary>Identifies the <see cref="ExitAnimation"/> dependency property.</summary>
    public static readonly DependencyProperty ExitAnimationProperty =
        DependencyProperty.Register(nameof(ExitAnimation), typeof(CoreEnums.AnimationType), typeof(OmenPopup), new PropertyMetadata(CoreEnums.AnimationType.Fade));

    /// <summary>Gets or sets the animation type for closing.</summary>
    public CoreEnums.AnimationType ExitAnimation
    {
        get => (CoreEnums.AnimationType)GetValue(ExitAnimationProperty);
        set => SetValue(ExitAnimationProperty, value);
    }

    /// <summary>Identifies the <see cref="EnterDuration"/> dependency property.</summary>
    public static readonly DependencyProperty EnterDurationProperty =
        DependencyProperty.Register(nameof(EnterDuration), typeof(int), typeof(OmenPopup), new PropertyMetadata(200));

    /// <summary>Gets or sets the duration (ms) of the enter animation.</summary>
    public int EnterDuration
    {
        get => (int)GetValue(EnterDurationProperty);
        set => SetValue(EnterDurationProperty, value);
    }

    /// <summary>Identifies the <see cref="ExitDuration"/> dependency property.</summary>
    public static readonly DependencyProperty ExitDurationProperty =
        DependencyProperty.Register(nameof(ExitDuration), typeof(int), typeof(OmenPopup), new PropertyMetadata(200));

    /// <summary>Gets or sets the duration (ms) of the exit animation.</summary>
    public int ExitDuration
    {
        get => (int)GetValue(ExitDurationProperty);
        set => SetValue(ExitDurationProperty, value);
    }

    /// <summary>Identifies the <see cref="EnterEasing"/> dependency property.</summary>
    public static readonly DependencyProperty EnterEasingProperty =
        DependencyProperty.Register(nameof(EnterEasing), typeof(CoreEnums.EasingType), typeof(OmenPopup), new PropertyMetadata(CoreEnums.EasingType.EaseOut));

    /// <summary>Gets or sets the easing function for the enter animation.</summary>
    public CoreEnums.EasingType EnterEasing
    {
        get => (CoreEnums.EasingType)GetValue(EnterEasingProperty);
        set => SetValue(EnterEasingProperty, value);
    }

    /// <summary>Identifies the <see cref="ExitEasing"/> dependency property.</summary>
    public static readonly DependencyProperty ExitEasingProperty =
        DependencyProperty.Register(nameof(ExitEasing), typeof(CoreEnums.EasingType), typeof(OmenPopup), new PropertyMetadata(CoreEnums.EasingType.EaseIn));

    /// <summary>Gets or sets the easing function for the exit animation.</summary>
    public CoreEnums.EasingType ExitEasing
    {
        get => (CoreEnums.EasingType)GetValue(ExitEasingProperty);
        set => SetValue(ExitEasingProperty, value);
    }

    // ------------------------------------------------------------------------
    // Cubic Bezier Points (for custom easing curves)
    // ------------------------------------------------------------------------

    /// <summary>Identifies the <see cref="EnterCubicBezierPoints"/> dependency property.</summary>
    public static readonly DependencyProperty EnterCubicBezierPointsProperty =
        DependencyProperty.Register(nameof(EnterCubicBezierPoints), typeof(string), typeof(OmenPopup), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the cubic Bezier control points for the enter animation when <see cref="EnterEasing"/> is <see cref="CoreEnums.EasingType.CubicBezier"/>.
    /// Format: "X1,Y1,X2,Y2" (e.g., "0.25,0.1,0.25,1.0"). Each value should be between 0 and 1.
    /// </summary>
    public string? EnterCubicBezierPoints
    {
        get => (string?)GetValue(EnterCubicBezierPointsProperty);
        set => SetValue(EnterCubicBezierPointsProperty, value);
    }

    /// <summary>Identifies the <see cref="ExitCubicBezierPoints"/> dependency property.</summary>
    public static readonly DependencyProperty ExitCubicBezierPointsProperty =
        DependencyProperty.Register(nameof(ExitCubicBezierPoints), typeof(string), typeof(OmenPopup), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the cubic Bezier control points for the exit animation when <see cref="ExitEasing"/> is <see cref="CoreEnums.EasingType.CubicBezier"/>.
    /// Format: "X1,Y1,X2,Y2" (e.g., "0.25,0.1,0.25,1.0"). Each value should be between 0 and 1.
    /// </summary>
    public string? ExitCubicBezierPoints
    {
        get => (string?)GetValue(ExitCubicBezierPointsProperty);
        set => SetValue(ExitCubicBezierPointsProperty, value);
    }

    // ------------------------------------------------------------------------
    // Dialog Button Properties (built‑in buttons)
    // ------------------------------------------------------------------------

    /// <summary>Represents a single dialog button with its display text and a command to close the popup.</summary>
    public class ButtonItem
    {
        /// <summary>Button display text.</summary>
        public string? Text { get; set; }
        /// <summary>Command to execute when the button is clicked.</summary>
        public ICommand Command { get; set; } = null!;
    }

    /// <summary>Identifies the <see cref="DialogButtons"/> dependency property.</summary>
    public static readonly DependencyProperty DialogButtonsProperty =
        DependencyProperty.Register(nameof(DialogButtons), typeof(CoreEnums.DialogAction), typeof(OmenPopup),
            new PropertyMetadata(CoreEnums.DialogAction.None, OnDialogButtonsChanged));

    /// <summary>
    /// Gets or sets the buttons to display in the dialog (e.g., OK, Cancel, Yes, No).
    /// This is a flags enumeration; combine values using bitwise OR (e.g., <c>DialogAction.OK | DialogAction.Cancel</c>).
    /// Only used when the popup is modal and no custom content is provided.
    /// </summary>
    public CoreEnums.DialogAction DialogButtons
    {
        get => (CoreEnums.DialogAction)GetValue(DialogButtonsProperty);
        set => SetValue(DialogButtonsProperty, value);
    }

    /// <summary>Identifies the <see cref="CustomButtonLabels"/> dependency property.</summary>
    public static readonly DependencyProperty CustomButtonLabelsProperty =
        DependencyProperty.Register(nameof(CustomButtonLabels), typeof(Dictionary<string, string>), typeof(OmenPopup),
            new PropertyMetadata(null, OnDialogButtonsChanged));

    /// <summary>
    /// Gets or sets custom labels for dialog buttons. The key is the button action (e.g., "OK", "Cancel", "Yes", "No")
    /// and the value is the displayed text. If a button is not present in this dictionary, the default label is used.
    /// </summary>
    public Dictionary<string, string>? CustomButtonLabels
    {
        get => (Dictionary<string, string>?)GetValue(CustomButtonLabelsProperty);
        set => SetValue(CustomButtonLabelsProperty, value);
    }

    /// <summary>Identifies the <see cref="ButtonItems"/> dependency property.</summary>
    public static readonly DependencyProperty ButtonItemsProperty =
        DependencyProperty.Register(nameof(ButtonItems), typeof(ObservableCollection<ButtonItem>), typeof(OmenPopup),
            new PropertyMetadata(null));

    /// <summary>
    /// Gets the collection of button items displayed in the dialog panel.
    /// This property is updated automatically when <see cref="DialogButtons"/> or <see cref="CustomButtonLabels"/> changes.
    /// </summary>
    public ObservableCollection<ButtonItem>? ButtonItems
    {
        get => (ObservableCollection<ButtonItem>?)GetValue(ButtonItemsProperty);
        set => SetValue(ButtonItemsProperty, value);
    }

    private static void OnDialogButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((OmenPopup)d).OnDialogButtonsChanged();
    }

    /// <summary>
    /// Called when <see cref="DialogButtons"/> or <see cref="CustomButtonLabels"/> changes.
    /// Implemented in a partial class to update the button panel.
    /// </summary>
    partial void OnDialogButtonsChanged();

    #endregion

    #region Positioning Properties (common for modal and lightweight)

    /// <summary>Identifies the <see cref="AnchorTarget"/> dependency property.</summary>
    public static readonly DependencyProperty AnchorTargetProperty =
        DependencyProperty.Register(nameof(AnchorTarget), typeof(CoreEnums.AnchorTarget), typeof(OmenPopup), new PropertyMetadata(CoreEnums.AnchorTarget.ParentContainer));

    /// <summary>Gets or sets the anchor target for popup positioning.</summary>
    public CoreEnums.AnchorTarget AnchorTarget
    {
        get => (CoreEnums.AnchorTarget)GetValue(AnchorTargetProperty);
        set => SetValue(AnchorTargetProperty, value);
    }

    /// <summary>Identifies the <see cref="AnchorElement"/> dependency property.</summary>
    public static readonly DependencyProperty AnchorElementProperty =
        DependencyProperty.Register(nameof(AnchorElement), typeof(FrameworkElement), typeof(OmenPopup));

    /// <summary>Gets or sets the UI element for anchoring (when AnchorTarget = UiElement).</summary>
    public FrameworkElement? AnchorElement
    {
        get => (FrameworkElement?)GetValue(AnchorElementProperty);
        set => SetValue(AnchorElementProperty, value);
    }

    /// <summary>Identifies the <see cref="Alignment"/> dependency property.</summary>
    public static readonly DependencyProperty AlignmentProperty =
        DependencyProperty.Register(nameof(Alignment), typeof(CoreEnums.PopupAlignment), typeof(OmenPopup), new PropertyMetadata(CoreEnums.PopupAlignment.MiddleCenter));

    /// <summary>Gets or sets the alignment of the popup relative to the anchor point.</summary>
    public CoreEnums.PopupAlignment Alignment
    {
        get => (CoreEnums.PopupAlignment)GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    /// <summary>Identifies the <see cref="OffsetX"/> dependency property.</summary>
    public static readonly DependencyProperty OffsetXProperty =
        DependencyProperty.Register(nameof(OffsetX), typeof(int), typeof(OmenPopup), new PropertyMetadata(0));

    /// <summary>Gets or sets the horizontal offset (pixels) from the anchor point.</summary>
    public int OffsetX
    {
        get => (int)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    /// <summary>Identifies the <see cref="OffsetY"/> dependency property.</summary>
    public static readonly DependencyProperty OffsetYProperty =
        DependencyProperty.Register(nameof(OffsetY), typeof(int), typeof(OmenPopup), new PropertyMetadata(0));

    /// <summary>Gets or sets the vertical offset (pixels) from the anchor point.</summary>
    public int OffsetY
    {
        get => (int)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    /// <summary>Identifies the <see cref="AutoFlip"/> dependency property.</summary>
    public static readonly DependencyProperty AutoFlipProperty =
        DependencyProperty.Register(nameof(AutoFlip), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether the popup automatically flips to avoid off‑screen.</summary>
    public bool AutoFlip
    {
        get => (bool)GetValue(AutoFlipProperty);
        set => SetValue(AutoFlipProperty, value);
    }

    /// <summary>Identifies the <see cref="CustomX"/> dependency property.</summary>
    public static readonly DependencyProperty CustomXProperty =
        DependencyProperty.Register(nameof(CustomX), typeof(double), typeof(OmenPopup), new PropertyMetadata(0.0));

    /// <summary>Gets or sets the custom X coordinate (when AnchorTarget = CustomCoordinates).</summary>
    public double CustomX
    {
        get => (double)GetValue(CustomXProperty);
        set => SetValue(CustomXProperty, value);
    }

    /// <summary>Identifies the <see cref="CustomY"/> dependency property.</summary>
    public static readonly DependencyProperty CustomYProperty =
        DependencyProperty.Register(nameof(CustomY), typeof(double), typeof(OmenPopup), new PropertyMetadata(0.0));

    /// <summary>Gets or sets the custom Y coordinate (when AnchorTarget = CustomCoordinates).</summary>
    public double CustomY
    {
        get => (double)GetValue(CustomYProperty);
        set => SetValue(CustomYProperty, value);
    }

    #endregion
}