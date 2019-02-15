using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Navigation;
using System.Text;
using Newtonsoft.Json;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentManagement;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace AdvancedForms.Helper
{
    public class CsvEtl
    {
        private readonly IContentManager _contentManager;

        public CsvEtl(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public async Task<string> GetAdvFormSubmissionsCSVstring(IEnumerable<ContentItem> pageOfContentItems)
        {
            dynamic selectedContent;
            Dictionary<string, dynamic> submissionHtml;
            List<string> column = new List<string>();
            column.Add("Form Name");
            column.Add("Submitted Date");
            column.Add("Created By");
            column.Add("Status");
            StringBuilder csv = new StringBuilder();
            dynamic dynamicObj;
            string value;
            foreach (var contentItem in pageOfContentItems)
            {
                csv.Append(string.Format("{0},", contentItem.Content.AutoroutePart.Path.ToString().Split("/")[1].Replace("-", " ")));
                csv.Append(string.Format("{0},", contentItem.CreatedUtc.Value));
                csv.Append(string.Format("{0},", contentItem.Owner));
                selectedContent = await _contentManager.GetAsync(contentItem.Content.AdvancedFormSubmissions.Status.Text.ToString(), VersionOptions.Published);
                if (selectedContent == null)
                {
                    selectedContent = await _contentManager.GetAsync(contentItem.Content.AdvancedFormSubmissions.Status.Text.ToString(), VersionOptions.DraftRequired);
                }
                csv.Append(string.Format("{0},", selectedContent != null && selectedContent.DisplayText != null ? selectedContent.DisplayText : string.Empty));
                submissionHtml = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(contentItem.Content.AdvancedFormSubmissions.Submission.Html.ToString());
                foreach (var entry in submissionHtml)
                {
                    if (!column.Contains(entry.Key))
                    {
                        column.Add(entry.Key);
                    }
                }
                for (int i = 4; i < column.Count; i++)
                {
                    dynamicObj = null;
                    value = string.Empty;
                    submissionHtml.TryGetValue(column[i], out dynamicObj);
                    TryGetString(dynamicObj, out value);
                    csv.Append(string.Format("{0},", value == null ? string.Empty : value.ToString().Replace(",", "").Replace("\r", "").Replace("\n", "")));
                }
                csv.AppendLine();
            }
            StringBuilder file = new StringBuilder(string.Join(",", column));
            file.AppendLine();
            file.Append(csv.ToString());
            return file.ToString();
        }

        public bool TryGetString(dynamic obj, out string value)
        {
            if (obj == null)
            {
                value = string.Empty;
            }
            else if (obj.ToString().Contains("{") && obj.ToString().Contains("}"))
            {
                value = string.Empty;
                foreach (JProperty prop in obj.Children())
                {
                    if (prop.Value != null && prop.Value.ToString() == "True")
                    {
                        value +=  prop.Path + "; ";
                    }
                }
            }
            else
            {
                value = obj.ToString();
            }
            return true;
        }
    }
}
