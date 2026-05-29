namespace Omen.Controls.Popup.Sample
{
    using Omen.Controls.Popup.Core.Enums;
    using Microsoft.Maui.Controls;
    using Microsoft.Maui.Graphics;
    using System;

    public partial class MainPage : ContentPage
    {
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();

            // Subscribe to slider changes
            DurationSlider.ValueChanged += (s, e) =>
            {
                DurationLabel.Text = $"{(int)e.NewValue} ms";
            };

            // Subscribe to overlay picker changes
            OverlayBrushPicker.SelectedIndexChanged += (s, e) =>
            {
                // Update preview or apply changes
            };
        }

        private async void OnShowPopupClicked(object? sender, EventArgs e)
        {
            try
            {
                if (myPopup.IsOpen)
                    await myPopup.CloseAsync();

                await Task.Delay(50);

                // Configure popup based on UI selections
                bool isModal = ModalRadio.IsChecked;
                myPopup.IsModal = isModal;

                myPopup.CloseOnOutsideClick = CloseOnOutsideClickCheckBox.IsChecked;
                myPopup.ShowCloseButton = ShowCloseButtonCheckBox.IsChecked;

                // Animation duration
                int duration = (int)DurationSlider.Value;
                myPopup.EnterDuration = duration;
                myPopup.ExitDuration = duration;

                // Overlay brush
                myPopup.OverlayBrush = GetOverlayBrush();

                // Create content based on selection
                if (SimpleTextRadio.IsChecked)
                {
                    myPopup.Content = new VerticalStackLayout
                    {
                        Spacing = 15,
                        Padding = 20,
                        Children =
                        {
                            new Label
                            {
                                Text = "Hello MAUI!",
                                FontSize = 24,
                                FontAttributes = FontAttributes.Bold,
                                HorizontalTextAlignment = TextAlignment.Center
                            },
                            new Label
                            {
                                Text = "This is a popup control for MAUI",
                                FontSize = 14,
                                TextColor = Colors.Gray,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    };
                }
                else
                {
                    myPopup.Content = new VerticalStackLayout
                    {
                        Spacing = 15,
                        Padding = 20,
                        Children =
                        {
                            new Label
                            {
                                Text = "Rich Content Popup",
                                FontSize = 20,
                                FontAttributes = FontAttributes.Bold,
                                HorizontalTextAlignment = TextAlignment.Center
                            },
                            new BoxView
                            {
                                Color = Colors.LightBlue,
                                HeightRequest = 50,
                                CornerRadius = 5
                            },
                            new Label
                            {
                                Text = "This popup contains multiple UI elements:",
                                FontSize = 12,
                                TextColor = Colors.Gray
                            },
                            new Button
                            {
                                Text = "Action Button",
                                BackgroundColor = Colors.Green,
                                TextColor = Colors.White,
                                Padding = 10,
                                Margin = new Thickness(0, 10)
                            },
                            new Label
                            {
                                Text = "Click outside or the close button to dismiss",
                                FontSize = 11,
                                TextColor = Colors.DarkGray,
                                HorizontalTextAlignment = TextAlignment.Center
                            }
                        }
                    };
                }

                // Show the popup
                await myPopup.ShowAsync();
                StatusLabel.Text = $"Popup shown at {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Error: {ex.Message}";
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            try
            {
                count++;
                CounterBtn.Text = $"Click me ({count} times)";

                // Show a simple counter popup
                if (myPopup.IsOpen)
                    await myPopup.CloseAsync();

                await Task.Delay(50);

                myPopup.IsModal = ModalRadio.IsChecked;
                myPopup.CloseOnOutsideClick = true;
                myPopup.ShowCloseButton = true;
                myPopup.OverlayBrush = GetOverlayBrush();

                int duration = (int)DurationSlider.Value;
                myPopup.EnterDuration = duration;
                myPopup.ExitDuration = duration;

                myPopup.Content = new VerticalStackLayout
                {
                    Spacing = 20,
                    Padding = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = $"Click Count: {count}",
                            FontSize = 32,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Purple,
                            HorizontalTextAlignment = TextAlignment.Center
                        },
                        new Label
                        {
                            Text = count == 1 ? "You clicked once!" : $"You've clicked {count} times!",
                            FontSize = 14,
                            TextColor = Colors.Gray,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    }
                };

                await myPopup.ShowAsync();
                StatusLabel.Text = $"Counter: {count} at {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusLabel.Text = $"Error: {ex.Message}";
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private Brush GetOverlayBrush()
        {
            return OverlayBrushPicker.SelectedIndex switch
            {
                0 => new SolidColorBrush(Color.FromArgb("#80000000")), // Black 50%
                1 => new SolidColorBrush(Color.FromArgb("#B3000000")), // Black 70%
                2 => new SolidColorBrush(Color.FromArgb("#801E90FF")), // Blue Tint
                _ => new SolidColorBrush(Color.FromArgb("#80000000"))  // Default
            };
        }
    }
}
