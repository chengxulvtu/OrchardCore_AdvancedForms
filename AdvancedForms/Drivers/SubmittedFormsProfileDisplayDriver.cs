using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AdvancedForms.ViewModels;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Environment.Shell;
using OrchardCore.Profile;
using YesSql;
using OrchardCore.Profile.ViewModels;

namespace AdvancedForms.Drivers
{
    public class SubmittedFormsProfileDisplayDriver : DisplayDriver<IProfile>
    {
        public const string GroupId = "SubmittedForms";
        private readonly INotifier _notifier;
        private readonly IShellHost _shellHost;
        private readonly ShellSettings _shellSettings;
        private readonly ISession _session;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;

        public SubmittedFormsProfileDisplayDriver(
            INotifier notifier,
            IShellHost shellHost,
            ShellSettings shellSettings,
            ISession session,
            IContentItemDisplayManager contentItemDisplayManager,
            IHtmlLocalizer<SubmittedFormsProfileDisplayDriver> h)
        {
            _notifier = notifier;
            _shellHost = shellHost;
            _session = session;
            _shellSettings = shellSettings;
            _contentItemDisplayManager = contentItemDisplayManager;
            H = h;
        }

        IHtmlLocalizer H { get; set; }

        public async override Task<IDisplayResult> EditAsync(IProfile profile, IUpdateModel updater)
        {
            var query = _session.Query<ContentItem, ContentItemIndex>();
            var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormSubmissions" && o.Latest).OrderByDescending(o => o.CreatedUtc).ListAsync();
            if (!profile.UserRoles.Any(o=>o == "Administrator"))
            {
                pageOfContentItems = pageOfContentItems.Where(o => o.Owner == profile.UserName);
            }

            if(pageOfContentItems.Count() > 0 && !string.IsNullOrEmpty(profile.Title))
            {
                pageOfContentItems = pageOfContentItems.Where(o => o.DisplayText.ToLower().Contains(profile.Title.ToLower()));
            }

            if (pageOfContentItems.Count() > 0 && !string.IsNullOrEmpty(profile.Status) && profile.Status.ToLower() != "all")
            {
                pageOfContentItems = pageOfContentItems.Where(o => o.Content.AdvancedFormSubmissions.Status.Text == profile.Status);
            }

            query = _session.Query<ContentItem, ContentItemIndex>();
            var pageContentStatus = await query.Where(o => o.ContentType == "AdvancedFormStatus" && o.Latest).OrderByDescending(o => o.CreatedUtc).ListAsync();
            List<KeyValue> lstStatus = new List<KeyValue>();
            foreach (var item in pageContentStatus)
            {
                lstStatus.Add(new KeyValue() { Key = item.DisplayText, Value = item.ContentItemId });
            }

            return await Task.FromResult<IDisplayResult>(
                    Initialize<ProfileViewModel>("Submission_List_Edit", item =>
                    {
                        item.ContentItemSummaries = pageOfContentItems.ToList();
                        item.ListStatus = lstStatus;
                        item.Title = profile.Title;
                        item.Status = profile.Status;
                    }).Location("Content:1").OnGroup(GroupId)
            );
        }

    }
}
