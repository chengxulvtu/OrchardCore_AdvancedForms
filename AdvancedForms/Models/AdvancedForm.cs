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
        public TextPart Group { get; set; }
        public EditorPart Header { get; set; }
        public EditorPart Footer { get; set; }
        public EditorPart Instructions;
        public BooleanPart HideFromListing { get; set; }
        public BooleanPart IsGlobalHeader { get; set; }
        public BooleanPart IsGlobalFooter { get; set; }
        public JSonEditorPart Container, AdminContainer, FormFields;

        public AdvancedForm(string description, string instructions, string container, string title, string header, string footer, string type, string adminContainer, bool isHideFromListing, bool isGlobalHeader, bool isGlobalFooter, string group, string formFields)
        {
            Description = new EditorPart(description);
            Title = title;
            Instructions = new EditorPart(instructions);
            Container = new JSonEditorPart(container);
            FormFields = new JSonEditorPart(formFields);
            AdminContainer = new JSonEditorPart(adminContainer);
            Header = new EditorPart(header);
            Footer = new EditorPart(footer);
            Type = new TextPart(type);
            Group = new TextPart(group);
            HideFromListing = new BooleanPart(isHideFromListing);
            IsGlobalHeader = new BooleanPart(isGlobalHeader);
            IsGlobalFooter = new BooleanPart(isGlobalFooter);
        }

    }
}
