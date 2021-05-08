using System.Collections.Generic;
using VisCPU.Instructions;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Extensions;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;
using VisCPU.Utility.Logging;

namespace VisCPU
{

    public class CpuSettings
    {
        private static InstructionSet s_CachedInstructionSet;

        public static SettingsCategory CpuCategory => SettingsCategories.Get( "cpu", true );

        public static SettingsCategory CpuExtensionsCategory => SettingsCategories.Get( "cpu.extensions", true );

        public static SettingsCategory CpuInstructionExtensionsCategory =>
            SettingsCategories.Get( "cpu.extensions.instructions", true );

        public static uint InstructionSize { get; } = 4;

        public static uint ByteSize { get; } = 16;

        public static InstructionSet FallbackSet { get; set; }

        public static InstructionSet InstructionSet
        {
            get
            {
                if ( s_CachedInstructionSet != null )
                {
                    return s_CachedInstructionSet;
                }

                IEnumerable < InstructionSet > extensions =
                    ExtensionLoader.LoadFrom < InstructionSet >(
                        CpuInstructionExtensionsCategory.
                            GetCategoryDirectory(),
                        true
                    );

                foreach ( InstructionSet instructionSet in extensions )
                {
                    Logger.LogMessage(
                        LoggerSystems.Console,
                        "Using Instruction Set: {0}",
                        SettingsManager.GetSettings < CpuSettings >().InstructionSetName
                    );

                    if ( instructionSet.SetKey == SettingsManager.GetSettings < CpuSettings >().InstructionSetName )
                    {
                        s_CachedInstructionSet = instructionSet;

                        break;
                    }
                }

                return s_CachedInstructionSet = s_CachedInstructionSet ?? FallbackSet;
            }
        }

        [field: Argument( Name = "cpu.interrupt" )]
        public uint CpuIntAddr { get; set; }

        [field: Argument( Name = "cpu.reset" )]
        public uint CpuResetAddr { get; set; }

        [field: Argument( Name = "cpu.crashdump" )]
        public bool DumpOnCrash { get; set; }

        [field: Argument( Name = "cpu.unmapped.warn" )]
        public bool WarnOnUnmappedAccess { get; set; }

        [field: Argument( Name = "cpu.instruction-set" )]
        public string InstructionSetName { get; set; } = "VisCPU-debug-set.v1";

        #region Private

        static CpuSettings()
        {
            SettingsManager.RegisterDefaultLoader(
                new JsonSettingsLoader(),
                CpuCategory,
                "cpu.json",
                new CpuSettings()
            );
        }

        #endregion
    }

}
