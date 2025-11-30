using System.Collections.Generic;

namespace KeyceWordEditor.Models
{
    public class Template
    {
        public string Name { get; set; } = "Modèle sans nom";
        public string Category { get; set; } = "Général";
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, string> Placeholders { get; set; } = new Dictionary<string, string>();
        public string Content { get; set; } = string.Empty;

        public static Template CreateBusinessLetter()
        {
            return new Template
            {
                Name = "Lettre d'affaires",
                Category = "Lettres",
                Description = "Modèle de lettre professionnelle",
                Placeholders = new Dictionary<string, string>
                {
                    {"from", "Expéditeur"},
                    {"to", "Destinataire"},
                    {"date", "Date"},
                    {"subject", "Objet"}
                },
                Content = @"[from]

[to]

[date]

Objet: [subject]

Madame, Monsieur,



Veuillez agréer, Madame, Monsieur, l'expression de mes salutations distinguées."
            };
        }
    }
}