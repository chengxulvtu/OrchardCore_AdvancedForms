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
using OrchardCore.Settings;
using OrchardCore.Entities;
using System.Linq;
using System.Security.Claims;

namespace AdvancedForms.Controllers
{
    [Authorize]
    public class AdvancedFormsController : Controller, IUpdateModel
    {

        private readonly IContentManager _contentManager;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentAliasManager _contentAliasManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly INotifier _notifier;
        private readonly ISiteService _siteService;
        private readonly YesSql.ISession _session;
        private const string _id = "AdvancedFormSubmissions";

        public AdvancedFormsController(
            IContentManager contentManager,
            IContentItemDisplayManager contentItemDisplayManager,
            IAuthorizationService authorizationService,
            IContentAliasManager contentAliasManager,
            INotifier notifier,
            ISiteService siteService,
            YesSql.ISession session,
            IContentDefinitionManager contentDefinitionManager,
            IHtmlLocalizer<AdvancedFormsController> localizer
            )
        {
            _authorizationService = authorizationService;
            _contentItemDisplayManager = contentItemDisplayManager;
            _contentManager = contentManager;
            _notifier = notifier;
            _siteService = siteService;
            _contentAliasManager = contentAliasManager;
            _session = session;
            _contentDefinitionManager = contentDefinitionManager;
            T = localizer;
        }

