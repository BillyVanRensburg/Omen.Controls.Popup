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

        #endregion

        public OmenPopup()
        {
            InitializeComponent();
        }

        public async Task ShowAsync()
        {
            if (IsOpen) return;
            OverlayGrid.Visibility = Visibility.Visible;
            IsOpen = true;
            Focusable = true;
            Focus(); // take focus to capture keyboard events
            await Task.CompletedTask;
        }

        public async Task CloseAsync()
        {
            if (!IsOpen) return;
            OverlayGrid.Visibility = Visibility.Collapsed;
            IsOpen = false;
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