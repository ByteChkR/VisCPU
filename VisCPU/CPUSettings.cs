using VisCPU.Instructions;

namespace VisCPU
{

    public static class CPUSettings
    {

        public static readonly uint s_InstructionSize = 4;
        public static readonly uint s_ByteSize = 16;

        public static bool DumpOnCrash { get; set; }

        public static bool WarnOnUnmappedAccess { get; set; }

        public static InstructionSet InstructionSet { get; set; }

    }

}
