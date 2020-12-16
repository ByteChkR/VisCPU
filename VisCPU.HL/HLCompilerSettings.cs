using System;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.HL
{

    [Serializable]
    public class HLCompilerSettings
    {

        [Argument( Name = "optimize-temp-vars" )]
        public bool OptimizeTempVarUsage = false;

        #region Public

        public static HLCompilerSettings Create()
        {
            return Settings.GetSettings < HLCompilerSettings >();
        }

        #endregion

        #region Private

        static HLCompilerSettings()
        {
            Settings.RegisterDefaultLoader(
                                           new JSONSettingsLoader(),
                                           "config/hl-compiler.json",
                                           new HLCompilerSettings()
                                          );
        }

        #endregion

    }

}
