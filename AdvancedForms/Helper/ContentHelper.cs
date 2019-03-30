using AdvancedForms.Enums;
using AdvancedForms.Models;
using AdvancedForms.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.Helper
{
    public class ContentHelper
    {
        private readonly IContentManager _contentManager;
        private readonly YesSql.ISession _session;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentAliasManager _contentAliasManager;

        public ContentHelper(
            IContentManager contentManager, 
            YesSql.ISession session,
            IContentDefinitionManager contentDefinitionManager,
            IContentAliasManager contentAliasManager)
        {
            _contentManager = contentManager;
            _session = session;
            _contentDefinitionManager = contentDefinitionManager;
            _contentAliasManager = contentAliasManager;
        }

        public async Task<int> EditCommentPOST(string contentItemId, bool isPublic, string id, string User, CommentPart viewModel, Func<ContentItem, Task> conditionallyPublish)
        {
            var content = await _contentManager.GetAsync(contentItemId, VersionOptions.DraftRequired);

            if (content == null)
            {
                return StatusCodes.Status204NoContent;
            }
            if (isPublic)
                content.Content.PublicComment = JToken.FromObject(viewModel);
            else
                content.Content.AdminComment = JToken.FromObject(viewModel);
            content.Owner = User;
            content.DisplayText = id;

            await conditionallyPublish(content);

            _session.Save(content);

            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(content.ContentType);
            return StatusCodes.Status201Created;
        }

        public async Task<AdvancedFormViewModel> ReturnView(string alias, string id, EntryType entryType)
        {
            var contentItemId = await _contentAliasManager.GetContentItemIdAsync("slug:AdvancedForms/" + alias);
            var contentItem = await _contentManager.GetAsync(contentItemId, VersionOptions.Published);
            var subContentItem = await _contentManager.GetAsync(id, VersionOptions.Published);

            var selectedContent = await _contentManager.GetAsync(subContentItem.Content.AdvancedFormSubmissions.Status.Text.ToString(), VersionOptions.Published);

            if (selectedContent == null)
            {
                selectedContent = await _contentManager.GetAsync(subContentItem.Content.AdvancedFormSubmissions.Status.Text.ToString(), VersionOptions.DraftRequired);
            }
            string statusText = string.Empty;
            if (selectedContent != null)
            {
                statusText = selectedContent.DisplayText;
            }
            return new AdvancedFormViewModel
            {
                Id = id,
                Owner = subContentItem.Owner,
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
                AdminSubmission = subContentItem.Content.AdvancedFormSubmissions.AdminSubmission != null ? subContentItem.Content.AdvancedFormSubmissions.AdminSubmission.Html.ToString() : null,
                EntryType = entryType,
                Status = subContentItem.Content.AdvancedFormSubmissions.Status.Text,
                StatusText = statusText,
                PublicEditor = new HTMLFieldViewModel() { ID = "PublicComment" }

            };
        }

    }
}
