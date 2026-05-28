using System.Windows;
using System.Windows.Controls;
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

    /// <summary>Identifies the <see cref="CloseOnOverlayClick"/> dependency property.</summary>
    public static readonly DependencyProperty CloseOnOverlayClickProperty =
        DependencyProperty.Register(nameof(CloseOnOverlayClick), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether clicking the overlay closes the popup (modal only).</summary>
    public bool CloseOnOverlayClick
    {
        get => (bool)GetValue(CloseOnOverlayClickProperty);
        set => SetValue(CloseOnOverlayClickProperty, value);
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

    /// <summary>Identifies the <see cref="StaysOpenOnOutsideClick"/> dependency property.</summary>
    public static readonly DependencyProperty StaysOpenOnOutsideClickProperty =
        DependencyProperty.Register(nameof(StaysOpenOnOutsideClick), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether the lightweight popup stays open when clicking outside.</summary>
    public bool StaysOpenOnOutsideClick
    {
        get => (bool)GetValue(StaysOpenOnOutsideClickProperty);
        set => SetValue(StaysOpenOnOutsideClickProperty, value);
    }

    /// <summary>Identifies the <see cref="AnchorElement"/> dependency property.</summary>
    public static readonly DependencyProperty AnchorElementProperty =
        DependencyProperty.Register(nameof(AnchorElement), typeof(FrameworkElement), typeof(OmenPopup));

    /// <summary>Gets or sets the element to which the lightweight popup anchors (if any).</summary>
    public FrameworkElement? AnchorElement
    {
        get => (FrameworkElement?)GetValue(AnchorElementProperty);
        set => SetValue(AnchorElementProperty, value);
    }

    #endregion

    #region Modal Positioning Properties

    /// <summary>Identifies the <see cref="ModalAnchorTarget"/> dependency property.</summary>
    public static readonly DependencyProperty ModalAnchorTargetProperty =
        DependencyProperty.Register(nameof(ModalAnchorTarget), typeof(CoreEnums.AnchorTarget), typeof(OmenPopup), new PropertyMetadata(CoreEnums.AnchorTarget.ParentContainer));

    /// <summary>Gets or sets the anchor target for modal popup positioning.</summary>
    public CoreEnums.AnchorTarget ModalAnchorTarget
    {
        get => (CoreEnums.AnchorTarget)GetValue(ModalAnchorTargetProperty);
        set => SetValue(ModalAnchorTargetProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalAnchorElement"/> dependency property.</summary>
    public static readonly DependencyProperty ModalAnchorElementProperty =
        DependencyProperty.Register(nameof(ModalAnchorElement), typeof(FrameworkElement), typeof(OmenPopup));

    /// <summary>Gets or sets the UI element for modal anchoring (when AnchorTarget = UiElement).</summary>
    public FrameworkElement? ModalAnchorElement
    {
        get => (FrameworkElement?)GetValue(ModalAnchorElementProperty);
        set => SetValue(ModalAnchorElementProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalAlignment"/> dependency property.</summary>
    public static readonly DependencyProperty ModalAlignmentProperty =
        DependencyProperty.Register(nameof(ModalAlignment), typeof(CoreEnums.PopupAlignment), typeof(OmenPopup), new PropertyMetadata(CoreEnums.PopupAlignment.MiddleCenter));

    /// <summary>Gets or sets the alignment of the modal popup relative to the anchor point.</summary>
    public CoreEnums.PopupAlignment ModalAlignment
    {
        get => (CoreEnums.PopupAlignment)GetValue(ModalAlignmentProperty);
        set => SetValue(ModalAlignmentProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalOffsetX"/> dependency property.</summary>
    public static readonly DependencyProperty ModalOffsetXProperty =
        DependencyProperty.Register(nameof(ModalOffsetX), typeof(int), typeof(OmenPopup), new PropertyMetadata(0));

    /// <summary>Gets or sets the horizontal offset (pixels) for modal popup positioning.</summary>
    public int ModalOffsetX
    {
        get => (int)GetValue(ModalOffsetXProperty);
        set => SetValue(ModalOffsetXProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalOffsetY"/> dependency property.</summary>
    public static readonly DependencyProperty ModalOffsetYProperty =
        DependencyProperty.Register(nameof(ModalOffsetY), typeof(int), typeof(OmenPopup), new PropertyMetadata(0));

    /// <summary>Gets or sets the vertical offset (pixels) for modal popup positioning.</summary>
    public int ModalOffsetY
    {
        get => (int)GetValue(ModalOffsetYProperty);
        set => SetValue(ModalOffsetYProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalAutoFlip"/> dependency property.</summary>
    public static readonly DependencyProperty ModalAutoFlipProperty =
        DependencyProperty.Register(nameof(ModalAutoFlip), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

    /// <summary>Gets or sets whether the modal popup automatically flips to avoid off‑screen.</summary>
    public bool ModalAutoFlip
    {
        get => (bool)GetValue(ModalAutoFlipProperty);
        set => SetValue(ModalAutoFlipProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalCustomX"/> dependency property.</summary>
    public static readonly DependencyProperty ModalCustomXProperty =
        DependencyProperty.Register(nameof(ModalCustomX), typeof(double), typeof(OmenPopup), new PropertyMetadata(0.0));

    /// <summary>Gets or sets the custom X coordinate (when AnchorTarget = CustomCoordinates).</summary>
    public double ModalCustomX
    {
        get => (double)GetValue(ModalCustomXProperty);
        set => SetValue(ModalCustomXProperty, value);
    }

    /// <summary>Identifies the <see cref="ModalCustomY"/> dependency property.</summary>
    public static readonly DependencyProperty ModalCustomYProperty =
        DependencyProperty.Register(nameof(ModalCustomY), typeof(double), typeof(OmenPopup), new PropertyMetadata(0.0));

    /// <summary>Gets or sets the custom Y coordinate (when AnchorTarget = CustomCoordinates).</summary>
    public double ModalCustomY
    {
        get => (double)GetValue(ModalCustomYProperty);
        set => SetValue(ModalCustomYProperty, value);
    }

    #endregion

    #region Lightweight Positioning Properties

    /// <summary>Identifies the <see cref="LightweightAlignment"/> dependency property.</summary>
    public static readonly DependencyProperty LightweightAlignmentProperty =
        DependencyProperty.Register(nameof(LightweightAlignment), typeof(CoreEnums.PopupAlignment), typeof(OmenPopup), new PropertyMetadata(CoreEnums.PopupAlignment.BottomCenter));

    /// <summary>Gets or sets the alignment for lightweight popups when anchored to a UI element.</summary>
    public CoreEnums.PopupAlignment LightweightAlignment
    {
        get => (CoreEnums.PopupAlignment)GetValue(LightweightAlignmentProperty);
        set => SetValue(LightweightAlignmentProperty, value);
    }

    /// <summary>Identifies the <see cref="LightweightOffsetX"/> dependency property.</summary>
    public static readonly DependencyProperty LightweightOffsetXProperty =
        DependencyProperty.Register(nameof(LightweightOffsetX), typeof(int), typeof(OmenPopup), new PropertyMetadata(5));

    /// <summary>Gets or sets the horizontal offset for lightweight popups.</summary>
    public int LightweightOffsetX
    {
        get => (int)GetValue(LightweightOffsetXProperty);
        set => SetValue(LightweightOffsetXProperty, value);
    }

    /// <summary>Identifies the <see cref="LightweightOffsetY"/> dependency property.</summary>
    public static readonly DependencyProperty LightweightOffsetYProperty =
        DependencyProperty.Register(nameof(LightweightOffsetY), typeof(int), typeof(OmenPopup), new PropertyMetadata(5));

    /// <summary>Gets or sets the vertical offset for lightweight popups.</summary>
    public int LightweightOffsetY
    {
        get => (int)GetValue(LightweightOffsetYProperty);
        set => SetValue(LightweightOffsetYProperty, value);
    }

    #endregion
}