using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;

namespace KeyceWordEditor.Extensions
{
    public class SpellCheckerModule
    {
        public void CheckSpelling(RichTextBox editor)
        {
            // Active le correcteur orthographique intégré de WPF
            editor.SpellCheck.IsEnabled = true;

            MessageBox.Show("Correcteur orthographique activé!\n\n" +
                          "Les mots mal orthographiés seront soulignés en rouge.\n" +
                          "Faites un clic droit pour voir les suggestions.",
                          "Correcteur Activé",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ToggleSpellCheck(RichTextBox editor)
        {
            bool isEnabled = !editor.SpellCheck.IsEnabled;
            editor.SpellCheck.IsEnabled = isEnabled;

            string message = isEnabled ?
                "✅ Correcteur orthographique ACTIVÉ\nLes erreurs seront soulignées en rouge." :
                "❌ Correcteur orthographique DÉSACTIVÉ";

            MessageBox.Show(message, "Correcteur",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void SetSpellCheckLanguage(RichTextBox editor, string languageCode)
        {
            try
            {
                // Définit la langue pour le correcteur (si supporté)
                var culture = new System.Globalization.CultureInfo(languageCode);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

                MessageBox.Show($"Langue du correcteur définie sur : {culture.DisplayName}",
                              "Langue", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur de configuration de langue: {ex.Message}",
                              "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}