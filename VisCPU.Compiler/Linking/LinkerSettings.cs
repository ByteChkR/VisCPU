using VisCPU.Utility.Settings;

namespace VisCPU.Compiler.Linking
{

    public class LinkerSettings
    {
        [Argument(Name = "linker:no-hide")]
        public bool NoHiddenItems;
        
        [Argument(Name = "linker:export-info")]
        [Argument(Name = "linker:export")]
        public bool ExportLinkerInfo;

        static LinkerSettings()
        {
            Settings.RegisterDefaultLoader( new JSONSettingsLoader(), "config/linker.json", new LinkerSettings() );
        }
        public static LinkerSettings Create() => Settings.GetSettings < LinkerSettings >();

    }

}