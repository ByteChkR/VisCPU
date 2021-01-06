namespace VisCPU
{

    public static class CPUSettings
    {

        public const int INSTRUCTION_SIZE = 4;
        public const int BYTE_SIZE = 16;

        public static bool DumpOnCrash { get; set; } = false;

        public static bool WarnOnUnmappedAccess { get; set; } = false;

        public static InstructionSet InstructionSet { get; set; }

    }

}
