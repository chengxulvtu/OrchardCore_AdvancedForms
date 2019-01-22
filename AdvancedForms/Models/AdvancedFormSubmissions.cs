using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Models
{
    public class AdvancedFormSubmissions
    {
        public EditorPart Submission;
        public string Title { get; set; }
        public TextPart Tag { get; set; }
        public EditorPart Metadata, Container, Header, Footer, Description, Instructions;
       
        public AdvancedFormSubmissions(string submission, string metadata, string title, string container, string header, string footer, string description, string tag, string instructions)
        {
            Submission = new EditorPart(submission);
            Title = title;
            Metadata = new EditorPart(metadata);
            Container = new EditorPart(container);
            Header = new EditorPart(header);
            Footer = new EditorPart(footer);
            Description = new EditorPart(description);
            Instructions = new EditorPart(instructions);
            Tag = new TextPart(tag);
        }

    }
}
