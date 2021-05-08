using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Console.Core.Subsystems.FileSystemBuilder
{

    public class DriveImageBuilderSettings
    {
        [Argument( Name = "dib:input" )]
        [Argument( Name = "dib:i" )]
        public string[] InputFiles = new string[0];

        [Argument( Name = "dib:image.format" )]
        public string[] ImageFormats = new[] { "FSv1" };

        #region Private

        static DriveImageBuilderSettings()
        {
            SettingsCategory diCategory = SettingsCategories.Get( "sdk.utils.disk", true );

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                diCategory,
                "builder.json",
                new DriveImageBuilderSettings()
            );
        }

        #endregion
    }

}
