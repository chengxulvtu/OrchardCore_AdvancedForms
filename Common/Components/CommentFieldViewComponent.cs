using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.ViewModels;

namespace Common.Components
{
    public class CommentFieldViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(CommentFieldViewModel model)
        {
            return View("Default", model);
        }
    }
}
