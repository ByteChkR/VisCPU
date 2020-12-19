using System;
using System.Collections.Generic;

using VisCPU.Compiler.Linking;
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

        public override void Run( IEnumerable < string > arguments )
        {
            BuilderSettings bs = new BuilderSettings();
            RunnerSettings rs = new RunnerSettings();
            CLISettings cs = new CLISettings();
            LinkerSettings ls = new LinkerSettings();
            ConsoleInInterfaceSettings ins = new ConsoleInInterfaceSettings();
            ConsoleOutInterfaceSettings outs = new ConsoleOutInterfaceSettings();
            HLCompilerSettings hs = new HLCompilerSettings();
            MemorySettings ms = new MemorySettings();
            MemoryBusSettings mbs = new MemoryBusSettings();

            IEnumerable < string > args = ArgumentSyntaxParser.GetArgNames( bs, rs, cs, ls, ins, outs, hs, ms, mbs);

            foreach ( string s1 in args )
            {
                System.Console.WriteLine( "Arg Name: " + s1 );
            }

            System.Console.WriteLine( "-log Subsystems: " );
            string[] names = Enum.GetNames( typeof( LoggerSystems ) );

            foreach ( string name in names )
            {
                System.Console.WriteLine( "\t" + name );
            }
        }

        #endregion

    }

}
