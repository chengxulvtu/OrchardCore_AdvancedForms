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
using UserProfile;
using AdvancedForms.Service;
using AdvancedForms.Drivers;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement;

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
            services.AddScoped<IContentHandler, ContentsHandler>();

            services.AddSingleton<IProfileService, ProfileService>();
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
                name: "EndUserProfileWithGroupId",
                areaName: "UserProfile",
                template: "Profile/{groupId}",
                defaults: new { controller = "Profile", action = "Index" }
            );

            routes.MapAreaRoute(
                name: "EndUserProfile",
                areaName: "UserProfile",
                template: "Profile",
                defaults: new { controller = "Profile", action = "Index" }
            );
        }
    }
}
