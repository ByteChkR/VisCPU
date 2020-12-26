﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisCPU.Console.Core.Settings;
using VisCPU.Console.Core.Subsystems;
using VisCPU.Console.Core.Subsystems.Modules;
using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.Instructions;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core
{
    public class VisConsole : ConsoleSystem
    {
        public override Dictionary<string, ConsoleSubsystem> SubSystems =>
            new Dictionary<string, ConsoleSubsystem>
            {
                {"run", new ProgramRunner()},
                {"build", new ProgramBuilder()},
                {"project", new ModuleSubSystem()},
                {"origin", new OriginSubSystem()}
            };

        #region Public

        public static void RunConsole(string[] args)
        {
            VisConsole vs = new VisConsole();
            vs.Run(args);
        }

        #endregion

        #region Private

        [STAThread]
        private static void Main(string[] args)
        {
            RunConsole(args);
        }

        private void Run(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("No Arguments");

                return;
            }

            CLISettings s = CLISettings.Create();
            EventManager.RegisterDefaultHandlers();
            Logger.OnLogReceive += (x, y) => System.Console.WriteLine($"[{x}] {y}");
            ArgumentSyntaxParser.Parse(args, s, Logger.Settings);
            Run(args as IEnumerable<string>);
        }

        public static StringBuilder ListSubsystems(Dictionary<string, ConsoleSubsystem> ss, StringBuilder sb,
            int indentation = 0)
        {
            sb.Append('\t', indentation);
            sb.AppendLine("Sub Systems:");
            foreach (KeyValuePair<string, ConsoleSubsystem> consoleSubsystem in ss)
            {
                sb.Append('\t', indentation + 1);
                sb.AppendLine(consoleSubsystem.Key);
                if (consoleSubsystem.Value is ConsoleSystem cs)
                {
                    ListSubsystems(cs.SubSystems, sb, indentation + 1);
                }
            }
            return sb;
        }

        internal static void RunConsole(CLISettings settings, string[] args,
            Dictionary<string, ConsoleSubsystem> subsystems)
        {
            do
            {
                string[] subSystemKeys = {args[0]};

                if (args[0].StartsWith("[") && args[0].EndsWith("]"))
                {
                    subSystemKeys = args[0].Remove(args[0].Length - 1, 1).Remove(0, 1)
                        .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
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

                    if (cmd == "EXIT")
                    {
                        return;
                    }
                }

                if (settings.WaitOnExit)
                {
                    System.Console.WriteLine("Press any key to exit");
                    System.Console.ReadLine();
                }
            } while (settings.Continuous);
        }

        #endregion
    }
}