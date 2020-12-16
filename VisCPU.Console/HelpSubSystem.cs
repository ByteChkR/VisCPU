using System;
using System.Collections.Generic;

using VisCPU.Compiler.Linking;
using VisCPU.Console.Subsystems;
using VisCPU.HL;
using VisCPU.Peripherals.Console;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console
{

    internal class HelpSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Run( IEnumerable < string > arguments )
        {
            BuilderSettings bs = new();
            RunnerSettings rs = new();
            CLISettings cs = new();
            LinkerSettings ls = new();
            ConsoleInInterfaceSettings ins = new();
            ConsoleOutInterfaceSettings outs = new();
            HLCompilerSettings hs = new();

            IEnumerable < string > args = ArgumentSyntaxParser.GetArgNames( bs, rs, cs, ls, ins, outs, hs );

            foreach ( string s1 in args )
            {
                System.Console.WriteLine( "Arg Name: " + s1 );
            }

            System.Console.WriteLine( "-log Subsystems: " );
            string[] names = Enum.GetNames < LoggerSystems >();

            foreach ( string name in names )
            {
                System.Console.WriteLine( "\t" + name );
            }
        }

        #endregion

    }

}
