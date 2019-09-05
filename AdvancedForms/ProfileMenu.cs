using Microsoft.Extensions.Localization;
using OrchardCore.Profile.Navigation;
using System;
using System.Threading.Tasks;


namespace AdvancedForms
{
    public class ProfileMenu : IProfileNavigationProvider
    {
        public ProfileMenu(IStringLocalizer<ProfileMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }



        public Task BuildNavigation(string name, ProfileNavigationBuilder builder)
        {
            if (!String.Equals(name, "profile", StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }
            builder
                .Add(T["Submitted Forms"], "2", advancedForms => advancedForms
                    .Url("Profile/submissions")
                    .AddClass("list-group-item list-group-item-action"))
                .Add(T["Advanced Forms"], "4", advancedForms => advancedForms
                    .Url("advancedForms")
                    .AddClass("list-group-item list-group-item-action"))
                .Add(T["Downloadable Forms"], "5", advancedForms => advancedForms
                    .Url("downloadableForms")
                    .AddClass("list-group-item list-group-item-action"));
            return Task.CompletedTask;
        }
    }
}
