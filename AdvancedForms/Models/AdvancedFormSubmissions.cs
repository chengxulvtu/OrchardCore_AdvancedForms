using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Models
{
    public class AdvancedFormSubmissions
    {
        public string Title { get; set; }
        public string Owner { get; set; }
        public TextPart ApplicationLocation { get; set; }
        public TextPart AdvancedFormId { get; set; }
        public TextPart Status { get; set; }
        public JSonEditorPart Submission, AdminSubmission, Metadata, FormFields;

        public AdvancedFormSubmissions(string submission, string metadata, string title, string owner, string status, string adminSubmission, string applicationLocation, string formFields, string advancedFormId)
        {
            Submission = new JSonEditorPart(submission);
            AdminSubmission = new JSonEditorPart(adminSubmission);
            Title = title;
            Metadata = new JSonEditorPart(metadata);
            FormFields = new JSonEditorPart(formFields);
            Owner = owner;
            Status = new TextPart(status);
            ApplicationLocation = new TextPart(applicationLocation);
            AdvancedFormId = new TextPart(advancedFormId);
        }
    }
}
