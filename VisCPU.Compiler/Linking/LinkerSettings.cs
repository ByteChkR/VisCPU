using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Compiler.Linking
{

    public class LinkerSettings
    {

        [Argument( Name = "linker:no-hide" )]
        public bool NoHiddenItems;

        [Argument( Name = "linker:export-info" )]
        [Argument( Name = "linker:export" )]
        public bool ExportLinkerInfo;

        #region Public

        public static LinkerSettings Create()
        {
            return SettingsSystem.GetSettings < LinkerSettings >();
        }

        #endregion

        #region Private

        static LinkerSettings()
        {
            SettingsSystem.RegisterDefaultLoader( new JSONSettingsLoader(), "config/linker.json", new LinkerSettings() );
        }

        #endregion

    }

}
