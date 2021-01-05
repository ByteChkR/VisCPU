using System;
using System.Collections.Generic;
using System.IO;

using VisCPU.Compiler.Linking;
using VisCPU.Utility;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace VisCPU.Compiler.Assembler
{
    public class AssemblyGeneratorSettings
    {
        [Argument(Name = "assembler:offset.global")]
        public uint GlobalOffset;

        [Argument(Name = "assembler:offset.trim")]
        public bool TrimOffset;

        #region Private

        static AssemblyGeneratorSettings()
        {
            SettingsSystem.RegisterDefaultLoader(new JSONSettingsLoader(),
                                                 Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/linker.json"), new AssemblyGeneratorSettings());
        }

        #endregion

        #region Public

        public static AssemblyGeneratorSettings Create()
        {
            return SettingsSystem.GetSettings<AssemblyGeneratorSettings>();
        }

        #endregion
    }
    public abstract class AssemblyGenerator : VisBase
    {
        protected override LoggerSystems SubSystem => LoggerSystems.AssemblyGenerator;

        #region Public

        public abstract List<byte> Assemble(LinkerResult result);

        #endregion
    }
}