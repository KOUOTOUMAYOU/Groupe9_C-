using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KeyceWordEditor.Dialogs
{
    public partial class ShapeDialog : Window
    {
        public UIElement? CreatedShape { get; private set; }
        private Color fillColor = Colors.LightBlue;
        private Color strokeColor = Colors.Black;

        public ShapeDialog()
        {
            InitializeComponent();
            UpdatePreview();
        }

        private void ShapeType_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void Dimension_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePreview();
        }

        private void FillColorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = ShowColorPicker(fillColor);
            if (color.HasValue)
            {
                fillColor = color.Value;
                var border = (Border)FillColorButton.Content;
                border.Background = new SolidColorBrush(fillColor);
                UpdatePreview();
            }
        }

        private void StrokeColorButton_Click(object sender, RoutedEventArgs e)
        {
            var color = ShowColorPicker(strokeColor);
            if (color.HasValue)
            {
                strokeColor = color.Value;
                var border = (Border)StrokeColorButton.Content;
                border.Background = new SolidColorBrush(strokeColor);
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
            if (PreviewCanvas == null) return;

            PreviewCanvas.Children.Clear();
            var shape = CreateShape();

            if (shape != null)
            {
                Canvas.SetLeft(shape, (PreviewCanvas.Width - WidthSlider.Value) / 2);
                Canvas.SetTop(shape, (PreviewCanvas.Height - HeightSlider.Value) / 2);
                PreviewCanvas.Children.Add(shape);
            }
        }

        private UIElement? CreateShape()
        {
            double width = WidthSlider?.Value ?? 150;
            double height = HeightSlider?.Value ?? 150;
            double strokeThickness = StrokeThicknessSlider?.Value ?? 2;

            if (RectangleRadio?.IsChecked == true)
            {
                return new Rectangle
                {
                    Width = width,
                    Height = height,
                    Fill = new SolidColorBrush(fillColor),
                    Stroke = new SolidColorBrush(strokeColor),
                    StrokeThickness = strokeThickness
                };
            }
            else if (EllipseRadio?.IsChecked == true)
            {
                return new Ellipse
                {
                    Width = width,
                    Height = height,
                    Fill = new SolidColorBrush(fillColor),
                    Stroke = new SolidColorBrush(strokeColor),
                    StrokeThickness = strokeThickness
                };
            }
            else if (TriangleRadio?.IsChecked == true)
            {
                var polygon = new Polygon
                {
                    Fill = new SolidColorBrush(fillColor),
                    Stroke = new SolidColorBrush(strokeColor),
                    StrokeThickness = strokeThickness
                };
                polygon.Points.Add(new Point(width / 2, 0));
                polygon.Points.Add(new Point(width, height));
                polygon.Points.Add(new Point(0, height));
                return polygon;
            }
            else if (StarRadio?.IsChecked == true)
            {
                var polygon = new Polygon
                {
                    Fill = new SolidColorBrush(fillColor),
                    Stroke = new SolidColorBrush(strokeColor),
                    StrokeThickness = strokeThickness
                };

                double cx = width / 2;
                double cy = height / 2;
                double outerRadius = Math.Min(width, height) / 2;
                double innerRadius = outerRadius * 0.4;

                for (int i = 0; i < 10; i++)
                {
                    double angle = Math.PI * i / 5 - Math.PI / 2;
                    double radius = (i % 2 == 0) ? outerRadius : innerRadius;
                    polygon.Points.Add(new Point(
                        cx + radius * Math.Cos(angle),
                        cy + radius * Math.Sin(angle)
                    ));
                }
                return polygon;
            }
            else if (ArrowRadio?.IsChecked == true)
            {
                var polygon = new Polygon
                {
                    Fill = new SolidColorBrush(fillColor),
                    Stroke = new SolidColorBrush(strokeColor),
                    StrokeThickness = strokeThickness
                };

                polygon.Points.Add(new Point(0, height * 0.4));
                polygon.Points.Add(new Point(width * 0.7, height * 0.4));
                polygon.Points.Add(new Point(width * 0.7, 0));
                polygon.Points.Add(new Point(width, height / 2));
                polygon.Points.Add(new Point(width * 0.7, height));
                polygon.Points.Add(new Point(width * 0.7, height * 0.6));
                polygon.Points.Add(new Point(0, height * 0.6));
                return polygon;
            }
            else if (LineRadio?.IsChecked == true)
            {
                return new Line
                {
                    X1 = 0,
                    Y1 = height / 2,
                    X2 = width,
                    Y2 = height / 2,
                    Stroke = new SolidColorBrush(strokeColor),
                    StrokeThickness = strokeThickness
                };
            }

            return null;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            CreatedShape = CreateShape();
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