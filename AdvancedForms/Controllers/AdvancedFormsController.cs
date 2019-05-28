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
using OrchardCore.Environment.Shell.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using OrchardCore.Media;
using OrchardCore.FileStorage;
using Microsoft.AspNetCore.StaticFiles;

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
        private readonly YesSql.ISession _session;
        private readonly IShellConfiguration _shellConfiguration;
        private readonly ILogger _logger;
        private readonly IMediaFileStore _mediaFileStore;
        private readonly IContentTypeProvider _contentTypeProvider;
        private const string _id = "AdvancedFormSubmissions";

        public AdvancedFormsController(
            IMediaFileStore mediaFileStore,
            IContentTypeProvider contentTypeProvider,
            IContentManager contentManager,
            IContentItemDisplayManager contentItemDisplayManager,
            IAuthorizationService authorizationService,
            IContentAliasManager contentAliasManager,
            IShellConfiguration shellConfiguration,
            INotifier notifier,
            YesSql.ISession session,
            ILogger<AdvancedFormsController> logger,
            IContentDefinitionManager contentDefinitionManager,
            IHtmlLocalizer<AdvancedFormsController> localizer
            )
        {
            _mediaFileStore = mediaFileStore;
            _contentTypeProvider = contentTypeProvider;
            _authorizationService = authorizationService;
            _contentItemDisplayManager = contentItemDisplayManager;
            _contentManager = contentManager;
            _notifier = notifier;
            _contentAliasManager = contentAliasManager;
            _session = session;
            _shellConfiguration = shellConfiguration;
            _logger = logger;
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
                Container = contentItem.Content.AdvancedForm.Container.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.Container.Html) : String.Empty,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                CaseID = "",
                HideFromListing = contentItem.Content.AdvancedForm.HideFromListing.Value,
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
                Container = contentItem.Content.AdvancedForm.Container.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.Container.Html) : String.Empty,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                CaseID = caseID,
                HideFromListing = contentItem.Content.AdvancedForm.HideFromListing.Value
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

        [HttpGet]
        [Route("AdvancedForms/GetPublicComments")]
        public async Task<IActionResult> GetPublicComments(string id)
        {
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var comments = await query.Where(o => o.ContentType == "PublicComment" && o.DisplayText == id && (o.Latest || o.Published)).ListAsync();
            return Ok(comments);
        }

        [HttpPost]
        [Route("AdvancedForms/Entry")]
        public async Task<IActionResult> Entry(string submission, string title, string id, string container,
            string header, string footer, string description, string type, string submissionId, string instructions, string owner, bool isDraft, bool hideFromListing)
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

            var formContent = await _contentManager.GetAsync(id, VersionOptions.Latest);
            string adminContainer = formContent.Content.AdvancedForm.AdminContainer.Html != null ? JsonConvert.SerializeObject(formContent.Content.AdvancedForm.AdminContainer.Html) : String.Empty;
            description = formContent.Content.AdvancedForm.Description.Html;
            instructions = formContent.Content.AdvancedForm.Instructions.Html;
            header = formContent.Content.AdvancedForm.Header.Html;
            footer = formContent.Content.AdvancedForm.Footer.Html;
            hideFromListing = formContent.Content.AdvancedForm.HideFromListing.Value;

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

            if (string.IsNullOrWhiteSpace(owner))
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
                type, instructions, owner, status, adminContainer, adminSubmission, Location, hideFromListing);

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
            model.Metadata, subTitle, model.Container, model.Header, model.Footer, model.Description, model.Type, model.Instructions, model.Owner, model.Status, model.AdminContainer, model.AdminSubmission, model.ApplicationLocation, model.HideFromListing);

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
            model.Metadata, subTitle, model.Container, model.Header, model.Footer, model.Description, model.Type, model.Instructions, model.Owner, model.Status, model.AdminContainer, model.AdminSubmission, model.ApplicationLocation, model.HideFromListing);

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

        #region "Manage Media Items"

        private static string[] DefaultAllowedFileExtensions = new string[] {
            // Images
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".ico",
            ".svg",

            // Documents
            ".pdf", // (Portable Document Format; Adobe Acrobat)
            ".doc", ".docx", // (Microsoft Word Document)
            ".ppt", ".pptx", ".pps", ".ppsx", // (Microsoft PowerPoint Presentation)
            ".odt", // (OpenDocument Text Document)
            ".xls", ".xlsx", // (Microsoft Excel Document)
            ".psd", // (Adobe Photoshop Document)

            // Audio
            ".mp3",
            ".m4a",
            ".ogg",
            ".wav",

            // Video
            ".mp4", ".m4v", // (MPEG-4)
            ".mov", // (QuickTime)
            ".wmv", // (Windows Media Video)
            ".avi",
            ".mpg",
            ".ogv", // (Ogg)
            ".3gp", // (3GPP)
        };

        [HttpPost]
        [Route("AdvancedForms/Admin/Upload")]
        public async Task<ActionResult<object>> Upload(
            string path,
            string contentType,
            ICollection<IFormFile> files)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnMedia))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(path))
            {
                path = "";
            }

            var section = _shellConfiguration.GetSection("OrchardCore.Media");

            // var maxUploadSize = section.GetValue("MaxRequestBodySize", 100_000_000);
            var maxFileSize = section.GetValue("MaxFileSize", 30_000_000);
            var allowedFileExtensions = section.GetSection("AllowedFileExtensions").Get<string[]>() ?? DefaultAllowedFileExtensions;

            var result = new List<object>();

            // Loop through each file in the request
            foreach (var file in files)
            {
                // TODO: support clipboard

                if (!allowedFileExtensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                {
                    result.Add(new
                    {
                        name = file.FileName,
                        size = file.Length,
                        folder = path,
                        error = T["This file extension is not allowed: {0}", Path.GetExtension(file.FileName)].ToString()
                    });

                    _logger.LogInformation("File extension not allowed: '{0}'", file.FileName);

                    continue;
                }

                if (file.Length > maxFileSize)
                {
                    result.Add(new
                    {
                        name = file.FileName,
                        size = file.Length,
                        folder = path,
                        error = T["The file {0} is too big. The limit is {1}MB", file.FileName, (int)Math.Floor((double)maxFileSize / 1024 / 1024)].ToString()
                    });

                    _logger.LogInformation("File too big: '{0}' ({1}B)", file.FileName, file.Length);

                    continue;
                }

                if (!allowedFileExtensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                {
                    result.Add(new
                    {
                        name = file.FileName,
                        size = file.Length,
                        folder = path,
                        error = T["This file extension is not allowed: {0}", Path.GetExtension(file.FileName)].ToString()
                    });

                    _logger.LogInformation("File extension not allowed: '{0}'", file.FileName);

                    continue;
                }

                if (file.Length > maxFileSize)
                {
                    result.Add(new
                    {
                        name = file.FileName,
                        size = file.Length,
                        folder = path,
                        error = T["The file {0} is too big. The limit is {1}MB", file.FileName, (int)Math.Floor((double)maxFileSize / 1024 / 1024)].ToString()
                    });

                    _logger.LogInformation("File too big: '{0}' ({1}B)", file.FileName, file.Length);

                    continue;
                }

                try
                {
                    var mediaFilePath = _mediaFileStore.Combine(path, file.FileName);

                    using (var stream = file.OpenReadStream())
                    {
                        await _mediaFileStore.CreateFileFromStream(mediaFilePath, stream);
                    }

                    var mediaFile = await _mediaFileStore.GetFileInfoAsync(mediaFilePath);

                    result.Add(CreateFileResult(mediaFile));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occured while uploading a media");

                    result.Add(new
                    {
                        name = file.FileName,
                        size = file.Length,
                        folder = path,
                        error = ex.Message
                    });
                }
            }

            return new { files = result.ToArray() };
        }

        [HttpPost]
        [Route("AdvancedForms/Admin/MoveMedia")]
        public async Task<IActionResult> MoveMedia(string oldPath, string newPath)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnMedia)
                || !await _authorizationService.AuthorizeAsync(User, Permissions.ManageAttachedMediaFieldsFolder, (object)oldPath))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath))
            {
                return NotFound();
            }

            if (await _mediaFileStore.GetFileInfoAsync(oldPath) == null)
            {
                return NotFound();
            }

            if (await _mediaFileStore.GetFileInfoAsync(newPath) != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, T["Cannot move media because a file already exists with the same name"]);
            }

            await _mediaFileStore.MoveFileAsync(oldPath, newPath);

            return Ok();
        }

        public object CreateFileResult(IFileStoreEntry mediaFile)
        {
            _contentTypeProvider.TryGetContentType(mediaFile.Name, out var contentType);

            return new
            {
                name = mediaFile.Name,
                size = mediaFile.Length,
                folder = mediaFile.DirectoryPath,
                url = _mediaFileStore.MapPathToPublicUrl(mediaFile.Path),
                mediaPath = mediaFile.Path,
                mime = contentType ?? "application/octet-stream"
            };
        }

        #endregion

    }
}
