using System;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.HL
{

    [Serializable]
    public class HLCompilerSettings
    {

        [field: Argument( Name = "compiler:optimize-temp-vars" )]
        public bool OptimizeTempVarUsage { get; set; } = true;

        [field: Argument( Name = "compiler:optimize-const-expr" )]
        public bool OptimizeConstExpressions { get; set; }

        #region Private

        static HLCompilerSettings()
        {
            SettingsCategory hlcCategory = SettingsCategories.Get( "sdk.compiler.hl", true );

            SettingsManager.RegisterDefaultLoader(
                                                  new JSONSettingsLoader(),
                                                  hlcCategory,
                                                  "compiler.json",
                                                  new HLCompilerSettings()
                                                 );
        }

        #endregion

    }

}
