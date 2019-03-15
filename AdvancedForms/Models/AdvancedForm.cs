using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Models
{
    public class AdvancedForm
    {
        public EditorPart Description;
        public string Title { get; set; }
        public TextPart Type { get; set; }
        public EditorPart Header { get; set; }
        public EditorPart Footer { get; set; }
        public EditorPart Instructions, Container, HtmlContainer, AdminContainer, AdminHtmlContainer;
       
        public AdvancedForm(string description, string instructions, string container, string title, string header, string footer, string type, string adminContainer, string htmlContainer, string adminHtmlContainer)
        {
            Description = new EditorPart(description);
            Title = title;
            Instructions = new EditorPart(instructions);
            Container = new EditorPart(container);
            HtmlContainer = new EditorPart(htmlContainer);
            AdminHtmlContainer = new EditorPart(adminHtmlContainer);
            AdminContainer = new EditorPart(adminContainer);
            Header = new EditorPart(header);
            Footer = new EditorPart(footer);
            Type = new TextPart(type);
        }

    }
}
