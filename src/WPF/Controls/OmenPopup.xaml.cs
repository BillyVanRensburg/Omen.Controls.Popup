using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Omen.Controls.Popup.WPF.Controls
{
    public partial class OmenPopup : UserControl
    {
        #region Dependency Properties

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(OmenPopup),
                new PropertyMetadata(false, OnIsOpenChanged));

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

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(OmenPopup));

        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty CanCloseOnEscapeProperty =
            DependencyProperty.Register(nameof(CanCloseOnEscape), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

        public bool CanCloseOnEscape
        {
            get => (bool)GetValue(CanCloseOnEscapeProperty);
            set => SetValue(CanCloseOnEscapeProperty, value);
        }

        public static readonly DependencyProperty CloseOnOverlayClickProperty =
            DependencyProperty.Register(nameof(CloseOnOverlayClick), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

        public bool CloseOnOverlayClick
        {
            get => (bool)GetValue(CloseOnOverlayClickProperty);
            set => SetValue(CloseOnOverlayClickProperty, value);
        }

        public static readonly DependencyProperty ShowCloseButtonProperty =
            DependencyProperty.Register(nameof(ShowCloseButton), typeof(bool), typeof(OmenPopup), new PropertyMetadata(true));

        public bool ShowCloseButton
        {
            get => (bool)GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, value);
        }

        public static readonly DependencyProperty CloseButtonTemplateProperty =
            DependencyProperty.Register(nameof(CloseButtonTemplate), typeof(ControlTemplate), typeof(OmenPopup));

        public ControlTemplate CloseButtonTemplate
        {
            get => (ControlTemplate)GetValue(CloseButtonTemplateProperty);
            set => SetValue(CloseButtonTemplateProperty, value);
        }

        #endregion

        public event EventHandler? Closed;

        public OmenPopup()
        {
            InitializeComponent();
            OverlayGrid.MouseLeftButtonDown += OverlayGrid_MouseLeftButtonDown;
        }

        private void OverlayGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsOpen && CloseOnOverlayClick)
                _ = CloseAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _ = CloseAsync();
        }

        public async Task ShowAsync()
        {
            if (IsOpen) return;
            OverlayGrid.Visibility = Visibility.Visible;
            IsOpen = true;
            Focusable = true;
            Focus();
            await Task.CompletedTask;
        }

        public async Task CloseAsync()
        {
            if (!IsOpen) return;
            OverlayGrid.Visibility = Visibility.Collapsed;
            IsOpen = false;
            Closed?.Invoke(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && IsOpen && CanCloseOnEscape)
            {
                _ = CloseAsync();
                e.Handled = true;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}