using Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Components
{
    public class CurrencyFieldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CurrencyFieldViewModel model)
        {
            return View("Default", model);
        }
    }
}
