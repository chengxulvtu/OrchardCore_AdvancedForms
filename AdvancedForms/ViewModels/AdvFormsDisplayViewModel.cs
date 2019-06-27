using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.ViewModels
{
    public class AdvFormsDisplayViewModel
    {
        public string Type { get; set; }
        public List<AdvFormsDisplay> Items { get; set; }
    }

    public class AdvFormsDisplay
    {
        public string DisplayText { get; set; }
        public string Action { get; set; }
        public string ContentItemId { get; set; }
        public string Description { get; set; }
    }
}
