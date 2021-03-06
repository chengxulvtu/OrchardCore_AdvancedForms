﻿using OrchardCore.Entities;
using System.Collections.Generic;

namespace OrchardCore.Profile
{
    public class Profile : Entity, IProfile
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public List<string> UserRoles { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int? Page { get; set; }
    }
}
