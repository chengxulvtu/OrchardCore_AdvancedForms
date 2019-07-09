using Common.Helpers;
using Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Components
{
    public class GoogleAddressFieldViewComponent : ViewComponent
    {
        public IHtmlLocalizer T { get; }

        public IViewComponentResult Invoke(GoogleAddressFieldViewModel model)
        {
            return View("Default", model);
        }
    }
}
