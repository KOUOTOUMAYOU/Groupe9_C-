using System.Windows;
using System.Windows.Media;

namespace KeyceWordEditor.Models
{
    public class TextStyle
    {
        public string Name { get; set; } = "Normal";
        public double FontSize { get; set; } = 12;
        public FontFamily FontFamily { get; set; } = new FontFamily("Segoe UI");
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;
        public FontStyle FontStyle { get; set; } = FontStyles.Normal;
        public Brush Foreground { get; set; } = Brushes.Black;
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
        public Thickness Margin { get; set; } = new Thickness(0);

        public static TextStyle CreateHeading1()
        {
            return new TextStyle
            {
                Name = "Titre 1",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 5)
            };
        }

        public static TextStyle CreateHeading2()
        {
            return new TextStyle
            {
                Name = "Titre 2",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 8, 0, 4)
            };
        }
    }
}