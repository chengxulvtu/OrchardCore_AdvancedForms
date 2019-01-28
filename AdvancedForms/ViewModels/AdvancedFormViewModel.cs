

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvancedForms.ViewModels
{
    public class AdvancedFormViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Type { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        [Required]
        public string Description { get; set; }
        public string Instructions { get; set; }
        [Required(ErrorMessage = "Form components is Required")]
        public string Container { get; set; }
        public string Submission { get; set; }
        public string SubmissionId { get; set; }
        public Enums.EntryType EntryType { get; set; }

        [BindNever]
        public IList<ContentPickerItemViewModel> SelectedItems { get; set; }
    }
}
