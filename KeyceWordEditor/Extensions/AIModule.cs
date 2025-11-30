using System;
using System.Threading.Tasks;
using System.Windows;

namespace KeyceWordEditor.Extensions
{
    public class AIModule
    {
        // Supprimé HttpClient qui n'était pas utilisé
        public AIModule()
        {
            // Constructeur simplifié
        }

        public async Task<string> ImproveText(string text)
        {
            try
            {
                await Task.Delay(100); // Simulation de traitement

                return await Task.Run(() =>
                {
                    // Logique simple d'amélioration de texte
                    if (string.IsNullOrWhiteSpace(text))
                        return text;

                    // Capitalise la première lettre de chaque phrase
                    var sentences = text.Split('.', '!', '?');
                    for (int i = 0; i < sentences.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(sentences[i]))
                        {
                            sentences[i] = sentences[i].Trim();
                            if (sentences[i].Length > 0)
                            {
                                sentences[i] = char.ToUpper(sentences[i][0]) +
                                              sentences[i].Substring(1).ToLower();
                            }
                        }
                    }

                    return string.Join(". ", sentences) + " 🚀";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur IA: {ex.Message}");
                return text;
            }
        }

        public async Task<string> GenerateText(string prompt)
        {
            try
            {
                await Task.Delay(200);

                return await Task.Run(() =>
                {
                    return $"**Texte généré par IA**\n\n" +
                           $"Prompt: {prompt}\n\n" +
                           $"Ceci est un texte généré automatiquement basé sur votre demande. " +
                           $"Dans une version complète, cette fonctionnalité utiliserait une API IA " +
                           $"comme OpenAI GPT pour générer du contenu pertinent.\n\n" +
                           $"📝 **Exemple de contenu** :\n" +
                           $"Le sujet '{prompt}' est très intéressant et mérite d'être approfondi. " +
                           $"On pourrait aborder plusieurs aspects comme l'historique, les applications " +
                           $"pratiques et les perspectives futures.";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de génération: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<string> SummarizeText(string text)
        {
            try
            {
                await Task.Delay(150);

                return await Task.Run(() =>
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return text;

                    var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length <= 100)
                        return text;

                    // Crée un résumé avec les 100 premiers mots
                    var summary = string.Join(" ", words, 0, 100);
                    return summary + "... [résumé automatique] 📋";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de résumé: {ex.Message}");
                return text;
            }
        }

        public async Task<string> CorrectGrammar(string text)
        {
            try
            {
                await Task.Delay(120);

                return await Task.Run(() =>
                {
                    // Simulation de correction grammaticale
                    if (string.IsNullOrWhiteSpace(text))
                        return text;

                    // Remplace les doubles espaces
                    text = text.Replace("  ", " ");

                    // Ajoute des points si manquants en fin de phrase
                    if (!text.EndsWith(".") && !text.EndsWith("!") && !text.EndsWith("?"))
                    {
                        text += ".";
                    }

                    return text + " ✓";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de correction: {ex.Message}");
                return text;
            }
        }
    }
}