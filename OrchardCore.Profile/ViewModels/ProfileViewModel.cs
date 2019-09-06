using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement;
using System.Collections.Generic;

namespace OrchardCore.Profile.ViewModels
{
    public class ProfileViewModel
    {
        public List<ContentItem> ContentItemSummaries { get; set; }
        public List<KeyValue> ListStatus { get; set; }
        public string Title { get; set; }
        public string UserName {get; set;}
        public List<string> UserRoles { get; set; }
        public string Status { get; set; }
        public IProfile iProfile { get; set; }
        [BindNever]
        public dynamic Pager { get; set; }
        public int? Page { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
