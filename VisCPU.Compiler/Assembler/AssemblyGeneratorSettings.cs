using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Compiler.Assembler
{

    public class AssemblyGeneratorSettings
    {

        [field: Argument( Name = "assembler:offset.global" )]
        public uint GlobalOffset { get; set; }

        #region Public

        public static AssemblyGeneratorSettings Create()
        {
            return SettingsSystem.GetSettings < AssemblyGeneratorSettings >();
        }

        #endregion

        #region Private

        static AssemblyGeneratorSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "config/linker.json"
                                                             ),
                                                 new AssemblyGeneratorSettings()
                                                );
        }

        #endregion

    }

}
