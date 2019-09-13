using Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Components
{
    public class SecurityGroupContentPickerViewComponent : ViewComponent
    {
        public IHtmlLocalizer T { get; }

        public IViewComponentResult Invoke(SecurityGroupViewModel model)
        {
            return View("Default", model);
        }
    }
}

