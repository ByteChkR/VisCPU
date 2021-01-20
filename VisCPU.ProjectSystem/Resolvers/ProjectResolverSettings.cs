using System.Collections.Generic;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.ProjectSystem.Resolvers
{

    public class ProjectResolverSettings
    {
        [field: Argument( Name = "projects.origins" )]
        public Dictionary < string, string > ModuleOrigins { get; set; } =
            new Dictionary < string, string >
            {
                { "local", SettingsCategories.Get( "sdk.projects.origins.local", true ).GetCategoryDirectory() }
            };

        #region Private

        static ProjectResolverSettings()
        {
            SettingsCategory moduleCategory = SettingsCategories.Get( "sdk.module", true );

            SettingsManager.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                moduleCategory,
                "resolver-settings.json",
                new ProjectResolverSettings()
            );
        }

        #endregion
    }

}
