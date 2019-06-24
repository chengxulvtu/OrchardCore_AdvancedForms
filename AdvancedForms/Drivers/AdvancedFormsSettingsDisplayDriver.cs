using OrchardCore.DisplayManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Settings;
using AdvancedForms.Models;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.DisplayManagement.Handlers;

namespace AdvancedForms.Drivers
{
    public class AdvancedFormsSettingsDisplayDriver : SectionDisplayDriver<ISite, AdvancedFormsSettings>
    {
        public const string GroupId = "AdvancedFormsSettings";

        public override IDisplayResult Edit(AdvancedFormsSettings section)
        {
            return Initialize<AdvancedFormsSettings>("AdvancedFormsSettings_Edit", model => {
                model.Footer = section.Footer;
                model.Header = section.Header;
            }).Location("Content:5").OnGroup(GroupId);
        }

        public override async Task<IDisplayResult> UpdateAsync(AdvancedFormsSettings section, BuildEditorContext context)
        {
            if (context.GroupId == GroupId)
            {
                await context.Updater.TryUpdateModelAsync(section, Prefix);
            }
            return await EditAsync(section, context);
        }
    }
}
