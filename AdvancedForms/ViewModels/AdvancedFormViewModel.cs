

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvancedForms.ViewModels
{
    public class AdvancedFormViewModel
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public DateTime? ModifiedUtc { get; set; }
        public DateTime? CreatedUtc { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Type { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        [Required]
        public string Description { get; set; }
        public string Instructions { get; set; }
        [Required(ErrorMessage = "Form components is Required")]
        public string Container { get; set; }
        public string AdminContainer { get; set; }
        public string Submission { get; set; }
        public string AdminSubmission { get; set; }
        public string ApplicationLocation { get; set; }
        public string Metadata { get; set; }
        public string SubmissionId { get; set; }
        public string CaseID { get; set; }
        public Enums.EntryType EntryType { get; set; }

        public string ReturnUrl { get; set; }

        public HTMLFieldViewModel AdminEditor { get; set; }

        public HTMLFieldViewModel PublicEditor { get; set; }

        [BindNever]
        public IList<ContentPickerItemViewModel> SelectedItems { get; set; }
    }
}
