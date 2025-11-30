using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace KeyceWordEditor.Models
{
    public class Document
    {
        public string Title { get; set; } = "Nouveau document";
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public FlowDocument Content { get; set; }
        public DocumentMetadata Metadata { get; set; }

        public Document()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            Metadata = new DocumentMetadata();
            Content = new FlowDocument();
        }

        public void UpdateModifiedDate()
        {
            ModifiedDate = DateTime.Now;
        }
    }

    public class DocumentMetadata
    {
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public string Language { get; set; } = "fr-FR";
    }
}