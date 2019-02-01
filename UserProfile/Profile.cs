using OrchardCore.Entities;

namespace UserProfile
{
    public class Profile : Entity, IProfile
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
