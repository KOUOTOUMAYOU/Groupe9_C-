using System.IO;
using System.Windows.Documents;
using Microsoft.Win32;
using KeyceWordEditor.Models;
using System.Windows;

namespace KeyceWordEditor.Services
{
    public class FileService
    {
        public Document? LoadDocument(string filePath)
        {
            try
            {
                var document = new Document();
                var flowDoc = new FlowDocument();
                TextRange range = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);

                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    range.Load(fs, DataFormats.Text);
                }

                document.Content = flowDoc;
                document.FilePath = filePath;
                document.Title = Path.GetFileName(filePath);

                return document;
            }
            catch
            {
                return null;
            }
        }

        public bool SaveDocument(Document document, string filePath)
        {
            try
            {
                TextRange range = new TextRange(document.Content.ContentStart, document.Content.ContentEnd);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    range.Save(fs, DataFormats.Text);
                }

                document.FilePath = filePath;
                document.Title = Path.GetFileName(filePath);
                document.UpdateModifiedDate();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string? ShowOpenDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Fichiers texte (*.txt)|*.txt|Fichiers RTF (*.rtf)|*.rtf|Tous les fichiers (*.*)|*.*",
                Title = "Ouvrir un document"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        public string? ShowSaveDialog()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Fichiers texte (*.txt)|*.txt|Fichiers RTF (*.rtf)|*.rtf|Tous les fichiers (*.*)|*.*",
                Title = "Enregistrer le document"
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }
    }
}