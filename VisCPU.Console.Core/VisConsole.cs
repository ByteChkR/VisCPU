using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VisCPU.Console.Core.Settings;
using VisCPU.Console.Core.Subsystems;
using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.Instructions;
using VisCPU.Peripherals;
using VisCPU.Peripherals.Benchmarking;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.Console.IO;
using VisCPU.Peripherals.HostFS;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core
{

    public class VisConsole : ConsoleSystem
    {
        public override Dictionary < string, ConsoleSubsystem > SubSystems =>
            new Dictionary < string, ConsoleSubsystem >
            {
                { "run", new ProgramRunner() },
                { "build", new ProgramBuilder() },
                { "project", new ProjectSubSystem() },
                { "origin", new OriginSubSystem() },
                { "reset", new ConsoleReset() }
            };

        #region Public


        public static StringBuilder ListSubsystems(
            Dictionary < string, ConsoleSubsystem > ss,
            StringBuilder sb ) =>
            ListSubsystems( ss, sb, 0 );
        public static StringBuilder ListSubsystems(
            Dictionary < string, ConsoleSubsystem > ss,
            StringBuilder sb,
            int indentation)
        {
            sb.Append( '\t', indentation );
            sb.AppendLine( "Sub Systems:" );

            foreach ( KeyValuePair < string, ConsoleSubsystem > consoleSubsystem in ss )
            {
                sb.Append( '\t', indentation + 1 );
                sb.AppendLine( consoleSubsystem.Key );

                if ( consoleSubsystem.Value is ConsoleSystem cs )
                {
                    ListSubsystems( cs.SubSystems, sb, indentation + 1 );
                }
            }

            return sb;
        }

        public static void RunConsole( string[] args )
        {
            CpuSettings.FallbackSet = new DefaultSet();

#if DEBUG
            Peripheral.DebugPeripherals = new Peripheral[]
            {
                new BenchmarkDevice(),
                new ConsoleInInterface(),
                new ConsoleOutInterface(),
                new HostFileSystem(),
                new ConsoleInterface()
            };
#endif

            VisConsole vs = new VisConsole();
            vs.Run( args );
        }

        internal static void RunConsole(
            CliSettings settings,
            string[] args,
            Dictionary < string, ConsoleSubsystem > subsystems )
        {
            do
            {
                string[] subSystemKeys = { args[0] };

                if ( args[0].StartsWith( "[" ) && args[0].EndsWith( "]" ) )
                {
                    subSystemKeys = args[0].
                                    Remove( args[0].Length - 1, 1 ).
                                    Remove( 0, 1 ).
                                    Split( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
                }

                foreach ( string subSystemKey in subSystemKeys )
                {
                    if ( !subsystems.TryGetValue( subSystemKey, out ConsoleSubsystem subsystem ) )
                    {
                        System.Console.WriteLine( "Invalid Argument" );

                        continue;
                    }

                    subsystem.Run( args.Skip( 1 ) );
                }

                if ( settings.Continuous )
                {
                    System.Console.WriteLine( "Type 'exit' to close console" );
                    System.Console.Write( "cli>" );
                    string cmd = System.Console.ReadLine().ToUpper();

                    if ( cmd == "EXIT" )
                    {
                        return;
                    }
                }

                if ( settings.WaitOnExit )
                {
                    System.Console.WriteLine( "Press any key to exit" );
                    System.Console.ReadLine();
                }
            }
            while ( settings.Continuous );
        }

        #endregion

        #region Private

        private void Run( string[] args )
        {
            if ( args.Length < 1 )
            {
                System.Console.WriteLine( "No Arguments" );

                return;
            }

            CliSettings s = SettingsManager.GetSettings < CliSettings >();
            EventManager.RegisterDefaultHandlers();
            Logger.OnLogReceive += ( x, y ) => System.Console.WriteLine( $"[{x}] {y}" );
            ArgumentSyntaxParser.Parse( args, s, Logger.s_Settings );

            Run(
                args.Concat(
                    s.Configs.SelectMany(
                        x => File.Exists( x )
                            ? File.ReadAllText( x ).
                                   Split(
                                       new[] { '\n', '\r', ' ' },
                                       StringSplitOptions.RemoveEmptyEntries
                                   )
                            : new string[0]
                    )
                )
            );
        }

        #endregion
    }

}
