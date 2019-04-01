using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Models
{
    public class AdvancedFormSubmissions
    {
        public string Title { get; set; }
        public string Owner { get; set; }
        public TextPart Type { get; set; }
        public TextPart Status { get; set; }
        //public EditorPart Metadata, Container, HtmlContainer, Header, Footer, Description, Instructions, AdminContainer, AdminHtmlContainer;
        public EditorPart HtmlContainer, Header, Footer, Description, Instructions, AdminHtmlContainer;
        public JSonEditorPart Submission, AdminSubmission, Metadata, Container, AdminContainer;


        public AdvancedFormSubmissions(string submission, string metadata, string title, string container, string header, 
            string footer, string description, string type, string instructions, string owner, string status, string adminContainer, string adminSubmission, string htmlcontainer, string adminHtmlContainer)
        {
            Submission = new JSonEditorPart(submission);
            AdminSubmission = new JSonEditorPart(adminSubmission);
            Title = title;
            Metadata = new JSonEditorPart(metadata);
            Container = new JSonEditorPart(container);
            HtmlContainer = new EditorPart(htmlcontainer);
            AdminContainer = new JSonEditorPart(adminContainer);
            AdminHtmlContainer = new EditorPart(adminHtmlContainer);
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
