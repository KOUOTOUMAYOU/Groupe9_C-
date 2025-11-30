using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KeyceWordEditor.Dialogs
{
    public partial class TableDialog : Window
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public Color BorderColor { get; private set; }
        public Color BackgroundColor { get; private set; }
        public new double BorderThickness { get; private set; }

        public TableDialog()
        {
            InitializeComponent();
            Rows = 3;
            Columns = 3;
            BorderColor = Colors.Black;
            BackgroundColor = Colors.White;
            BorderThickness = 1;
            UpdatePreview();
        }

        private void RowsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RowsTextBox != null)
            {
                RowsTextBox.Text = ((int)e.NewValue).ToString();
                UpdatePreview();
            }
        }

        private void ColumnsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ColumnsTextBox != null)
            {
                ColumnsTextBox.Text = ((int)e.NewValue).ToString();
                UpdatePreview();
            }
        }

        private void BorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = ShowColorPicker(BorderColor);
            if (color.HasValue)
            {
                BorderColor = color.Value;
                var border = (Border)BorderColorButton.Content;
                border.Background = new SolidColorBrush(BorderColor);
                UpdatePreview();
            }
        }

        private void BackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = ShowColorPicker(BackgroundColor);
            if (color.HasValue)
            {
                BackgroundColor = color.Value;
                var border = (Border)BackgroundColorButton.Content;
                border.Background = new SolidColorBrush(BackgroundColor);
                UpdatePreview();
            }
        }

        private Color? ShowColorPicker(Color currentColor)
        {
            var dialog = new ColorPickerDialog(currentColor);
            if (dialog.ShowDialog() == true)
            {
                return dialog.SelectedColor;
            }
            return null;
        }

        private void UpdatePreview()
        {
            if (PreviewGrid == null || PreviewBorder == null) return;

            PreviewGrid.Children.Clear();
            PreviewGrid.RowDefinitions.Clear();
            PreviewGrid.ColumnDefinitions.Clear();

            int rows = (int)RowsSlider.Value;
            int cols = (int)ColumnsSlider.Value;

            for (int i = 0; i < rows; i++)
                PreviewGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < cols; i++)
                PreviewGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var cell = new Border
                    {
                        BorderBrush = new SolidColorBrush(BorderColor),
                        BorderThickness = new Thickness(1),
                        Background = new SolidColorBrush(BackgroundColor),
                        Margin = new Thickness(2)
                    };

                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    PreviewGrid.Children.Add(cell);
                }
            }

            PreviewBorder.BorderBrush = new SolidColorBrush(BorderColor);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RowsTextBox.Text, out int rows) && rows > 0)
                Rows = rows;
            else
                Rows = 3;

            if (int.TryParse(ColumnsTextBox.Text, out int cols) && cols > 0)
                Columns = cols;
            else
                Columns = 3;

            if (double.TryParse(BorderThicknessTextBox.Text, out double thickness) && thickness > 0)
                BorderThickness = thickness;
            else
                BorderThickness = 1;

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