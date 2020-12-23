using System;
using System.IO;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.HL
{
    [Serializable]
    public class HLCompilerSettings
    {
        [Argument(Name = "compiler:optimize-temp-vars")]
        public bool OptimizeTempVarUsage = false;

        #region Private

        static HLCompilerSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "config/hl-compiler.json"),
                new HLCompilerSettings()
            );
        }

        #endregion

        #region Public

        public static HLCompilerSettings Create()
        {
            return SettingsSystem.GetSettings<HLCompilerSettings>();
        }

        #endregion
    }
}