using System.Threading.Tasks;
using AdvancedForms.Models;
using Microsoft.AspNetCore.Routing;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;

namespace AdvancedForms.Handlers
{
    public class ListPartHandler : ContentPartHandler<ListPart>
    {
        public override Task GetContentItemAspectAsync(ContentItemAspectContext context, ListPart part)
        {
            context.For<ContentItemMetadata>(contentItemMetadata =>
            {
                contentItemMetadata.AdminRouteValues = new RouteValueDictionary
                {
                    {"Area", "AdvancedForms"},
                    {"Controller", "Admin"},
                    {"Action", "Edit"},
                    {"contentItemId", context.ContentItem.ContentItemId}
                };
            });

            return Task.CompletedTask;
        }
    }
}
