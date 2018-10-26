﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement.ModelBinding;
using System.Threading.Tasks;
using AdvancedForms.ViewModels;

namespace AdvancedForms.Controllers
{
    public class AdvancedFormsController : Controller
    {

        private readonly IContentManager _contentManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentAliasManager _contentAliasManager;

        public AdvancedFormsController(
            IContentManager contentManager,
            IContentItemDisplayManager contentItemDisplayManager,
            IAuthorizationService authorizationService,
            IContentAliasManager contentAliasManager
            )
        {
            _authorizationService = authorizationService;
            _contentItemDisplayManager = contentItemDisplayManager;
            _contentManager = contentManager;
            _contentAliasManager = contentAliasManager;
        }

        [Route("AdvancedForms")]
        [Route("AdvancedForms/Index")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Route("AdvancedForms/{alias}")]
        public async Task<IActionResult> Display(string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                await Index(); 
            }
      
            var contentItemId = await _contentAliasManager.GetContentItemIdAsync("slug:AdvancedForms/" + alias);

            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Published);

            if (contentItem == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewContent, contentItem))
            {
                return Unauthorized();
            }

            var model = new AdvancedFormViewModel
            {
                Id = contentItemId,
                Title = contentItem.Content.AdvancedForm.Title,
                Container = contentItem.Content.AdvancedForm.Container.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html
            };

            return View(model);

        }

        [HttpPost]
        [Route("AdvancedForms/Entry")]
        public async Task<ActionResult> Entry(string submission)
        {
            string contentItemId = "";

            var content = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (content == null)
            {
                return NotFound();
            }

            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, content))
            //{
            //    return Unauthorized();
            //}

            return View();

            //return await EditPOST(viewModel, returnUrl, async contentItem =>
            //{
                await _contentManager.PublishAsync(new ContentItem());

            //    var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

            //    _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
            //        ? T["Your content has been published."]
            //        : T["Your {0} has been published.", typeDefinition.DisplayName]);
            //});
        }

    }
}
