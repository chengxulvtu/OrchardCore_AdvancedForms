using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Models
{
    public class AdvancedFormSubmissions
    {
        public EditorPart Submission, AdminSubmission;
        public string Title { get; set; }
        public string Owner { get; set; }
        public TextPart Type { get; set; }
        public TextPart Status { get; set; }
        public EditorPart Metadata, Container, Header, Footer, Description, Instructions, AdminContainer;
       
        public AdvancedFormSubmissions(string submission, string metadata, string title, string container, string header, 
            string footer, string description, string type, string instructions, string owner, string status, string adminContainer, string adminSubmission)
        {
            Submission = new EditorPart(submission);
            AdminSubmission = new EditorPart(adminSubmission);
            Title = title;
            Metadata = new EditorPart(metadata);
            Container = new EditorPart(container);
            AdminContainer = new EditorPart(adminContainer);
            Header = new EditorPart(header);
            Footer = new EditorPart(footer);
            Description = new EditorPart(description);
            Instructions = new EditorPart(instructions);
            Type = new TextPart(type);
            Owner = owner;
            Status = new TextPart(status);
        }

    }
}
