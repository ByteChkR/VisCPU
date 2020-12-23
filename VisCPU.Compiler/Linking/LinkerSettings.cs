﻿using System;
using System.IO;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Compiler.Linking
{
    public class LinkerSettings
    {
        [Argument(Name = "linker:export-info")] [Argument(Name = "linker:export")]
        public bool ExportLinkerInfo;

        [Argument(Name = "linker:no-hide")] public bool NoHiddenItems;

        #region Private

        static LinkerSettings()
        {
            SettingsSystem.RegisterDefaultLoader(new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/linker.json"), new LinkerSettings());
        }

        #endregion

        #region Public

        public static LinkerSettings Create()
        {
            return SettingsSystem.GetSettings<LinkerSettings>();
        }

        #endregion
    }
}