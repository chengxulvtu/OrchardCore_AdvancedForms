using System;
using Microsoft.AspNetCore.Builder;
using OrchardCore.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Security.Permissions;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.Scripting;
using OrchardCore.Recipes;
using UserProfile.ViewModels;

namespace UserProfile
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
            services.AddScoped<IPermissionProvider, Permissions>();
        }

        public override void Configure(IApplicationBuilder builder, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            
        }
    }
}
