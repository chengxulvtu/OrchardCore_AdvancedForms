using Microsoft.AspNetCore.Routing;
using OrchardCore.Entities;
using System.Collections.Generic;

namespace OrchardCore.Profile
{
    public interface IProfile : IEntity
    {
        string UserName { get; set; }
        List<string> UserRoles { get; set; }
        string Title { get; set; }
        string Status { get; set; }
        int? Page { get; set; }
    }
}
