using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.ViewModels
{
    public class ContentPickerItemViewModel
    {
        public string ContentItemId { get; set; }
        public string DisplayText { get; set; }
        public bool HasPublished { get; set; }
        public bool HideFromListing { get; set; }
    }
}
