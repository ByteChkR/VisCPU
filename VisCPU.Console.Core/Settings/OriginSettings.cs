using System;
using System.Collections.Generic;
using System.IO;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
{
    [Serializable]
    public class OriginSettings
    {
        static OriginSettings()
        {
            Utility.Settings.SettingsSystem.RegisterDefaultLoader(new JSONSettingsLoader(), Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "config/origins.json"
            ), new OriginSettings());
        }
        public Dictionary<string, string> origins = new Dictionary<string, string>
        {
            { "local", Utility.Settings.SettingsSystem.GetDefaultFile < OriginSettings >() }
        };

        public static OriginSettings Create() => Utility.Settings.SettingsSystem.GetSettings < OriginSettings >();

    }
}