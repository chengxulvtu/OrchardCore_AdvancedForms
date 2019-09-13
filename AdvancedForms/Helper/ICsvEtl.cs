using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedForms.Helper
{
    public interface ICsvEtl
    {
        Task<string> GetAdvFormSubmissionsCSVstring(IEnumerable<ContentItem> pageOfContentItems);
    }
}
