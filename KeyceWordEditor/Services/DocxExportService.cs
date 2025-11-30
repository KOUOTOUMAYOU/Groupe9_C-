using System.IO;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows;

namespace KeyceWordEditor.Services
{
    public class DocxExportService
    {
        public void ExportToDocx(FlowDocument document, string filePath)
        {
            try
            {
                // Pour DOCX, on utilise RTF comme format intermédiaire
                TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    range.Save(fs, DataFormats.Rtf);
                }

                MessageBox.Show($"Document exporté en DOCX (RTF): {filePath}", "Succès",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur export DOCX: {ex.Message}", "Erreur",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public FlowDocument ImportFromDocx(string filePath)
        {
            try
            {
                var document = new FlowDocument();
                TextRange range = new TextRange(document.ContentStart, document.ContentEnd);

                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    // Essayer RTF d'abord, puis texte brut
                    try
                    {
                        range.Load(fs, DataFormats.Rtf);
                    }
                    catch
                    {
                        fs.Position = 0;
                        range.Load(fs, DataFormats.Text);
                    }
                }

                return document;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur import DOCX: {ex.Message}", "Erreur",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}