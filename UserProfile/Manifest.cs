using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "User Profile Module Sample",
    Author = "Matt Kenney",
    Version = "0.1",
    Website = "https://github.com/themattkenney/OrchardCore_AdvancedForms"
)]

[assembly: Feature(
    Id = "UserProfile",
    Name = "UserProfile",
    Description = "Manage Advanced Forms User Profiles.",
    Dependencies = new[] { "OrchardCore.Contents, OrchardCore.Lists" },
    Category = "MRT Software"
)]


