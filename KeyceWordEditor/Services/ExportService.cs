using System.IO;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows;

namespace KeyceWordEditor.Services
{
    public class PdfExportService
    {
        public void ExportToPdf(FlowDocument document, string filePath)
        {
            try
            {
                // Version simplifiée - export en texte brut
                TextRange range = new TextRange(document.ContentStart, document.ContentEnd);
                string textContent = range.Text;

                // Créer un fichier texte avec extension .pdf (simulation)
                File.WriteAllText(filePath, $"PDF EXPORT - {DateTime.Now}\n\n{textContent}");

                MessageBox.Show($"Document exporté en PDF (simulé): {filePath}\n\nPour une vraie exportation PDF, installez iTextSharp ou une autre bibliothèque PDF.",
                              "Export PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur export PDF: {ex.Message}", "Erreur",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}