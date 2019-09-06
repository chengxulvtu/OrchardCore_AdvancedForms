using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.ViewModels
{
    public class SubmissionsViewModel
    {
        public string DisplayText { get; set; }
        [BindNever]
        public dynamic Pager { get; set; }
        public List<dynamic> ContentItemSummaries { get; set; }
    }
}
