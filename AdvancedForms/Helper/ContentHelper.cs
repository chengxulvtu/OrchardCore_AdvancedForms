using AdvancedForms.Enums;
using AdvancedForms.Models;
using AdvancedForms.ViewModels;
using Common.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

            if (subContentItem == null)
            {
                subContentItem = await _contentManager.GetAsync(id, VersionOptions.DraftRequired);
            }

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
                Id = contentItem.ContentItemId,
                Owner = subContentItem.Owner,
                Title = contentItem.DisplayText,
                Type = contentItem.Content.AdvancedForm.Type.Text,
                Group = contentItem.Content.AdvancedForm.Group.Text,
                Header = contentItem.Content.AdvancedForm.Header.Html,
                Footer = contentItem.Content.AdvancedForm.Footer.Html,
                Container = contentItem.Content.AdvancedForm.Container.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.Container.Html) : String.Empty,
                FormFields = contentItem.Content.AdvancedForm.FormFields != null && contentItem.Content.AdvancedForm.FormFields.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.FormFields.Html) : String.Empty,
                AdminContainer = contentItem.Content.AdvancedForm.AdminContainer.Html != null ? JsonConvert.SerializeObject(contentItem.Content.AdvancedForm.AdminContainer.Html) : String.Empty,
                Description = contentItem.Content.AdvancedForm.Description.Html,
                Instructions = contentItem.Content.AdvancedForm.Instructions.Html,
                SubmissionId = subContentItem.ContentItemId,
                Submission = subContentItem.Content.AdvancedFormSubmissions.Submission.Html != null ? JsonConvert.SerializeObject(subContentItem.Content.AdvancedFormSubmissions.Submission.Html) : String.Empty,
                AdminSubmission = subContentItem.Content.AdvancedFormSubmissions.AdminSubmission.Html != null ? JsonConvert.SerializeObject(subContentItem.Content.AdvancedFormSubmissions.AdminSubmission.Html) : String.Empty,
                EntryType = entryType,
                Status = subContentItem.Content.AdvancedFormSubmissions.Status.Text,
                StatusText = statusText,
                PublicEditor = new HTMLFieldViewModel() { ID = "PublicComment" },
                HideFromListing = contentItem.Content.AdvancedForm.HideFromListing.Value,
                IsGlobalHeader = contentItem.Content.AdvancedForm.IsGlobalHeader.Value,
                IsGlobalFooter = contentItem.Content.AdvancedForm.IsGlobalFooter.Value

            };
        }

        public bool IsMapLocationExist(string adminContainer)
        {
            bool isMapLocation = false;
            var adminfields = JObject.Parse(adminContainer);
            if (adminfields["components"] != null && adminfields["components"][0] != null && adminfields["components"][0]["components"] != null)
            {
                string value = this.GetValueFromJObject("doNotMapLocation", adminfields["components"][0]["components"]);
                if (!string.IsNullOrEmpty(value))
                {
                    return true;
                }
            }
            return isMapLocation;
        }

        public string GetInputValue(string data, string Input)
        {
            string value = string.Empty;
            Dictionary<string, object> contents = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            if (contents.Any(o => o.Key == Input))
            {
                return contents.FirstOrDefault(o => o.Key == Input).Value.ToString();
            }
            return value;
        }

        public string GetValueFromJObject(string Key, JToken obj)
        {
            string value = string.Empty;
            if (obj.ToString().Contains("{") && obj.ToString().Contains("}"))
            {
                value = string.Empty;
                foreach (var prop in obj.Children())
                {
                    if (prop["key"].ToString() == Key)
                    {
                        value = prop["defaultValue"].ToString();
                        break;
                    }
                }
            }
            return value;
        }

    }
}
