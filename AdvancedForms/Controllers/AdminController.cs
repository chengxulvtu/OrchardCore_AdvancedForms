using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Settings;
using YesSql;
using OrchardCore.Admin;
using AdvancedForms.ViewModels;
using AdvancedForms.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using OrchardCore.ContentManagement.Records;
using AdvancedForms.Helper;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AdvancedForms.Controllers
{
    [Admin]
    public class AdminController : Controller, IUpdateModel
    {
        private const string _id = "AdvancedForm";
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISiteService _siteService;
        private readonly YesSql.ISession _session;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly INotifier _notifier;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IContentAliasManager _contentAliasManager;

        public AdminController(
            IContentManager contentManager,
            IContentItemDisplayManager contentItemDisplayManager,
            IContentDefinitionManager contentDefinitionManager,
            ISiteService siteService,
            INotifier notifier,
            YesSql.ISession session,
            IShapeFactory shapeFactory,
            ILogger<AdminController> logger,
            IHtmlLocalizer<AdminController> localizer,
            IAuthorizationService authorizationService,
            IContentAliasManager contentAliasManager
            )
        {
            _authorizationService = authorizationService;
            _notifier = notifier;
            _contentItemDisplayManager = contentItemDisplayManager;
            _session = session;
            _siteService = siteService;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            _logger = logger;
            T = localizer;
            _contentAliasManager = contentAliasManager;
        }

        public IHtmlLocalizer T { get; }

        public dynamic New { get; set; }

        #region "   Create/Edit Form    "

        public async Task<IActionResult> Adminfields(string contentItemId)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, contentItem))
            {
                return Unauthorized();
            }

            var model = new AdvancedFormViewModel
            {
                Id = contentItemId,
                EntryType = Enums.EntryType.Edit,
                Title = contentItem.DisplayText,
                Container = contentItem.Content.AdvancedForm.Container.Html,
                HtmlContainer = contentItem.Content.AdvancedForm.HtmlContainer.Html,
                AdminContainer = contentItem.Content.AdvancedForm.AdminContainer.Html,
                AdminHtmlContainer = contentItem.Content.AdvancedForm.AdminHtmlContainer.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                Type = contentItem.Content.AdvancedForm.Type.Text,
            };

            return View(model);
        }

        [HttpPost, ActionName("Adminfields")]
        [FormValueRequired("submit.Save")]
        public Task<IActionResult> AdminfieldsPOST(AdvancedFormViewModel viewModel, string returnUrl)
        {
            return EditPOST(viewModel, returnUrl, contentItem =>
            {
                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content draft has been saved."]
                    : T["Your {0} Admin fields draft has been saved.", typeDefinition.DisplayName]);

                return Task.CompletedTask;
            });
        }

        [HttpPost, ActionName("Adminfields")]
        [FormValueRequired("submit.Publish")]
        public async Task<IActionResult> AdminfieldsAndPublishPOST(AdvancedFormViewModel viewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            string contentItemId = viewModel.Id;

            var content = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (content == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, content))
            {
                return Unauthorized();
            }

            return await EditPOST(viewModel, returnUrl, async contentItem =>
            {
                await _contentManager.PublishAsync(contentItem);

                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content has been published."]
                    : T["Your {0} Admin fields has been published.", typeDefinition.DisplayName]);
            });
        }

        public async Task<IActionResult> Create()
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms))
            {
                return Unauthorized();
            }

            return View(new AdvancedFormViewModel());
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Publish")]
        public async Task<IActionResult> CreateAndPublishPOST(AdvancedFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }


            // pass a dummy content to the authorization check to check for "own" variations
            var dummyContent = await _contentManager.NewAsync(_id);

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, dummyContent))
            {
                return Unauthorized();
            }

            return await CreatePOST(viewModel, async contentItem =>
            {
                await _contentManager.PublishAsync(contentItem);

                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(T["Your Adavanced Form has been published."]);
            });
        }

        [HttpPost, ActionName("Create")]
        [FormValueRequired("submit.Save")]
        public async Task<IActionResult> CreateAndSavePOST(AdvancedFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }


            // pass a dummy content to the authorization check to check for "own" variations
            var dummyContent = await _contentManager.NewAsync(_id);

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, dummyContent))
            {
                return Unauthorized();
            }

            return await CreatePOST(viewModel, async contentItem =>
            {
                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);
                _notifier.Success(T["Your Adavanced Form has been saved on draft."]);
            });
        }

        private async Task<IActionResult> CreatePOST(AdvancedFormViewModel viewModel, Func<ContentItem, Task> conditionallyPublish)
        {
            var contentItem = await _contentManager.NewAsync(_id);

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, contentItem))
            {
                return Unauthorized();
            }

            var advForm = new AdvancedForm(viewModel.Description, viewModel.Instructions,
                viewModel.Container, viewModel.Title, viewModel.Header, viewModel.Footer, viewModel.Type, viewModel.AdminContainer, viewModel.HtmlContainer, viewModel.AdminHtmlContainer);
            contentItem.Content.AdvancedForm = JToken.FromObject(advForm);
            contentItem.DisplayText = viewModel.Title;
            contentItem.Content.AutoroutePart.Path = CreatePath(viewModel.Title);
            if (!ModelState.IsValid)
            {
                _session.Cancel();
                return View(viewModel);
            }

            await _contentManager.CreateAsync(contentItem, VersionOptions.Draft);

            await conditionallyPublish(contentItem);

            return RedirectToAction("Edit", new RouteValueDictionary { { "ContentItemId", contentItem.ContentItemId } });
        }

        [HttpGet]
        [Route("AdvancedForms/GetAdminComments")]
        public async Task<IActionResult> GetAdminComments(string id)
        {
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var comments = await query.Where(o => o.ContentType == "AdminComment" && o.DisplayText == id && (o.Latest || o.Published)).ListAsync();
            return Ok(comments);
        }


        public async Task<IActionResult> Edit(string contentItemId, string returnUrl)
        {
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (contentItem == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, contentItem))
            {
                return Unauthorized();
            }

            var selectedContent = await _contentManager.GetAsync(contentItem.Content.AdvancedForm.Type.Text.ToString(), VersionOptions.Published);

            if (selectedContent == null)
            {
                selectedContent = await _contentManager.GetAsync(contentItem.Content.AdvancedForm.Type.Text.ToString(), VersionOptions.DraftRequired);
            }

            List<ContentPickerItemViewModel> lst = new List<ContentPickerItemViewModel>();
            if (selectedContent != null)
            {
                ContentPickerItemViewModel contentPick = new ContentPickerItemViewModel();
                contentPick.ContentItemId = selectedContent.ContentItemId;
                contentPick.DisplayText = selectedContent.DisplayText;
                contentPick.HasPublished = selectedContent.Published;
                lst.Add(contentPick);
            }
            var model = new AdvancedFormViewModel
            {
                Id = contentItemId,
                EntryType = Enums.EntryType.Edit,
                Title = contentItem.DisplayText,
                Container = contentItem.Content.AdvancedForm.Container.Html,
                HtmlContainer = contentItem.Content.AdvancedForm.HtmlContainer.Html,
                AdminContainer = contentItem.Content.AdvancedForm.AdminContainer.Html,
                AdminHtmlContainer = contentItem.Content.AdvancedForm.AdminHtmlContainer.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                Type = contentItem.Content.AdvancedForm.Type.Text,
                SelectedItems = lst

            };

            model.ReturnUrl = returnUrl;

            return View("Create", model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Save")]
        public Task<IActionResult> EditPOST(AdvancedFormViewModel viewModel, string returnUrl)
        {
            return EditPOST(viewModel, returnUrl, contentItem =>
            {
                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content draft has been saved."]
                    : T["Your {0} draft has been saved.", typeDefinition.DisplayName]);

                return Task.CompletedTask;
            }, true);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Publish")]
        public async Task<IActionResult> EditAndPublishPOST(AdvancedFormViewModel viewModel, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            string contentItemId = viewModel.Id;

            var content = await _contentManager.GetAsync(contentItemId, VersionOptions.Latest);

            if (content == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, content))
            {
                return Unauthorized();
            }

            return await EditPOST(viewModel, returnUrl, async contentItem =>
            {
                await _contentManager.PublishAsync(contentItem);

                var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

                _notifier.Success(string.IsNullOrWhiteSpace(typeDefinition.DisplayName)
                    ? T["Your content has been published."]
                    : T["Your {0} has been published.", typeDefinition.DisplayName]);
            }, true);
        }

        private async Task<IActionResult> EditPOST(AdvancedFormViewModel viewModel, string returnUrl, Func<ContentItem, Task> conditionallyPublish, bool isEditPage = false)
        {
            string contentItemId = viewModel.Id;

            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.DraftRequired);

            if (contentItem == null)
            {
                return NotFound();
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms, contentItem))
            {
                return Unauthorized();
            }

            var advForm = new AdvancedForm(viewModel.Description, viewModel.Instructions,
                viewModel.Container, viewModel.Title, viewModel.Header, viewModel.Footer, viewModel.Type, viewModel.AdminContainer, viewModel.HtmlContainer, viewModel.AdminHtmlContainer);
            contentItem.Content.AdvancedForm = JToken.FromObject(advForm);
            contentItem.DisplayText = viewModel.Title;
            contentItem.Content.AutoroutePart.Path = CreatePath(viewModel.Title);

            var model = new AdvancedFormViewModel
            {
                Id = viewModel.Id,
                Title = contentItem.DisplayText,
                Container = contentItem.Content.AdvancedForm.Container.Html,
                HtmlContainer = contentItem.Content.AdvancedForm.HtmlContainer.Html,
                AdminContainer = contentItem.Content.AdvancedForm.AdminContainer.Html,
                AdminHtmlContainer = contentItem.Content.AdvancedForm.AdminHtmlContainer.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                Type = contentItem.Content.AdvancedForm.Type.Text
            };

            if (!ModelState.IsValid)
            {
                _session.Cancel();
                if(isEditPage)
                    return View("Create", model);
                else
                    return View("Adminfields", model);
            }

            await conditionallyPublish(contentItem);

            // The content item needs to be marked as saved (again) in case the drivers or the handlers have
            // executed some query which would flush the saved entities. In this case the changes happening in handlers 
            // would not be taken into account.
            _session.Save(contentItem);

            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(contentItem.ContentType);

            if (returnUrl == null)
            {
                if (isEditPage)
                    return RedirectToAction("Edit", new RouteValueDictionary { { "ContentItemId", viewModel.Id } });
                else
                    return RedirectToAction("Adminfields", new RouteValueDictionary { { "ContentItemId", viewModel.Id } });
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
        }

        #endregion

        #region "   Admin Form Submission Screen    "

        [Route("AdvancedForms/admin/{alias}/Submission/{id}")]
        public async Task<IActionResult> Submission(string alias, string id)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageAdvancedForms))
            {
                return Unauthorized();
            }

            if (String.IsNullOrWhiteSpace(alias))
            {
                return Redirect("/");
            }
            else if (String.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var contentItemId = await _contentAliasManager.GetContentItemIdAsync("slug:AdvancedForms/" + alias);
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Published);
            var subContentItem = await _contentManager.GetAsync(id, VersionOptions.Published);

            if (subContentItem == null)
            {
                subContentItem = await _contentManager.GetAsync(id, VersionOptions.Draft);
            }

            if (subContentItem == null)
            {
                return NotFound();
            }

            var selectedContent = await _contentManager.GetAsync(subContentItem.Content.AdvancedFormSubmissions.Status.Text.ToString(), VersionOptions.Published);

            if (selectedContent == null)
            {
                selectedContent = await _contentManager.GetAsync(subContentItem.Content.AdvancedFormSubmissions.Status.Text.ToString(), VersionOptions.DraftRequired);
            }

            List<ContentPickerItemViewModel> lst = new List<ContentPickerItemViewModel>();
            if (selectedContent != null)
            {
                ContentPickerItemViewModel contentPick = new ContentPickerItemViewModel();
                contentPick.ContentItemId = selectedContent.ContentItemId;
                contentPick.DisplayText = selectedContent.DisplayText;
                contentPick.HasPublished = selectedContent.Published;
                lst.Add(contentPick);
            }
            var model = new AdvancedFormViewModel
            {
                Id = contentItem.ContentItemId,
                Owner = contentItem.Owner,
                ModifiedUtc = subContentItem.ModifiedUtc,
                CreatedUtc = subContentItem.CreatedUtc,
                Title = contentItem.DisplayText,
                Type = contentItem.Content.AdvancedForm.Type.Text,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                Container = contentItem.Content.AdvancedForm.Container.Html,
                HtmlContainer = contentItem.Content.AdvancedForm.HtmlContainer.Html,
                AdminContainer = contentItem.Content.AdvancedForm.AdminContainer.Html,
                AdminHtmlContainer = contentItem.Content.AdvancedForm.AdminHtmlContainer.Html,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                SubmissionId = subContentItem.ContentItemId,
                Submission = subContentItem.Content.AdvancedFormSubmissions.Submission.Html,
                AdminSubmission = subContentItem.Content.AdvancedFormSubmissions.AdminSubmission.Html,
                Metadata = subContentItem.Content.AdvancedFormSubmissions.Metadata.Html,
                Status = subContentItem.Content.AdvancedFormSubmissions.Status.Text,
                SelectedItems = lst,
                AdminEditor = new HTMLFieldViewModel() { ID = "AdminComment" },
                PublicEditor = new HTMLFieldViewModel() { ID = "PublicComment" }
            };
            return View("Submission", model);
        }

        public async Task<IActionResult> Submissions()
        {
            SubmissionsViewModel model = new SubmissionsViewModel();
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormSubmissions" && (o.Latest || o.Published)).OrderByDescending(o => o.CreatedUtc).ListAsync();
            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in pageOfContentItems)
            {
                contentItemSummaries.Add(await _contentItemDisplayManager.BuildDisplayAsync(contentItem, this, "SubmissionAdmin_ListItem"));
            }
            model.ContentItemSummaries = contentItemSummaries;
            return View(model);
        }

        [HttpGet]
        [Route("AdvancedForms/GetGraphData")]
        public async Task<IActionResult> GetGraphData()
        {
            Graph graph = new Graph();
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var allStatus = await query.Where(o => o.ContentType == "AdvancedFormStatus" && (o.Latest || o.Published)).ListAsync();
            //dynamic FreqStatus = new System.Dynamic.ExpandoObject();
            Dictionary<string, string> statusWithColor = new Dictionary<string, string>();
            query = _session.Query<ContentItem, ContentItemIndex>();
            var allTypes = await query.Where(o => o.ContentType == "AdvancedFormTypes" && (o.Latest || o.Published)).ListAsync();
            graph.FreqData = new List<FrequencyData>();
            FrequencyData data;
            query = _session.Query<ContentItem, ContentItemIndex>();
            var allSubmissions = await query.Where(o => o.ContentType == "AdvancedFormSubmissions" && o.Latest).ListAsync();
            int formCount = 0;
            foreach (var type in allTypes)
            {
                data = new FrequencyData();
                data.State = type.DisplayText;
                data.freq = new Dictionary<string, int>();
                foreach (var status in allStatus)
                {
                    statusWithColor[status.DisplayText.Replace(" ", "_")] = status.Content.AdvancedFormStatus.Color.Text;
                    formCount = allSubmissions.Where(o => o.Content.AdvancedFormSubmissions.Status.Text.ToString() == status.ContentItemId
                    && o.Content.AdvancedFormSubmissions.Type.Text.ToString() == type.ContentItemId).Count();
                    data.freq.Add(status.DisplayText.Replace(" ", "_"), formCount);
                }
                graph.FreqData.Add(data);
            }
            graph.FreqStatusColor = statusWithColor;
            return Ok(graph);
        }

        [HttpPost, ActionName("Submissions")]
        [FormValueRequired("submit.Filter")]
        public async Task<IActionResult> SubmissionsFilter(string DisplayText = "")
        {
            DisplayText = string.IsNullOrEmpty(DisplayText) ? "" : DisplayText;
            SubmissionsViewModel model = new SubmissionsViewModel();
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var pageOfContentItems = await query.Where(o => o.DisplayText.Contains(DisplayText) && o.ContentType == "AdvancedFormSubmissions" && (o.Latest || o.Published)).OrderByDescending(o => o.CreatedUtc).ListAsync();
            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in pageOfContentItems)
            {
                contentItemSummaries.Add(await _contentItemDisplayManager.BuildDisplayAsync(contentItem, this, "SubmissionAdmin_ListItem"));
            }
            model.ContentItemSummaries = contentItemSummaries;
            model.DisplayText = DisplayText;
            return View(model);
        }

        [HttpPost, ActionName("Submissions")]
        [FormValueRequired("submit.Export")]
        public async Task<FileContentResult> SubmissionsExport(string checkedItems)
        {
            if (string.IsNullOrEmpty(checkedItems))
                return null;
            int[] selectedItems = checkedItems.Split(',').Select(int.Parse).ToArray();
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormSubmissions" && (o.Latest || o.Published)).OrderByDescending(o => o.CreatedUtc).ListAsync();
            pageOfContentItems = pageOfContentItems.Where(o => selectedItems.Contains(o.Id)).ToList();
            string file = await new CsvEtl(_contentManager).GetAdvFormSubmissionsCSVstring(pageOfContentItems);
            return File(new System.Text.UTF8Encoding().GetBytes(file), "text/csv", "Submissions.csv");
        }
        #endregion


        private string CreatePath(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                title = "AdvancedForms" + "/" + title.Replace(" ", "-");
            }
            return title;
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
