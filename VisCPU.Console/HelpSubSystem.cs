using System;
using System.Collections.Generic;

using viscc;

using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Peripherals;
using VisCPU.Utility.Logging;

namespace VisCPU.Console
{

    internal class HelpSubSystem : ConsoleSubsystem
    {

        public override void Run( IEnumerable < string > arguments )
        {
            BuilderSettings bs = new BuilderSettings();
            RunnerSettings rs = new RunnerSettings();
            CLISettings cs = new CLISettings();
            LinkerSettings ls = new LinkerSettings();
            ConsoleInInterfaceSettings ins = new ConsoleInInterfaceSettings();
            ConsoleOutInterfaceSettings outs = new ConsoleOutInterfaceSettings();
            HLCompilerSettings hs = new HLCompilerSettings();

            IEnumerable<string> args = ArgumentSyntaxParser.GetArgNames(bs, rs, cs, ls, ins, outs, hs);

            foreach (string s1 in args)
            {
                System.Console.WriteLine("Arg Name: " + s1);
            }


            System.Console.WriteLine("-log Subsystems: ");
            string[] names = Enum.GetNames<LoggerSystems>();

            foreach (string name in names)
            {
                System.Console.WriteLine("\t" + name);
            }
        }

    }

}