using System;
using System.IO;
using System.Windows.Documents;
using System.Windows;
using Microsoft.Win32;

namespace KeyceWordEditor.Services
{
    public class DocxService
    {
        public void ExportToDocx(FlowDocument document, string filePath)
        {
            try
            {
                // Pour une implémentation réelle avec DocumentFormat.OpenXml
                // Mais pour l'instant, on va utiliser une solution simple

                // Solution temporaire : exporter en RTF (Word peut l'ouvrir)
                TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    range.Save(fs, DataFormats.Rtf);
                }

                MessageBox.Show($"Document exporté avec succès vers : {filePath}",
                              "Export réussi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'export DOCX : {ex.Message}",
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public FlowDocument? ImportFromDocx(string filePath)
        {
            try
            {
                // Pour l'instant, on va gérer seulement les fichiers texte et RTF
                // Une vraie implémentation DOCX nécessiterait DocumentFormat.OpenXml

                var document = new FlowDocument();
                TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    if (filePath.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
                    {
                        range.Load(fs, DataFormats.Rtf);
                    }
                    else
                    {
                        range.Load(fs, DataFormats.Text);
                    }
                }

                return document;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'import DOCX : {ex.Message}",
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public string? ShowExportDialog()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Document Word (*.docx)|*.docx|Document RTF (*.rtf)|*.rtf|Document texte (*.txt)|*.txt",
                DefaultExt = ".docx",
                Title = "Exporter le document"
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }

        public string? ShowImportDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Documents Word (*.docx;*.doc)|*.docx;*.doc|Documents RTF (*.rtf)|*.rtf|Documents texte (*.txt)|*.txt",
                Title = "Importer un document"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }
    }
}