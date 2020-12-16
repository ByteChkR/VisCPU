using System;

using VisCPU.Utility.Settings;

namespace VisCPU.HL
{

    [Serializable]
    public class HLCompilerSettings
    {
        static HLCompilerSettings()
        {
            Settings.RegisterDefaultLoader(new JSONSettingsLoader(), "config/hl-compiler.json", new HLCompilerSettings());
        }

        public static HLCompilerSettings Create()
        {
            return Settings.GetSettings<HLCompilerSettings>();
        }

        [Argument(Name = "optimize-temp-vars")]
        public bool OptimizeTempVarUsage = false;

    }

}