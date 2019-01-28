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
        public EditorPart Instructions, Container;
       
        public AdvancedForm(string description, string instructions, string container, string title, string header, string footer, string type)
        {
            Description = new EditorPart(description);
            Title = title;
            Instructions = new EditorPart(instructions);
            Container = new EditorPart(container);
            Header = new EditorPart(header);
            Footer = new EditorPart(footer);
            Type = new TextPart(type);
        }

    }
}
