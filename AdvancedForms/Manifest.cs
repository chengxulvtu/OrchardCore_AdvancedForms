using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Advanced Forms Module",
    Author = "Matt Kenney",
    Version = "0.1",
    Website = "https://github.com/themattkenney/OrchardCore_AdvancedForms"
)]

[assembly: Feature(
    Id = "AdvancedForms",
    Name = "AdvancedForms",
    Description = "Creates forms content types, uses Forms.IO open source MIT open source tools. NOTE: Turn on Query Module before turning on this module.",
    Dependencies = new[] { "OrchardCore.Contents, OrchardCore.Lists" },
    Category = "MRT Software"
)]


