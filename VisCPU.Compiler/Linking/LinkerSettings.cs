using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Compiler.Linking
{

    public class LinkerSettings
    {

        [field: Argument( Name = "linker:no-hide" )]
        public bool NoHiddenItems { get; }

        [field: Argument( Name = "linker:export-info" )]
        [field: Argument( Name = "linker:export" )]
        public bool ExportLinkerInfo { get; set; }

        #region Public

        public static LinkerSettings Create()
        {
            return SettingsSystem.GetSettings < LinkerSettings >();
        }

        #endregion

        #region Private

        static LinkerSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "config/linker.json"
                                                             ),
                                                 new LinkerSettings()
                                                );
        }

        #endregion

    }

}
