using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KeyceWordEditor.Dialogs
{
    public partial class ColorPickerDialog : Window
    {
        public Color SelectedColor { get; private set; }
        private bool isUpdating = false;

        public ColorPickerDialog(Color initialColor)
        {
            InitializeComponent();
            SelectedColor = initialColor;
            InitializePresetColors();
            SetColor(initialColor);
        }

        private void InitializePresetColors()
        {
            var presetColors = new[]
            {
                Colors.Black, Colors.White, Colors.Red, Colors.Green, Colors.Blue,
                Colors.Yellow, Colors.Cyan, Colors.Magenta, Colors.Orange, Colors.Purple,
                Colors.Pink, Colors.Brown, Colors.Gray, Colors.LightGray, Colors.DarkGray,
                Colors.LightBlue, Colors.LightGreen, Colors.LightPink, Colors.LightYellow,
                Colors.DarkRed, Colors.DarkGreen, Colors.DarkBlue, Colors.Gold, Colors.Silver
            };

            foreach (var color in presetColors)
            {
                var button = new Button
                {
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(2),
                    Background = new SolidColorBrush(color),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };

                button.Click += (s, e) => SetColor(color);
                PresetColorsPanel.Children.Add(button);
            }
        }

        private void SetColor(Color color)
        {
            isUpdating = true;

            RedSlider.Value = color.R;
            GreenSlider.Value = color.G;
            BlueSlider.Value = color.B;

            UpdatePreview();

            isUpdating = false;
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isUpdating)
            {
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            if (PreviewBorder == null) return;

            byte r = (byte)RedSlider.Value;
            byte g = (byte)GreenSlider.Value;
            byte b = (byte)BlueSlider.Value;

            SelectedColor = Color.FromRgb(r, g, b);
            PreviewBorder.Background = new SolidColorBrush(SelectedColor);

            if (!isUpdating && HexTextBox != null)
            {
                HexTextBox.Text = $"#{r:X2}{g:X2}{b:X2}";
            }
        }

        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdating) return;

            try
            {
                string hex = HexTextBox.Text.Replace("#", "");
                if (hex.Length == 6)
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);

                    isUpdating = true;
                    RedSlider.Value = r;
                    GreenSlider.Value = g;
                    BlueSlider.Value = b;
                    UpdatePreview();
                    isUpdating = false;
                }
            }
            catch
            {
                // Ignorer les valeurs invalides
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}