using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Environment.Cache;

namespace AdvancedForms.Handlers
{
    public class ContentsHandler : ContentHandlerBase
    {
        private readonly ITagCache _tagCache;

        public ContentsHandler(ITagCache tagCache)
        {
            _tagCache = tagCache;
        }

        public override Task PublishedAsync(PublishContentContext context)
        {
            return _tagCache.RemoveTagAsync($"contentitemid:{context.ContentItem.ContentItemId}");
        }

        public override Task RemovedAsync(RemoveContentContext context)
        {
            return _tagCache.RemoveTagAsync($"contentitemid:{context.ContentItem.ContentItemId}");
        }

        public override Task UnpublishedAsync(PublishContentContext context)
        {
            return _tagCache.RemoveTagAsync($"contentitemid:{context.ContentItem.ContentItemId}");
        }

        public override Task GetContentItemAspectAsync(ContentItemAspectContext context)
        {
            return context.ForAsync<ContentItemMetadata>(metadata =>
            {
                if (context.ContentItem.ContentType == "AdvancedForm")
                {
                    metadata.EditorRouteValues = new RouteValueDictionary {
                            {"Area", "AdvancedForms"},
                            {"Controller", "Admin"},
                            {"Action", "Edit"},
                            {"ContentItemId", context.ContentItem.ContentItemId}
                        };
                }

                if (context.ContentItem.ContentType == "AdvancedFormSubmissions")
                {
                    try
                    {
                        string path = context.ContentItem.Content.AutoroutePart.Path.ToString().Split("/")[1];
                        metadata.EditorRouteValues = new RouteValueDictionary {
                            {"Area", "AdvancedForms"},
                            {"Controller", path},
                            {"Action", "Submission"},
                            {"id", context.ContentItem.ContentItemId}
                        };
                    }
                    catch
                    {
                    }
                }

                if (context.ContentItem.ContentType == "AdvancedForm")
                {
                    metadata.AdminRouteValues = new RouteValueDictionary {
                        {"Area", "AdvancedForms"},
                        {"Controller", "Admin"},
                        {"Action", "Edit"},
                        {"ContentItemId", context.ContentItem.ContentItemId}
                    };
                }

                if (context.ContentItem.ContentType == "AdvancedFormSubmissions")
                {
                    try
                    {
                        string path = context.ContentItem.Content.AutoroutePart.Path.ToString().Split("/")[1];
                        metadata.AdminRouteValues = new RouteValueDictionary {
                            {"Area", "AdvancedForms"},
                            {"Controller", path},
                            {"Action", "Submission"},
                            {"id", context.ContentItem.ContentItemId}
                        };
                    }
                    catch
                    {
                    }
                }
                return Task.CompletedTask;
            });
        }
    }
}