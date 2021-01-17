using System.Collections.Generic;

using VisCPU.Extensions;
using VisCPU.Instructions;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU
{

    public class CPUSettings
    {

        public static readonly SettingsCategory s_CpuCategory = SettingsCategories.Get( "cpu", true );

        public static readonly SettingsCategory s_CpuExtensionsCategory =
            SettingsCategories.Get( "cpu.extensions", true );

        public static readonly SettingsCategory s_CpuInstructionExtensionsCategory =
            SettingsCategories.Get( "cpu.extensions.instructions", true );

        private static InstructionSet s_CachedInstructionSet = null;

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
                                                                s_CpuInstructionExtensionsCategory.
                                                                    GetCategoryDirectory(),
                                                                true
                                                               );

                foreach ( InstructionSet instructionSet in extensions )
                {
                    if ( instructionSet.SetKey == SettingsManager.GetSettings < CPUSettings >().InstructionSetName )
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

        static CPUSettings()
        {
            SettingsManager.RegisterDefaultLoader(
                                                  new JSONSettingsLoader(),
                                                  s_CpuCategory,
                                                  "cpu.json",
                                                  new CPUSettings()
                                                 );
        }

        #endregion

    }

}
