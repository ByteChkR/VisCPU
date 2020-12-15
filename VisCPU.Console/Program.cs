using System;
using System.Collections.Generic;
using System.Linq;

using viscc;

using VisCPU;
using VisCPU.Instructions;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.Console
{
    internal static class Program
    {

        private static readonly Dictionary<string, ConsoleSubsystem> subsystems =
            new Dictionary<string, ConsoleSubsystem>
            {
                { "run", new ProgramRunner() },
                { "build", new ProgramBuilder() }
            };

        private static void RunConsole(CLISettings settings, string[] args)
        {
            do
            {
                string[] subSystemKeys = { args[0] };
                if (args[0].StartsWith('[') && args[0].EndsWith(']'))
                {
                    subSystemKeys = args[0].Remove(args[0].Length - 1, 1).Remove(0, 1)
                                           .Split(';', StringSplitOptions.RemoveEmptyEntries);
                }

                CPUSettings.InstructionSet = new DefaultSet();
                foreach (string subSystemKey in subSystemKeys)
                {
                    if (!subsystems.TryGetValue(subSystemKey, out ConsoleSubsystem subsystem))
                    {
                        System.Console.WriteLine("Invalid Argument");
                        continue;
                    }

                    subsystem.Run(args.Skip(1));
                }

                System.Console.WriteLine();
                System.Console.WriteLine("Process Stopped.");
                if (settings.Continuous)
                {
                    System.Console.WriteLine("Type 'exit' to close console");
                    System.Console.Write("cli>");
                    string cmd = System.Console.ReadLine().ToUpper();
                    if (cmd == "EXIT") return;
                }
                if (settings.WaitOnExit)
                {
                    System.Console.WriteLine("Press any key to exit");
                    System.Console.ReadLine();
                }
            } while (settings.Continuous);
        }

        public static void RunConsole(string[] args) => Main(args);

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("No Arguments");
                return;
            }

            CLISettings s = new CLISettings();
            EventManager.Initialize();
            Logger.OnLogReceive += ( x, y ) => System.Console.WriteLine( $"[{x}] {y}" );
            ArgumentSyntaxParser.Parse(args, s, Logger.Settings);

            RunConsole(s, args);
        }

        

    }
    public class CLISettings
    {
        [Argument(Name = "continuous")]
        [Argument(Name = "loop")]
        public readonly bool Continuous = false;
        [Argument(Name = "waitOnExit")]
        public readonly bool WaitOnExit = false;

    }
}