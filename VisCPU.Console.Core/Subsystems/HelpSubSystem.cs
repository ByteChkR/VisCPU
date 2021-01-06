using System;
using System.Collections.Generic;
using System.Text;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.HostFS;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems
{

    public class HelpSubSystem : ConsoleSubsystem
    {

        private readonly ConsoleSystem owner;

        #region Public

        public HelpSubSystem( ConsoleSystem owner )
        {
            this.owner = owner;
        }

        public override void Run( IEnumerable < string > arguments )
        {
            if ( owner != null )
            {
                Log( VisConsole.ListSubsystems( owner.SubSystems, new StringBuilder() ).ToString() );
            }

            WriteSubsystem( "CLI", CLISettings.Create() );
            WriteSubsystem( "build", BuilderSettings.Create() );

            WriteSettings( "linker", LinkerSettings.Create() );
            WriteSettings( "compiler", HLCompilerSettings.Create() );
            WriteSettings( "assembler", AssemblyGeneratorSettings.Create() );

            WriteSubsystem( "run", RunnerSettings.Create() );

            WriteSettings( "console-in", ConsoleInInterfaceSettings.Create() );
            WriteSettings( "console-out", ConsoleOutInterfaceSettings.Create() );
            WriteSettings( "memory", MemorySettings.Create() );
            WriteSettings( "memory-bus", MemoryBusSettings.Create() );
            WriteSettings( "hostfs", HostFileSystemSettings.Create() );

            System.Console.WriteLine( "-log Subsystems: " );
            string[] names = Enum.GetNames( typeof( LoggerSystems ) );

            foreach ( string name in names )
            {
                System.Console.WriteLine( "\t" + name );
            }
        }

        #endregion

        #region Private

        private void WriteSettings( string subName, object settings )
        {
            System.Console.WriteLine( $"Arguments: {subName}" );

            IEnumerable < string > args = ArgumentSyntaxParser.GetArgNames( settings );

            foreach ( string s1 in args )
            {
                System.Console.WriteLine( "\tArg Name: " + s1 );
            }
        }

        private void WriteSubsystem( string subName, object settings )
        {
            System.Console.WriteLine( $"Arguments: {subName}" );

            IEnumerable < string > args = ArgumentSyntaxParser.GetArgNames( settings );

            foreach ( string s1 in args )
            {
                System.Console.WriteLine( "\tArg Name: " + s1 );
            }
        }

        #endregion

    }

}
