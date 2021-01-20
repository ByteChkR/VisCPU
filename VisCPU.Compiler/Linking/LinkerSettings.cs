using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Compiler.Linking
{

    public class LinkerSettings
    {
        [field: Argument( Name = "linker:no-hide" )]
        public bool NoHiddenItems { get; set; }

        [field: Argument( Name = "linker:export-info" )]
        [field: Argument( Name = "linker:export" )]
        public bool ExportLinkerInfo { get; set; }

        #region Private

        static LinkerSettings()
        {
            SettingsCategory linkerCategory = SettingsCategories.Get( "sdk.compiler.vasm", true );

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                linkerCategory,
                "linker.json",
                new LinkerSettings()
            );
        }

        #endregion
    }

}
