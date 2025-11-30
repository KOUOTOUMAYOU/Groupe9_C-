using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace KeyceWordEditor.Resources
{
    public static class ResourceManager
    {
        private static readonly string dictionariesPath = "Resources/Dictionaries/";
        private static readonly string templatesPath = "Resources/Templates/";
        private static readonly string iconsPath = "Resources/Icons/";

        public static HashSet<string> LoadDictionary(string language)
        {
            var dictionary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var filePath = Path.Combine(dictionariesPath, $"{language}.dic");

            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                        {
                            dictionary.Add(line.Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement dictionnaire: {ex.Message}");
            }

            return dictionary;
        }

        public static List<Template> LoadTemplates()
        {
            var templates = new List<Template>();

            try
            {
                if (Directory.Exists(templatesPath))
                {
                    var templateFiles = Directory.GetFiles(templatesPath, "*.xml");

                    foreach (var file in templateFiles)
                    {
                        try
                        {
                            var template = LoadTemplateFromFile(file);
                            if (template != null)
                            {
                                templates.Add(template);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erreur chargement template {file}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement templates: {ex.Message}");
            }

            return templates;
        }

        private static Template LoadTemplateFromFile(string filePath)
        {
            var doc = new XmlDocument();
            doc.Load(filePath);

            var template = new Template
            {
                Name = doc.SelectSingleNode("//Name")?.InnerText ?? "Sans nom",
                Category = doc.SelectSingleNode("//Category")?.InnerText ?? "Général",
                Description = doc.SelectSingleNode("//Description")?.InnerText ?? "",
                Content = doc.SelectSingleNode("//Content")?.InnerText ?? ""
            };

            // Chargement des placeholders
            var placeholderNodes = doc.SelectNodes("//Placeholders/Placeholder");
            if (placeholderNodes != null)
            {
                foreach (XmlNode node in placeholderNodes)
                {
                    var key = node.Attributes?["key"]?.Value;
                    var description = node.Attributes?["description"]?.Value;

                    if (!string.IsNullOrEmpty(key))
                    {
                        template.Placeholders[key] = description ?? "";
                    }
                }
            }

            return template;
        }

        public static ImageSource GetIcon(string iconKey)
        {
            try
            {
                var resource = Application.Current.FindResource(iconKey);
                return resource as ImageSource;
            }
            catch
            {
                return null;
            }
        }

        public static void InitializeResources()
        {
            // Création des répertoires s'ils n'existent pas
            Directory.CreateDirectory(dictionariesPath);
            Directory.CreateDirectory(templatesPath);
            Directory.CreateDirectory(iconsPath);

            // Chargement des dictionnaires par défaut
            LoadDefaultDictionaries();
            LoadDefaultTemplates();
        }

        private static void LoadDefaultDictionaries()
        {
            // Les dictionnaires sont déjà créés dans les fichiers .dic
            // Cette méthode peut être étendue pour ajouter d'autres langues
        }

        private static void LoadDefaultTemplates()
        {
            // Les templates sont déjà créés dans les fichiers XML
            // Cette méthode peut être étendue pour générer des templates par défaut
        }
    }

    public class Template
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Placeholders { get; set; }
        public string Content { get; set; }

        public Template()
        {
            Placeholders = new Dictionary<string, string>();
        }

        public string ApplyPlaceholders(Dictionary<string, string> values)
        {
            var result = Content;

            foreach (var placeholder in Placeholders)
            {
                var key = $"[{placeholder.Key}]";
                var value = values.ContainsKey(placeholder.Key) ? values[placeholder.Key] : "";
                result = result.Replace(key, value);
            }

            return result;
        }
    }
}