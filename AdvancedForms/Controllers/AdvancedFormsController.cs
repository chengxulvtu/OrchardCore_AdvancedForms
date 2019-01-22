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

namespace AdvancedForms.Controllers
{
    public class AdvancedFormsController : Controller, IUpdateModel
    {

        private readonly IContentManager _contentManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentAliasManager _contentAliasManager;
        private readonly YesSql.ISession _session;
        private const string _id = "AdvancedFormSubmissions";

        public AdvancedFormsController(
            IContentManager contentManager,
            IContentItemDisplayManager contentItemDisplayManager,
            IAuthorizationService authorizationService,
            IContentAliasManager contentAliasManager,
            YesSql.ISession session
            )
        {
            _authorizationService = authorizationService;
            _contentItemDisplayManager = contentItemDisplayManager;
            _contentManager = contentManager;
            _contentAliasManager = contentAliasManager;
            _session = session;
        }


        [Route("AdvancedForms/{alias}")]
        public async Task<IActionResult> Display(string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                return Redirect("/");
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
                Title = contentItem.DisplayText,
                Tag = contentItem.Content.AdvancedForm.Tag.Text,
                Container = contentItem.Content.AdvancedForm.Container.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html
            };

            return View(model);

        }

        [HttpPost]
        [Route("AdvancedForms/Entry")]
        public async Task<IActionResult> Entry(string submission, string title, string id, string container, string header, string footer, string description, string tag, string submissionId, string instructions)
        {
            ContentItem contentItem;
            if (!string.IsNullOrWhiteSpace(submissionId))
            {
                contentItem = await _contentManager.GetAsync(submissionId, VersionOptions.Latest);
            }
            else
            {
                contentItem = await _contentManager.NewAsync(_id);
            }
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.SubmitForm, contentItem))
            {
                return Unauthorized();
            }

            var subObject = JObject.Parse(submission);
            string guid = contentItem.ContentItemId;
            string subTitle = title + " " + DateTime.Now.ToUniversalTime().ToString() + " " + guid; 
            var advFormSub = new AdvancedFormSubmissions(subObject["data"].ToString(), 
                subObject["metadata"].ToString(), subTitle, container, header, footer, description, tag, instructions);
            contentItem.Content.AdvancedFormSubmissions = JToken.FromObject(advFormSub);
            contentItem.DisplayText = subTitle;
            contentItem.Content.AutoroutePart.Path = CreatePath(title, guid);

            if (!ModelState.IsValid)
            {
                _session.Cancel();
                return StatusCode(StatusCodes.Status406NotAcceptable);
            }

            if (!string.IsNullOrWhiteSpace(submissionId))
            {
                await _contentManager.UpdateAsync(contentItem);
            }
            else
            {
                await _contentManager.CreateAsync(contentItem, VersionOptions.Draft);
            }

            await _contentManager.PublishAsync(contentItem);
            return StatusCode(StatusCodes.Status201Created);  
        }

        [Route("AdvancedForms/{alias}/Edit/{id}")]
        public async Task<IActionResult> Edit(string alias, string id)
        {
            return await ReturnView(alias, id, EntryType.Edit);
        }

        [Route("AdvancedForms/{alias}/View/{id}")]
        public async Task<IActionResult> View(string alias, string id)
        {
            return await ReturnView(alias, id, EntryType.View);
        }

        [Route("AdvancedForms/{alias}/Print/{id}")]
        public async Task<IActionResult> Print(string alias, string id)
        {
            return await ReturnView(alias, id, EntryType.Print);
        }

        private async Task<IActionResult> ReturnView(string alias, string id, EntryType entryType)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                return Redirect("/");
            }
            else if (String.IsNullOrWhiteSpace(id))
            {
                await Display(alias);
            }

            var contentItemId = await _contentAliasManager.GetContentItemIdAsync("slug:AdvancedForms/" + alias);
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Published);
            var subContentItem = await _contentManager.GetAsync(id, VersionOptions.Latest);
            var viewName = entryType == EntryType.Print ? "Print" : "Display";

            if (contentItem == null)
            {
                return NotFound();
            }

            if (entryType == EntryType.View)
            {
                if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewContent, subContentItem))
                {
                    return Unauthorized();
                }               
            }
            else if (!await _authorizationService.AuthorizeAsync(User, Permissions.SubmitForm, subContentItem))
            {
               return Unauthorized();
            }

            var model = new AdvancedFormViewModel
            {
                Id = contentItemId,
                Title = contentItem.DisplayText,
                Tag = contentItem.Content.AdvancedForm.Tag.Text,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                Container = subContentItem.Content.AdvancedFormSubmissions.Container.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                SubmissionId = subContentItem.ContentItemId,
                Submission = subContentItem.Content.AdvancedFormSubmissions.Submission.Html,
                EntryType = entryType
            };

            return View(viewName, model);

        }

        private string CreatePath(string title, string quid)
        {
            if (!string.IsNullOrEmpty(title))
            {
                title = "AdvancedForms" + "/" + title.Replace(" ", "-") + "/View/" + quid;
            }
            return title;
        }

    }
}
