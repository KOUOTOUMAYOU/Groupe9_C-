using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;

namespace KeyceWordEditor.Services
{
    public class PrintService
    {
        public void PrintDocument(FlowDocument document)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Document");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur d'impression: {ex.Message}");
            }
        }

        public void PrintPreview(FlowDocument document)
        {
            MessageBox.Show("Fonctionnalité d'aperçu à implémenter");
        }

        public void ConfigurePageSetup()
        {
            var pageSetupDialog = new PrintDialog();
            pageSetupDialog.ShowDialog();
        }
    }
}