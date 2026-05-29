using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using CoreEnums = Omen.Controls.Popup.Core.Enums;

namespace Omen.Controls.Popup.MAUI.Controls;

public partial class OmenPopup
{
    #region Core Properties

    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(OmenPopup), false,
            propertyChanged: OnIsOpenChanged);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    private static async void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var popup = (OmenPopup)bindable;
        if ((bool)newValue)
            await popup.ShowAsync();
        else
            await popup.CloseAsync();
    }

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(object), typeof(OmenPopup), null);

    // Added 'new' keyword to hide base ContentView.Content property
    public new object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly BindableProperty IsModalProperty =
        BindableProperty.Create(nameof(IsModal), typeof(bool), typeof(OmenPopup), true);

    public bool IsModal
    {
        get => (bool)GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public static readonly BindableProperty CanCloseOnEscapeProperty =
        BindableProperty.Create(nameof(CanCloseOnEscape), typeof(bool), typeof(OmenPopup), true);

    public bool CanCloseOnEscape
    {
        get => (bool)GetValue(CanCloseOnEscapeProperty);
        set => SetValue(CanCloseOnEscapeProperty, value);
    }

    public static readonly BindableProperty CloseOnOutsideClickProperty =
        BindableProperty.Create(nameof(CloseOnOutsideClick), typeof(bool), typeof(OmenPopup), true);

    public bool CloseOnOutsideClick
    {
        get => (bool)GetValue(CloseOnOutsideClickProperty);
        set => SetValue(CloseOnOutsideClickProperty, value);
    }

    public static readonly BindableProperty ShowCloseButtonProperty =
        BindableProperty.Create(nameof(ShowCloseButton), typeof(bool), typeof(OmenPopup), true);

    public bool ShowCloseButton
    {
        get => (bool)GetValue(ShowCloseButtonProperty);
        set => SetValue(ShowCloseButtonProperty, value);
    }

    public static readonly BindableProperty CloseButtonTemplateProperty =
        BindableProperty.Create(nameof(CloseButtonTemplate), typeof(ControlTemplate), typeof(OmenPopup), null);

    public ControlTemplate? CloseButtonTemplate
    {
        get => (ControlTemplate?)GetValue(CloseButtonTemplateProperty);
        set => SetValue(CloseButtonTemplateProperty, value);
    }

    public static readonly BindableProperty OverlayBrushProperty =
        BindableProperty.Create(nameof(OverlayBrush), typeof(Brush), typeof(OmenPopup), null);

    public Brush? OverlayBrush
    {
        get => (Brush?)GetValue(OverlayBrushProperty);
        set => SetValue(OverlayBrushProperty, value);
    }

    public static readonly BindableProperty EnterAnimationProperty =
        BindableProperty.Create(nameof(EnterAnimation), typeof(CoreEnums.AnimationType), typeof(OmenPopup), CoreEnums.AnimationType.Fade);

    public CoreEnums.AnimationType EnterAnimation
    {
        get => (CoreEnums.AnimationType)GetValue(EnterAnimationProperty);
        set => SetValue(EnterAnimationProperty, value);
    }

    public static readonly BindableProperty ExitAnimationProperty =
        BindableProperty.Create(nameof(ExitAnimation), typeof(CoreEnums.AnimationType), typeof(OmenPopup), CoreEnums.AnimationType.Fade);

    public CoreEnums.AnimationType ExitAnimation
    {
        get => (CoreEnums.AnimationType)GetValue(ExitAnimationProperty);
        set => SetValue(ExitAnimationProperty, value);
    }

    public static readonly BindableProperty EnterDurationProperty =
        BindableProperty.Create(nameof(EnterDuration), typeof(int), typeof(OmenPopup), 200);

    public int EnterDuration
    {
        get => (int)GetValue(EnterDurationProperty);
        set => SetValue(EnterDurationProperty, value);
    }

    public static readonly BindableProperty ExitDurationProperty =
        BindableProperty.Create(nameof(ExitDuration), typeof(int), typeof(OmenPopup), 200);

    public int ExitDuration
    {
        get => (int)GetValue(ExitDurationProperty);
        set => SetValue(ExitDurationProperty, value);
    }

    public static readonly BindableProperty EnterEasingProperty =
        BindableProperty.Create(nameof(EnterEasing), typeof(CoreEnums.EasingType), typeof(OmenPopup), CoreEnums.EasingType.EaseOut);

    public CoreEnums.EasingType EnterEasing
    {
        get => (CoreEnums.EasingType)GetValue(EnterEasingProperty);
        set => SetValue(EnterEasingProperty, value);
    }

    public static readonly BindableProperty ExitEasingProperty =
        BindableProperty.Create(nameof(ExitEasing), typeof(CoreEnums.EasingType), typeof(OmenPopup), CoreEnums.EasingType.EaseIn);

    public CoreEnums.EasingType ExitEasing
    {
        get => (CoreEnums.EasingType)GetValue(ExitEasingProperty);
        set => SetValue(ExitEasingProperty, value);
    }

    public static readonly BindableProperty EnterCubicBezierPointsProperty =
        BindableProperty.Create(nameof(EnterCubicBezierPoints), typeof(string), typeof(OmenPopup), null);

    public string? EnterCubicBezierPoints
    {
        get => (string?)GetValue(EnterCubicBezierPointsProperty);
        set => SetValue(EnterCubicBezierPointsProperty, value);
    }

    public static readonly BindableProperty ExitCubicBezierPointsProperty =
        BindableProperty.Create(nameof(ExitCubicBezierPoints), typeof(string), typeof(OmenPopup), null);

    public string? ExitCubicBezierPoints
    {
        get => (string?)GetValue(ExitCubicBezierPointsProperty);
        set => SetValue(ExitCubicBezierPointsProperty, value);
    }

    #endregion

    #region Positioning Properties

    public static readonly BindableProperty AnchorTargetProperty =
        BindableProperty.Create(nameof(AnchorTarget), typeof(CoreEnums.AnchorTarget), typeof(OmenPopup), CoreEnums.AnchorTarget.ParentContainer);

    public CoreEnums.AnchorTarget AnchorTarget
    {
        get => (CoreEnums.AnchorTarget)GetValue(AnchorTargetProperty);
        set => SetValue(AnchorTargetProperty, value);
    }

    public static readonly BindableProperty AnchorElementProperty =
        BindableProperty.Create(nameof(AnchorElement), typeof(View), typeof(OmenPopup), null);

    public View? AnchorElement
    {
        get => (View?)GetValue(AnchorElementProperty);
        set => SetValue(AnchorElementProperty, value);
    }

    public static readonly BindableProperty AlignmentProperty =
        BindableProperty.Create(nameof(Alignment), typeof(CoreEnums.PopupAlignment), typeof(OmenPopup), CoreEnums.PopupAlignment.MiddleCenter);

    public CoreEnums.PopupAlignment Alignment
    {
        get => (CoreEnums.PopupAlignment)GetValue(AlignmentProperty);
        set => SetValue(AlignmentProperty, value);
    }

    public static readonly BindableProperty OffsetXProperty =
        BindableProperty.Create(nameof(OffsetX), typeof(int), typeof(OmenPopup), 0);

    public int OffsetX
    {
        get => (int)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public static readonly BindableProperty OffsetYProperty =
        BindableProperty.Create(nameof(OffsetY), typeof(int), typeof(OmenPopup), 0);

    public int OffsetY
    {
        get => (int)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public static readonly BindableProperty AutoFlipProperty =
        BindableProperty.Create(nameof(AutoFlip), typeof(bool), typeof(OmenPopup), true);

    public bool AutoFlip
    {
        get => (bool)GetValue(AutoFlipProperty);
        set => SetValue(AutoFlipProperty, value);
    }

    public static readonly BindableProperty CustomXProperty =
        BindableProperty.Create(nameof(CustomX), typeof(double), typeof(OmenPopup), 0.0);

    public double CustomX
    {
        get => (double)GetValue(CustomXProperty);
        set => SetValue(CustomXProperty, value);
    }

    public static readonly BindableProperty CustomYProperty =
        BindableProperty.Create(nameof(CustomY), typeof(double), typeof(OmenPopup), 0.0);

    public double CustomY
    {
        get => (double)GetValue(CustomYProperty);
        set => SetValue(CustomYProperty, value);
    }

    #endregion

    #region Dialog Button Properties

    public static readonly BindableProperty DialogButtonsProperty =
        BindableProperty.Create(nameof(DialogButtons), typeof(CoreEnums.DialogAction), typeof(OmenPopup), CoreEnums.DialogAction.None,
            propertyChanged: OnDialogButtonsChanged);

    public CoreEnums.DialogAction DialogButtons
    {
        get => (CoreEnums.DialogAction)GetValue(DialogButtonsProperty);
        set => SetValue(DialogButtonsProperty, value);
    }

    public static readonly BindableProperty CustomButtonLabelsProperty =
        BindableProperty.Create(nameof(CustomButtonLabels), typeof(Dictionary<string, string>), typeof(OmenPopup), null,
            propertyChanged: OnDialogButtonsChanged);

    public Dictionary<string, string>? CustomButtonLabels
    {
        get => (Dictionary<string, string>?)GetValue(CustomButtonLabelsProperty);
        set => SetValue(CustomButtonLabelsProperty, value);
    }

    // ButtonItems collection (for UI binding) – we'll implement later
    // public ObservableCollection<ButtonItem>? ButtonItems { get; set; }

    private static void OnDialogButtonsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var popup = (OmenPopup)bindable;
        popup.OnDialogButtonsChanged();
    }

    private void OnDialogButtonsChanged()
    {
        // Dialog buttons changed - could be used for future implementation
    }

    #endregion
}