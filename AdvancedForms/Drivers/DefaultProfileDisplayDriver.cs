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
using UserProfile;
using YesSql;

namespace AdvancedForms.Drivers
{
    public class DefaultProfileDisplayDriver : DisplayDriver<IProfile>
    {
        public const string GroupId = "general";
        private readonly INotifier _notifier;
        private readonly IShellHost _shellHost;
        private readonly ShellSettings _shellSettings;
        private readonly ISession _session;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;

        public DefaultProfileDisplayDriver(
            INotifier notifier,
            IShellHost shellHost,
            ShellSettings shellSettings,
            ISession session,
            IContentItemDisplayManager contentItemDisplayManager,
            IHtmlLocalizer<DefaultProfileDisplayDriver> h)
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
            var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormSubmissions" && o.Latest).ListAsync();
            if (profile.UserName.ToLower() != "admin")
            {
                pageOfContentItems = pageOfContentItems.Where(o => o.Owner == profile.UserName);
            }
            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in pageOfContentItems)
            {
                contentItemSummaries.Add(await _contentItemDisplayManager.BuildDisplayAsync(contentItem, updater, "SummaryAdmin"));
            }

            return await Task.FromResult<IDisplayResult>(
                    Initialize<ProfileViewModel>("List_Edit", item =>
                    {
                        item.ContentItemSummaries = contentItemSummaries;
                    }).Location("Content:1").OnGroup(GroupId)
            );
        }
    }
}
