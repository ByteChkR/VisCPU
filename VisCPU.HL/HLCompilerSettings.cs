using System;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.HL
{

    [Serializable]
    public class HlCompilerSettings
    {
        [field: Argument( Name = "compiler:optimize-temp-vars" )]
        public bool OptimizeTempVarUsage { get; set; } = true;

        [field: Argument( Name = "compiler:optimize-const-expr" )]
        public bool OptimizeConstExpressions { get; set; }
        [field: Argument(Name = "compiler:optimize-reduce-expr")]
        public bool OptimizeReduceExpressions { get; set; }
        [field: Argument(Name = "compiler:optimize-if-expr")]
        public bool OptimizeIfConditionExpressions { get; set; }

        #region Private

        static HlCompilerSettings()
        {
            SettingsCategory hlcCategory = SettingsCategories.Get( "sdk.compiler.hl", true );

            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                hlcCategory,
                "compiler.json",
                new HlCompilerSettings()
            );
        }

        #endregion
    }

}
