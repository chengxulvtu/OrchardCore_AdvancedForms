using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class GoogleAddressFieldViewModel
    {
        public string ID { get; set; }
        public string Value { get; set; }
        public LocalizedHtmlString Title { get; set; }
        public LocalizedHtmlString Hint { get; set; }
        public string GoogleAPI { get; set; }
    }
}
