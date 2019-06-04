using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement.ModelBinding;
using System.Threading.Tasks;
using AdvancedForms.ViewModels;
using Newtonsoft.Json.Linq;
using AdvancedForms.Models;
using Microsoft.AspNetCore.Http;
using AdvancedForms.Enums;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.Notify;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.ContentManagement.Records;
using YesSql;
using OrchardCore.Mvc.ActionConstraints;
using AdvancedForms.Helper;
using Newtonsoft.Json;

namespace AdvancedForms.Controllers
{
    [Authorize]
    public class ProfileController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentAliasManager _contentAliasManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly INotifier _notifier;
        private readonly YesSql.ISession _session;

        public ProfileController(
            IContentManager contentManager,
            IContentItemDisplayManager contentItemDisplayManager,
            IAuthorizationService authorizationService,
            IContentAliasManager contentAliasManager,
            INotifier notifier,
            YesSql.ISession session,
            IContentDefinitionManager contentDefinitionManager,
            IHtmlLocalizer<AdvancedFormsController> localizer
            )
        {
            _authorizationService = authorizationService;
            _contentItemDisplayManager = contentItemDisplayManager;
            _contentManager = contentManager;
            _notifier = notifier;
            _contentAliasManager = contentAliasManager;
            _session = session;
            _contentDefinitionManager = contentDefinitionManager;
            T = localizer;
        }
        public IHtmlLocalizer T { get; }


        public IActionResult Test()
        {
            return View();
        }
    }
}