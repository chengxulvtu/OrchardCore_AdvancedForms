using System.Collections.Generic;
using OrchardCore.Security.Permissions;

namespace AdvancedForms
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageAdvancedForms = new Permission("ManageAdvancedFormsContent", "Manage AdvancedForms");
        public static readonly Permission ManageOwnAdvancedForms = new Permission("ManageOwnAdvancedForms", "Manage Own AdvancedForms", new[] { ManageAdvancedForms });
        public static readonly Permission SubmitForm = new Permission("SubmitForm", "Submit Form Submission");
        public static readonly Permission ViewContent = new Permission("ViewContent", "View all content");
        public static readonly Permission ViewOwnContent = new Permission("ViewOwnContent", "View own content Sumbissions", new[] { ViewContent });

        public static readonly Permission ManageMedia = new Permission("ManageMediaContent", "Manage Media");
        public static readonly Permission ManageOwnMedia = new Permission("ManageOwnMedia", "Manage Own Media", new[] { ManageMedia });
        public static readonly Permission ManageAttachedMediaFieldsFolder = new Permission("ManageAttachedMediaFieldsFolder", "Manage Attached Media Fields Folder");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageAdvancedForms,
                ManageOwnAdvancedForms,
                ViewContent,
                ViewOwnContent,
                ManageMedia, ManageOwnMedia, ManageAttachedMediaFieldsFolder
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ManageAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] { ManageAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    Permissions = new[] { ManageAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] { ManageOwnAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] { ManageOwnAdvancedForms, SubmitForm }
                },
                 new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] { ViewContent }
                },
                new PermissionStereotype {
                    Name = "Anonymous",
                    Permissions = new[] { ViewContent }
                },
                new PermissionStereotype {
                    Name = "CITIZEN",
                    Permissions = new[] { ManageAdvancedForms, SubmitForm, ManageMedia, ManageOwnMedia, ManageAttachedMediaFieldsFolder }
                },
            };
        }
    }
}