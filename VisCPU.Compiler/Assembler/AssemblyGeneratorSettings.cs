using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Compiler.Assembler
{

    public class AssemblyGeneratorSettings
    {
        [field: Argument( Name = "assembler:offset.global" )]
        public uint GlobalOffset { get; set; }

        #region Private

        static AssemblyGeneratorSettings()
        {
            SettingsCategory assemblerCategory = SettingsCategories.Get( "sdk.compiler.vasm", true );

            SettingsManager.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                assemblerCategory,
                "assembler.json",
                new AssemblyGeneratorSettings()
            );
        }

        #endregion
    }

}
