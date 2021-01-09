using System;

namespace VisCPU.Utility.Logging
{

    [Flags]
    public enum LoggerSystems
    {

        All = -1,
        EventSystem = 1,
        MemoryBus = 2,
        AssemblyGenerator = 4,
        FileCompilation = 8,
        Linker = 16,
        Parser = 32,
        Console = 64,
        HL_Compiler = 128,
        HL_Parser = 256,
        Peripherals = 512,
        MathInstructions = 1024,
        LogicInstructions = 2048,
        BaseInstructions = 4096,
        StackInstructions = 8192,
        BitwiseInstructions = 16384,
        BranchInstructions = 32768,
        MemoryInstructions = 65536,
        UriResolver = 131072,
        HL_Importer = 262144,
        ModuleSystem = 524288,
        Emit = 1048576,

        AllInstructions = MathInstructions |
                          LogicInstructions |
                          BaseInstructions |
                          StackInstructions |
                          BitwiseInstructions |
                          BranchInstructions |
                          MemoryInstructions,

        HL = HL_Parser | HL_Compiler | HL_Importer,

        Compiler = AssemblyGenerator | FileCompilation | Linker | Parser,

        Default = UriResolver | Console | ModuleSystem

    }

}
