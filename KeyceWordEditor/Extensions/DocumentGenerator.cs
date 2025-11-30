using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace KeyceWordEditor.Extensions
{
    public class DocumentGenerator
    {
        public FlowDocument GenerateFromTemplate(string templateType, Dictionary<string, string> parameters)
        {
            var document = new FlowDocument();

            switch (templateType.ToLower())
            {
                case "lettre":
                    GenerateLetterTemplate(document, parameters);
                    break;
                case "rapport":
                    GenerateReportTemplate(document, parameters);
                    break;
                case "cv":
                    GenerateCVTemplate(document, parameters);
                    break;
                default:
                    GenerateBasicTemplate(document, parameters);
                    break;
            }

            return document;
        }

        private void GenerateLetterTemplate(FlowDocument document, Dictionary<string, string> parameters)
        {
            var from = parameters.ContainsKey("from") ? parameters["from"] : "";
            var to = parameters.ContainsKey("to") ? parameters["to"] : "";
            var date = parameters.ContainsKey("date") ? parameters["date"] : DateTime.Now.ToShortDateString();

            var paragraph1 = new Paragraph(new Run($"{from}\n\n{to}\n\n{date}\n\nObjet : "));
            var paragraph2 = new Paragraph(new Run("Madame, Monsieur,\n\n"));
            var paragraph3 = new Paragraph(new Run("\n\nVeuillez agréer, Madame, Monsieur, l'expression de mes salutations distinguées."));

            document.Blocks.Add(paragraph1);
            document.Blocks.Add(paragraph2);
            document.Blocks.Add(paragraph3);
        }

        private void GenerateReportTemplate(FlowDocument document, Dictionary<string, string> parameters)
        {
            var title = parameters.ContainsKey("title") ? parameters["title"] : "Rapport";
            var author = parameters.ContainsKey("author") ? parameters["author"] : "";
            var date = parameters.ContainsKey("date") ? parameters["date"] : DateTime.Now.ToShortDateString();

            var titleParagraph = new Paragraph(new Run(title))
            {
                FontSize = 16
            };

            var infoParagraph = new Paragraph(new Run($"Auteur: {author}\nDate: {date}"));
            var summaryParagraph = new Paragraph(new Run("Résumé exécutif:\n"));
            var introductionParagraph = new Paragraph(new Run("1. Introduction\n"));
            var conclusionParagraph = new Paragraph(new Run("Conclusion\n"));

            document.Blocks.Add(titleParagraph);
            document.Blocks.Add(infoParagraph);
            document.Blocks.Add(summaryParagraph);
            document.Blocks.Add(introductionParagraph);
            document.Blocks.Add(conclusionParagraph);
        }

        private void GenerateCVTemplate(FlowDocument document, Dictionary<string, string> parameters)
        {
            var name = parameters.ContainsKey("name") ? parameters["name"] : "";
            var email = parameters.ContainsKey("email") ? parameters["email"] : "";
            var phone = parameters.ContainsKey("phone") ? parameters["phone"] : "";

            var headerParagraph = new Paragraph(new Run($"{name}\n{email} | {phone}"))
            {
                FontSize = 14
            };

            var experienceParagraph = new Paragraph(new Run("Expérience professionnelle:\n"));
            var educationParagraph = new Paragraph(new Run("Formation:\n"));
            var skillsParagraph = new Paragraph(new Run("Compétences:\n"));

            document.Blocks.Add(headerParagraph);
            document.Blocks.Add(experienceParagraph);
            document.Blocks.Add(educationParagraph);
            document.Blocks.Add(skillsParagraph);
        }

        private void GenerateBasicTemplate(FlowDocument document, Dictionary<string, string> parameters)
        {
            var title = parameters.ContainsKey("title") ? parameters["title"] : "Nouveau document";

            var titleParagraph = new Paragraph(new Run(title))
            {
                FontSize = 14
            };

            document.Blocks.Add(titleParagraph);
        }
    }
}