using System.Collections.Generic;

namespace OrchardCore.Profile.ViewModels
{
    public class ProfileViewModel
    {
        public List<dynamic> ContentItemSummaries { get; set; }
        public List<KeyValue> ListStatus { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
