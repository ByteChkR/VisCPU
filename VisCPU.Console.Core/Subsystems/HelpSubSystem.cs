using System;
using System.Collections.Generic;
using VisCPU.Compiler.Linking;
using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems
{
    public class HelpSubSystem : ConsoleSubsystem
    {
        #region Public

        public override void Run(IEnumerable<string> arguments)
        {
            WriteSubsystem("CLI", CLISettings.Create());
            WriteSubsystem("build", BuilderSettings.Create());

            WriteSettings("linker", LinkerSettings.Create());
            WriteSettings("compiler", HLCompilerSettings.Create());

            WriteSubsystem("run", RunnerSettings.Create());

            WriteSettings("console-in", ConsoleInInterfaceSettings.Create());
            WriteSettings("console-out", ConsoleOutInterfaceSettings.Create());
            WriteSettings("memory", MemorySettings.Create());
            WriteSettings("memory-bus", MemoryBusSettings.Create());

            System.Console.WriteLine("-log Subsystems: ");
            string[] names = Enum.GetNames(typeof(LoggerSystems));

            foreach (string name in names)
            {
                System.Console.WriteLine("\t" + name);
            }
        }

        #endregion


        private void WriteSubsystem(string subName, object settings)
        {
            System.Console.WriteLine($"Subsystem: {subName}");

            IEnumerable<string> args = ArgumentSyntaxParser.GetArgNames(settings);

            foreach (string s1 in args)
            {
                System.Console.WriteLine("\tArg Name: " + s1);
            }
        }

        private void WriteSettings(string subName, object settings)
        {
            System.Console.WriteLine($"Settings: {subName}");

            IEnumerable<string> args = ArgumentSyntaxParser.GetArgNames(settings);

            foreach (string s1 in args)
            {
                System.Console.WriteLine("\tArg Name: " + s1);
            }
        }
    }
}