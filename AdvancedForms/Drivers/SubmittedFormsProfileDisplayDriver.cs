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
        public const string GroupId = "submissions";

        public SubmittedFormsProfileDisplayDriver(IHtmlLocalizer<SubmittedFormsProfileDisplayDriver> h)
        {
            H = h;
        }

        IHtmlLocalizer H { get; set; }

        public async override Task<IDisplayResult> EditAsync(IProfile profile, IUpdateModel updater)
        {

            return await Task.FromResult<IDisplayResult>(
                    Initialize<ProfileViewModel>("Submission_List_Edit", item =>
                    {
                        item.UserRoles = profile.UserRoles;
                        item.UserName = profile.UserName;
                        item.Title = profile.Title;
                        item.Status = profile.Status;
                    }).Location("Content:1").OnGroup(GroupId)
            );
        }

    }
}
