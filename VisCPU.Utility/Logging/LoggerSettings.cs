using System;

using viscc;

namespace VisCPU.Utility.Logging
{

    [Flags]
    public enum LoggerSystems : int
    {

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

        AllInstructions = MathInstructions |
                          LogicInstructions |
                          BaseInstructions |
                          StackInstructions |
                          BitwiseInstructions |
                          BranchInstructions |
                          MemoryInstructions,

        HL = HL_Parser | HL_Compiler,

        Compiler = AssemblyGenerator | FileCompilation | Linker | Parser,
        
        Default = HL | Compiler | UriResolver | Console
    }

    public static class Logger
    {

        public static readonly LoggerSettings Settings = new LoggerSettings();
        public static event Action<LoggerSystems, string> OnLogReceive;

        internal static void LogMessage(LoggerSystems subsystem, string message)
        {

            if (!Settings.EnableAll && (Settings.EnabledSystems & subsystem) == 0)
                return;

            OnLogReceive?.Invoke(subsystem, message);
        }

    }

    public class LoggerSettings
    {

        [Argument(Name = "log-all")]
        public bool EnableAll;

        [Argument(Name = "log")]
        private LoggerSystems enabledSystems = LoggerSystems.Default;

        public LoggerSystems EnabledSystems => (LoggerSystems)(EnableAll ? -1 : (int)enabledSystems);


    }

}
