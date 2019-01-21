using System.Threading.Tasks;
using OrchardCore.Data.Migration;
using OrchardCore.Recipes.Services;
using OrchardCore.Recipes.Models;

namespace AdvancedForms
{
    public class Migrations : DataMigration
    {

        private readonly IRecipeMigrator _recipeMigrator;

        public Migrations(IRecipeMigrator recipeMigrator)
        {
            _recipeMigrator = recipeMigrator;
        }

        public async Task<int> CreateAsync()
        {
            await _recipeMigrator.ExecuteAsync("advancedforms.recipe.json", this);

            return 1;
        }
    }
}
