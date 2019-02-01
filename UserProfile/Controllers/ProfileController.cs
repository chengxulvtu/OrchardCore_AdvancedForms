﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Settings;
using UserProfile.ViewModels;
using YesSql;

namespace UserProfile.Controllers
{
    [Authorize]
    public class ProfileController : Controller, IUpdateModel
    {
        private readonly IDisplayManager<IProfile> _profileDisplayManager;
        private readonly IProfileService _profileService;
        private readonly INotifier _notifier;
        private readonly IAuthorizationService _authorizationService;

        public ProfileController(
            IProfileService profileService,
            IDisplayManager<IProfile> profileDisplayManager,
            IAuthorizationService authorizationService,
            INotifier notifier,
            IHtmlLocalizer<ProfileController> h,
            IStringLocalizer<ProfileController> s
            )
        {
            _profileDisplayManager = profileDisplayManager;
            _profileService = profileService;
            _notifier = notifier;
            _authorizationService = authorizationService;
            H = h;
            S = s;
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
            var viewModel = new ProfileIndexViewModel
            {
                GroupId = groupId,
                Shape = await _profileDisplayManager.BuildEditorAsync(profile, this, false, groupId)
            };

            return View(viewModel);
        }

    }
}
