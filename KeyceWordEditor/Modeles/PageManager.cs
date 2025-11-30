using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace KeyceWordEditor.Models
{
    public class PageManager
    {
        private readonly RichTextBox _editor;
        private int _currentPage = 1;
        private const int MAX_CHARS_PER_PAGE = 1500;

        public PageManager(RichTextBox editor)
        {
            _editor = editor;
        }

        public void AddNewPage()
        {
            var pageBreak = new Paragraph();
            pageBreak.Inlines.Add(new Run("\n--- Nouvelle Page ---\n"));
            pageBreak.Foreground = Brushes.Gray;
            pageBreak.FontStyle = FontStyles.Italic;
            pageBreak.TextAlignment = TextAlignment.Center;

            _editor.Document.Blocks.Add(pageBreak);
            _currentPage++;
        }

        public void CheckAndAddPage()
        {
            var text = new TextRange(_editor.Document.ContentStart, _editor.Document.ContentEnd).Text;
            var totalChars = text.Length;
            var estimatedPages = (totalChars / MAX_CHARS_PER_PAGE) + 1;

            if (estimatedPages > _currentPage)
            {
                AddNewPage();
            }
        }

        public int GetCurrentPage() => _currentPage;
        public int GetTotalPages()
        {
            var text = new TextRange(_editor.Document.ContentStart, _editor.Document.ContentEnd).Text;
            return (text.Length / MAX_CHARS_PER_PAGE) + 1;
        }
    }
}