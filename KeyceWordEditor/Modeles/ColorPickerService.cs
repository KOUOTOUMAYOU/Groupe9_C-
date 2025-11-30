using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace KeyceWordEditor.Services
{
    public class ColorPickerService
    {
        public Color? ShowColorPicker(Color? initialColor = null)
        {
            // Version simplifiée sans ColorDialog
            // Retourne une couleur prédéfinie
            var colors = new[]
            {
                Colors.Black, Colors.Red, Colors.Blue, Colors.Green, Colors.Purple,
                Colors.Orange, Colors.Brown, Colors.Pink, Colors.Teal, Colors.Navy
            };

            var currentIndex = 0;
            if (initialColor.HasValue)
            {
                currentIndex = Array.IndexOf(colors, initialColor.Value);
                if (currentIndex == -1) currentIndex = 0;
            }

            var nextIndex = (currentIndex + 1) % colors.Length;
            return colors[nextIndex];
        }

        public void ApplyTextColor(RichTextBox editor, Color color)
        {
            editor.Selection.ApplyPropertyValue(System.Windows.Documents.TextElement.ForegroundProperty,
                                              new SolidColorBrush(color));
        }

        public void ApplyBackgroundColor(RichTextBox editor, Color color)
        {
            editor.Selection.ApplyPropertyValue(System.Windows.Documents.TextElement.BackgroundProperty,
                                              new SolidColorBrush(color));
        }

        public void ApplyHighlightColor(RichTextBox editor, Color color)
        {
            editor.Selection.ApplyPropertyValue(System.Windows.Documents.TextElement.BackgroundProperty,
                                              new SolidColorBrush(color));
        }
    }
}