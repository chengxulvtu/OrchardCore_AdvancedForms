using Microsoft.AspNetCore.Routing;
using OrchardCore.Entities;

namespace UserProfile
{
    public interface IProfile : IEntity
    {
        string UserName { get; set; }
    }
}
