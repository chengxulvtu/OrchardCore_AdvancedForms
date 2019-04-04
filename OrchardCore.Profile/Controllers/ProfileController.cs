using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Profile.ViewModels;
using YesSql;

namespace OrchardCore.Profile.Controllers
{
    [Authorize]
    public class ProfileController : Controller, IUpdateModel
    {
        private readonly IDisplayManager<IProfile> _profileDisplayManager;
        private readonly IProfileService _profileService;
        private readonly INotifier _notifier;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISession _session;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ProfileController(
            IProfileService profileService,
            IDisplayManager<IProfile> profileDisplayManager,
            IAuthorizationService authorizationService,
            INotifier notifier,
            ISession session,
            IHtmlLocalizer<ProfileController> h,
            IStringLocalizer<ProfileController> s,
            IContentDefinitionManager contentDefinitionManager
            )
        {
            _profileDisplayManager = profileDisplayManager;
            _profileService = profileService;
            _notifier = notifier;
            _authorizationService = authorizationService;
            H = h;
            S = s;
            _session = session;
            _contentDefinitionManager = contentDefinitionManager;
        }

        IHtmlLocalizer H { get; set; }
        IStringLocalizer S { get; set; }


        public async Task<IActionResult> Index(string groupId = "")
        {
            if (string.IsNullOrWhiteSpace(groupId))
            {
                groupId = "general";
            }

            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageGroupProfile, (object)groupId))
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetProfileAsync();
            if (profile.UserName != User.Identity.Name)
            {
                profile.UserName = User.Identity.Name;
                await _profileService.UpdateProfileAsync(profile);
            }

            var query = _session.Query<ContentItem, ContentItemIndex>();
            var pageOfContentItems = await query.Where(o => o.ContentType == "AdvancedFormSubmissions" && o.Latest).OrderByDescending(o => o.CreatedUtc).ListAsync();
            if (profile.UserName.ToLower() != "admin")
            {
                pageOfContentItems = pageOfContentItems.Where(o => o.Owner == profile.UserName);
            }
            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in pageOfContentItems)
            {
                contentItemSummaries.Add(contentItem);
            }

            var viewModel = new ProfileIndexViewModel
            {
                GroupId = groupId,
                Shape = await _profileDisplayManager.BuildEditorAsync(profile, this, false, groupId),
                ContentItemSummaries = contentItemSummaries
            };

            return View(viewModel);
        }

    }
}
