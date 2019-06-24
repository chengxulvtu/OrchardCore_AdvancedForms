using System;
using Microsoft.AspNetCore.Builder;
using OrchardCore.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using OrchardCore.Security.Permissions;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Navigation;
using AdvancedForms.Handlers;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Profile;
using AdvancedForms.Drivers;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement;
using OrchardCore.Profile.Navigation;
using OrchardCore.Settings;

namespace AdvancedForms
{
    public class Startup : StartupBase
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<IPermissionProvider, Permissions>();

            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IProfileNavigationProvider, ProfileMenu>();
            services.AddScoped<IContentHandler, ContentsHandler>();
            services.AddScoped<IDisplayDriver<ISite>, AdvancedFormsSettingsDisplayDriver>();
            services.AddScoped<IDisplayManager<IProfile>, DisplayManager<IProfile>>();
            services.AddScoped<IDisplayDriver<IProfile>, DefaultProfileDisplayDriver>();
        }

        public override void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaRoute(
                name: "DisplayAdvancedForm",
                areaName: "AdvancedForms",
                template: "AdvancedForms/{alias}",
                defaults: new { controller = "AdvancedForms", action = "Display" }
            );

            routes.MapAreaRoute(
                name: "DisplayAdvancedFormCaseID",
                areaName: "AdvancedForms",
                template: "AdvancedForms/{alias}/Case/{caseID}",
                defaults: new { controller = "AdvancedForms", action = "Display" }
            );

            routes.MapAreaRoute(
                name: "EditSubmissionAdvancedForm",
                areaName: "AdvancedForms",
                template: "AdvancedForms/{alias}/Submission/{id}",
                defaults: new { controller = "Admin", action = "Submission" }
            );

            routes.MapAreaRoute(
                name: "EditSubmissionAdminAdvancedForm",
                areaName: "AdvancedForms",
                template: "AdvancedForms/admin/{alias}/Submission/{id}",
                defaults: new { controller = "Admin", action = "Submission" }
            );
        }
    }
}
