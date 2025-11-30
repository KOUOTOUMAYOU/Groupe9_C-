using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace KeyceWordEditor
{
    public partial class FindReplaceWindow : Window
    {
        private readonly RichTextBox _editor;
        private TextPointer _currentSearchPosition;

        public FindReplaceWindow(RichTextBox targetEditor)
        {
            InitializeComponent();
            _editor = targetEditor;
            _currentSearchPosition = _editor.Document.ContentStart;

            // Focus sur la zone de recherche
            FindTextBox.Focus();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            FindText(FindTextBox.Text);
        }

        private void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceText(FindTextBox.Text, ReplaceTextBox.Text);
        }

        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceAll(FindTextBox.Text, ReplaceTextBox.Text);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FindText(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Veuillez entrer un texte à rechercher.", "Recherche",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var textRange = new TextRange(_currentSearchPosition, _editor.Document.ContentEnd);
            var index = textRange.Text.IndexOf(searchText);

            if (index >= 0)
            {
                var start = textRange.Start.GetPositionAtOffset(index);
                var end = textRange.Start.GetPositionAtOffset(index + searchText.Length);
                _editor.Selection.Select(start, end);
                _currentSearchPosition = end;
                _editor.Focus();
            }
            else
            {
                MessageBox.Show($"Le texte '{searchText}' n'a pas été trouvé.", "Recherche",
                              MessageBoxButton.OK, MessageBoxImage.Information);
                _currentSearchPosition = _editor.Document.ContentStart;
            }
        }

        private void ReplaceText(string searchText, string replaceText)
        {
            if (string.IsNullOrEmpty(searchText)) return;

            // Si du texte est sélectionné et correspond au texte recherché, on remplace
            if (!string.IsNullOrEmpty(_editor.Selection.Text) &&
                _editor.Selection.Text.Equals(searchText))
            {
                _editor.Selection.Text = replaceText;
            }

            // On cherche l'occurrence suivante
            FindText(searchText);
        }

        private void ReplaceAll(string searchText, string replaceText)
        {
            if (string.IsNullOrEmpty(searchText)) return;

            var documentRange = new TextRange(_editor.Document.ContentStart, _editor.Document.ContentEnd);
            var text = documentRange.Text;
            var newText = text.Replace(searchText, replaceText);

            if (newText != text)
            {
                documentRange.Text = newText;
                MessageBox.Show($"Remplacement effectué : {searchText} → {replaceText}",
                              "Remplacer tout", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Aucune occurrence de '{searchText}' trouvée.",
                              "Remplacer tout", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}