        public IHtmlLocalizer T { get; }

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
                Type = contentItem.Content.AdvancedForm.Type.Text,
                Group = contentItem.Content.AdvancedForm.Group.Text,
                Container = contentItem.Content.AdvancedForm.Container.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.Container.Html) : String.Empty,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                CaseID = "",
                HideFromListing = contentItem.Content.AdvancedForm.HideFromListing.Value,
                IsGlobalHeader = contentItem.Content.AdvancedForm.IsGlobalHeader.Value,
                IsGlobalFooter = contentItem.Content.AdvancedForm.IsGlobalFooter.Value,
            };
            return View(model);
        }

        [Route("AdvancedForms/{alias}/Case/{caseID}")]
        public async Task<IActionResult> Display(string alias, string caseID)
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
                Type = contentItem.Content.AdvancedForm.Type.Text,
                Group = contentItem.Content.AdvancedForm.Group.Text,
                Container = contentItem.Content.AdvancedForm.Container.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.Container.Html) : String.Empty,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                CaseID = caseID,
                HideFromListing = contentItem.Content.AdvancedForm.HideFromListing.Value,
                IsGlobalHeader = contentItem.Content.AdvancedForm.IsGlobalHeader.Value,
                IsGlobalFooter = contentItem.Content.AdvancedForm.IsGlobalFooter.Value
            };
            return View(model);
        }

        [HttpPost]
        [Route("AdvancedForms/SaveUpdatePublicComment")]
        public async Task<IActionResult> SavePublicComment(string id, string contentItemId, string comment, string attachment)
        {
            ContentItem content;
            if (!string.IsNullOrWhiteSpace(contentItemId))
            {
                content = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);
            }
            else
            {
                content = await _contentManager.NewAsync("PublicComment");
                await _contentManager.CreateAsync(content, VersionOptions.Draft);
            }

            attachment = string.IsNullOrEmpty(attachment) ? "" : attachment;

            var model = new CommentPart(comment, attachment);


            await _contentManager.PublishAsync(content);

            //return Ok(StatusCodes.Status200OK);
            int returnCode = await new ContentHelper(_contentManager, _session, _contentDefinitionManager, _contentAliasManager).EditCommentPOST(content.ContentItemId, true, id, User.Identity.Name, model, async contentItem =>
              {
                  await _contentManager.PublishAsync(contentItem);
                  var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                  _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                      ? T["Your content has been published."]
                      : T["Your {0} has been published.", typeDefinition.DisplayName]);
              });

            if (returnCode == StatusCodes.Status204NoContent)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(returnCode);
            }
        }

        [Route("AdvancedForms")]
        [AllowAnonymous]
        public async Task<IActionResult> AdvancedForms()
        {
            List<AdvFormsDisplayViewModel> model = new List<AdvFormsDisplayViewModel>();
            AdvFormsDisplayViewModel displayModel;
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var allAdvancedForms = await query.Where(o => o.ContentType == "AdvancedForm" && o.Published).ListAsync();
            query = _session.Query<ContentItem, ContentItemIndex>();
            var allAdvancedFormTypes = await query.Where(o => o.ContentType == "AdvancedFormTypes" && o.Published).ListAsync();
            allAdvancedFormTypes = allAdvancedFormTypes.Where(o => !Boolean.Parse(o.Content.AdvancedFormTypes.HideFromListing.Value.ToString()));
            foreach (var item in allAdvancedFormTypes)
            {
                displayModel = new AdvFormsDisplayViewModel();
                var filteredForms = allAdvancedForms.Where(o => o.Content.AdvancedForm.Type.Text.ToString() == item.ContentItemId && !Boolean.Parse(o.Content.AdvancedForm.HideFromListing.Value.ToString())).ToList();
                displayModel.Items = GetAFDisplayList(filteredForms, true);
                if (displayModel.Items.Count > 0)
                {
                    displayModel.Type = item.DisplayText;
                    model.Add(displayModel);
                }
            }
            return View(model);
        }

        [Route("submission-confirmation")]
        [AllowAnonymous]
        public IActionResult Thankyou()
        {
            return View();
        }

        [Route("DownloadableForms")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadableForms()
        {
            List<AdvFormsDisplayViewModel> model = new List<AdvFormsDisplayViewModel>();
            AdvFormsDisplayViewModel displayModel;
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var allDownloadForms = await query.Where(o => o.ContentType == "DownloadableForm" && o.Published).ListAsync();
            query = _session.Query<ContentItem, ContentItemIndex>();
            var allDonwloadFormTypes = await query.Where(o => o.ContentType == "AdvancedFormTypes" && o.Published).ListAsync();
            allDonwloadFormTypes = allDonwloadFormTypes.Where(o => !Boolean.Parse(o.Content.AdvancedFormTypes.HideFromListing.Value.ToString()));
            foreach (var item in allDonwloadFormTypes)
            {
                displayModel = new AdvFormsDisplayViewModel();
                var filteredForms = allDownloadForms.Where(o => o.Content.DownloadableForm.FormType.ContentItemIds[0].ToString() == item.ContentItemId).ToList();
                displayModel.Items = GetAFDisplayList(filteredForms, false);
                if (displayModel.Items.Count > 0)
                {
                    displayModel.Type = item.DisplayText;
                    model.Add(displayModel);
                }
            }
            return View(model);
        }

        public List<AdvFormsDisplay> GetAFDisplayList(List<ContentItem> contentList, bool IsAdvancedForm)
        {
            List<string> userRoles = null;
            if (IsAdvancedForm)
            {
                userRoles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                if (userRoles.Count == 0)
                {
                    userRoles.Add("CITIZEN");
                }
            }
            string[] groups;
            List<AdvFormsDisplay> lstDisplay = new List<AdvFormsDisplay>();
            AdvFormsDisplay AFdisplay = null;
            foreach (var form in contentList)
            {
                if (IsAdvancedForm)
                {
                    groups = form.Content.AdvancedForm.Group.Text.ToString().Split(',');
                    if (userRoles.Any(o => o == "Administrator" || groups.Contains(o)))
                    {
                        AFdisplay = new AdvFormsDisplay()
                        {
                            Action = form.Content.AutoroutePart.Path.ToString(),
                            Description = form.Content.AdvancedForm.Description.Html
                        };
                    }
                }
                else
                {
                    AFdisplay = new AdvFormsDisplay()
                    {
                        Action = form.Content.DownloadableForm.UploadFile.Paths[0].ToString(),
                        Description = form.Content.DownloadableForm.Description.Html
                    };
                }
                if (AFdisplay != null)
                {
                    AFdisplay.ContentItemId = form.ContentItemId;
                    AFdisplay.DisplayText = form.DisplayText;
                    lstDisplay.Add(AFdisplay);
                    AFdisplay = null;
                }
            }
            return lstDisplay;
        }

        [HttpGet]
        [Route("AdvancedForms/GetPublicComments")]
        public async Task<IActionResult> GetPublicComments(string id)
        {
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var comments = await query.Where(o => o.ContentType == "PublicComment" && o.DisplayText == id && (o.Latest || o.Published)).ListAsync();
            return Ok(comments);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ContactUsSubmission")]
        public async Task<IActionResult> ContactUsSubmission(string name, string email, string phoneNumber, string message)
        {
            ContentItem contentItem = await _contentManager.NewAsync("ContactUs");
            await _contentManager.CreateAsync(contentItem, VersionOptions.Draft);
            var model = new ContactUs(name, email, phoneNumber, message);
            contentItem.Content.ContactUs = JToken.FromObject(model);
            await _contentManager.PublishAsync(contentItem);
            return Ok();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("AdvancedForms/Entry")]
        public async Task<IActionResult> Entry(string submission, string title, string id, string container,
            string header, string footer, string description, string type, string submissionId, string instructions, string owner, bool isDraft, bool hideFromListing, bool isGlobalHeader, bool isGlobalFooter, string group)
        {
            ContentItem content;
            string adminSubmission = string.Empty;
            if (!string.IsNullOrWhiteSpace(submissionId))
            {
                content = await _contentManager.GetAsync(submissionId, VersionOptions.Latest);
                adminSubmission = content.Content.AdvancedFormSubmissions.AdminSubmission != null ?
                    content.Content.AdvancedFormSubmissions.AdminSubmission.Html.ToString() : null;
            }
            else
            {
                content = await _contentManager.NewAsync(_id);
                await _contentManager.CreateAsync(content, VersionOptions.Draft);
            }

            if (!group.Contains("Anonymous") && !await _authorizationService.AuthorizeAsync(User, Permissions.SubmitForm, content))
            {
                return Unauthorized();
            }

            var formContent = await _contentManager.GetAsync(id, VersionOptions.Latest);
            string adminContainer = formContent.Content.AdvancedForm.AdminContainer.Html != null ? JsonConvert.SerializeObject(formContent.Content.AdvancedForm.AdminContainer.Html) : String.Empty;
            description = formContent.Content.AdvancedForm.Description.Html;
            instructions = formContent.Content.AdvancedForm.Instructions.Html;
            header = formContent.Content.AdvancedForm.Header.Html;
            footer = formContent.Content.AdvancedForm.Footer.Html;
            hideFromListing = formContent.Content.AdvancedForm.HideFromListing.Value;
            isGlobalFooter = formContent.Content.AdvancedForm.IsGlobalFooter.Value;
            isGlobalHeader = formContent.Content.AdvancedForm.IsGlobalHeader.Value;

            string metadata = string.Empty, data, status = string.Empty;
            var query = _session.Query<ContentItem, ContentItemIndex>();
            if (isDraft)
            {
                data = submission;
                var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormStatus" && o.DisplayText == "Draft" && (o.Latest || o.Published)).ListAsync();
                foreach (var item in pageOfContentItems)
                {
                    status = item.ContentItemId;
                }
            }
            else
            {
                var subObject = JObject.Parse(submission);
                metadata = subObject["metadata"].ToString();
                data = subObject["data"].ToString();
                var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormStatus" && o.DisplayText == "Submitted" && (o.Latest || o.Published)).ListAsync();
                foreach (var item in pageOfContentItems)
                {
                    status = item.ContentItemId;
                }
            }

            if (string.IsNullOrWhiteSpace(owner) && !string.IsNullOrEmpty(User.Identity.Name))
            {
                owner = User.Identity.Name;
            }
            string guid = content.ContentItemId;
            string subTitle = title + " " + DateTime.Now.ToUniversalTime().ToString() + " " + guid;
            string Location = string.Empty;
            if (adminContainer != null)
            {
                ContentHelper helper = new ContentHelper(_contentManager, _session, _contentDefinitionManager, _contentAliasManager);
                Location = helper.GetInputValue(data, "applicationLocation");
                if (string.IsNullOrEmpty(adminSubmission))
                {
                    adminSubmission = "{\r\n  \"doNotMapLocation\": false\r\n}";
                }
            }
            var viewModel = new AdvancedFormSubmissions(data, metadata, subTitle, container, header, footer, description,
                type, instructions, owner, status, adminContainer, adminSubmission, Location, hideFromListing, isGlobalHeader, isGlobalFooter, group);

            return await EditPOST(content.ContentItemId, title, viewModel, async contentItem =>
            {
                if (!isDraft)
                {
                    await _contentManager.PublishAsync(contentItem);
                }

                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content has been published."]
                    : T["Your {0} has been published.", typeDefinition.DisplayName]);
            });
        }

        [HttpPost]
        [Route("AdvancedForms/admin/{alias}/Submission/{id}")]
        [FormValueRequired("submit.Save")]
        public async Task<IActionResult> SubmissionSave(AdvancedFormViewModel model, string returnUrl = "")
        {
            ContentItem content;
            if (!string.IsNullOrWhiteSpace(model.SubmissionId))
            {
                content = await _contentManager.GetAsync(model.SubmissionId, VersionOptions.Latest);
            }
            else
            {
                content = await _contentManager.NewAsync(_id);
                await _contentManager.CreateAsync(content, VersionOptions.Draft);
            }

            if (string.IsNullOrWhiteSpace(model.Owner))
            {
                model.Owner = User.Identity.Name;
            }

            string guid = content.ContentItemId;
            string subTitle = model.Title + " " + DateTime.Now.ToUniversalTime().ToString() + " " + guid;
            var subObject = JObject.Parse(model.Submission);
            var viewModel = new AdvancedFormSubmissions(model.Submission,
            model.Metadata, subTitle, model.Container, model.Header, model.Footer, model.Description, model.Type, model.Instructions, model.Owner, model.Status, model.AdminContainer, model.AdminSubmission, model.ApplicationLocation, model.HideFromListing, model.IsGlobalHeader, model.IsGlobalFooter, model.Group);

            await EditPOST(content.ContentItemId, model.Title, viewModel, async contentItem =>
            {
                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);
                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content has been published."]
                    : T["Your {0} has been published.", typeDefinition.DisplayName]);
            });

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/AdvancedForms/Admin/Submissions");
        }

        [HttpPost]
        [Route("AdvancedForms/admin/{alias}/Submission/{id}")]
        [FormValueRequired("submit.Publish")]
        public async Task<IActionResult> SubmissionPublish(AdvancedFormViewModel model, string returnUrl = "")
        {
            ContentItem content;
            if (!string.IsNullOrWhiteSpace(model.SubmissionId))
            {
                content = await _contentManager.GetAsync(model.SubmissionId, VersionOptions.Latest);
            }
            else
            {
                content = await _contentManager.NewAsync(_id);
                await _contentManager.CreateAsync(content, VersionOptions.Draft);
            }

            if (string.IsNullOrWhiteSpace(model.Owner))
            {
                model.Owner = User.Identity.Name;
            }

            string guid = content.ContentItemId;
            string subTitle = model.Title + " " + DateTime.Now.ToUniversalTime().ToString() + " " + guid;
            var subObject = JObject.Parse(model.Submission);
            var viewModel = new AdvancedFormSubmissions(model.Submission,
            model.Metadata, subTitle, model.Container, model.Header, model.Footer, model.Description, model.Type, model.Instructions, model.Owner, model.Status, model.AdminContainer, model.AdminSubmission, model.ApplicationLocation, model.HideFromListing, model.IsGlobalHeader, model.IsGlobalFooter, model.Group);

            await EditPOST(content.ContentItemId, model.Title, viewModel, async contentItem =>
            {
                await _contentManager.PublishAsync(contentItem);

                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content has been published."]
                    : T["Your {0} has been published.", typeDefinition.DisplayName]);
            });

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/AdvancedForms/Admin/Submissions");
        }

        private async Task<IActionResult> EditPOST(string contentItemId, string title, AdvancedFormSubmissions viewModel, Func<ContentItem, Task> conditionallyPublish)
        {

            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.DraftRequired);

            if (contentItem == null)
            {
                return NotFound();
            }

            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, contentItem))
            //{
            //    return Unauthorized();
            //}

            string guid = contentItem.ContentItemId;
            contentItem.Content.AdvancedFormSubmissions = JToken.FromObject(viewModel);
            contentItem.DisplayText = title;
            contentItem.Author = User.Identity.Name;
            contentItem.Owner = viewModel.Owner;
            contentItem.Content.AutoroutePart.Path = CreatePath(title, guid);

            await conditionallyPublish(contentItem);

            // The content item needs to be marked as saved (again) in case the drivers or the handlers have
            // executed some query which would flush the saved entities. In this case the changes happening in handlers 
            // would not be taken into account.
            _session.Save(contentItem);


            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);
            return Ok(contentItem);
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
            var subContentItem = await _contentManager.GetAsync(id, VersionOptions.Published);
            var viewName = entryType == EntryType.Print ? "Print" : "Display";

            if (subContentItem == null)
            {
                subContentItem = await _contentManager.GetAsync(id, VersionOptions.Draft);
            }

            if (subContentItem == null)
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
            var model = await new ContentHelper(_contentManager, _session, _contentDefinitionManager, _contentAliasManager).ReturnView(alias, id, entryType);
            var globalAFSetting = (await _siteService.GetSiteSettingsAsync()).As<AdvancedFormsSettings>();

            if (model.IsGlobalHeader)
            {
                model.Header = globalAFSetting.Header;
            }

            if (model.IsGlobalFooter)
            {
                model.Footer = globalAFSetting.Footer;
            }

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
