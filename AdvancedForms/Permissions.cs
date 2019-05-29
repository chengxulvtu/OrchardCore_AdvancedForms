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

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageAdvancedForms,
                ManageOwnAdvancedForms,
                SubmitForm,
                ViewContent,
                ViewOwnContent
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { ManageAdvancedForms, ManageOwnAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Editor",
                    Permissions = new[] { ManageAdvancedForms, ManageOwnAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Moderator",
                    Permissions = new[] { ManageAdvancedForms, ManageOwnAdvancedForms, SubmitForm }
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] { ViewOwnContent }
                },
                new PermissionStereotype {
                    Name = "Contributor",
                    Permissions = new[] { ViewOwnContent }
                },
                 new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] { ViewOwnContent }
                },
                new PermissionStereotype {
                    Name = "Anonymous",
                    Permissions = new[] { ViewOwnContent }
                }
            };
        }
    }
}