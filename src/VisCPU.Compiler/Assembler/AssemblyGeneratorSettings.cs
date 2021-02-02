﻿using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

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
                                                  new JsonSettingsLoader(),
                                                  assemblerCategory,
                                                  "assembler.json",
                                                  new AssemblyGeneratorSettings()
                                                 );
        }

        #endregion

    }

}
