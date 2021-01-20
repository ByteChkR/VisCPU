using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.Peripherals;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems
{

    public class ProgramRunner : ConsoleSubsystem
    {
        #region Public

        public static void Run( Dictionary < string, string > args )
        {
            CpuSettings cpuSettings = SettingsManager.GetSettings < CpuSettings >();
            RunnerSettings settings = new RunnerSettings();

            HlCompilerSettings hls = SettingsManager.GetSettings < HlCompilerSettings >();
            MemorySettings ms = SettingsManager.GetSettings < MemorySettings >();
            MemoryBusSettings mbs = SettingsManager.GetSettings < MemoryBusSettings >();

            ArgumentSyntaxParser.Parse(
                args,
                settings,
                hls,
                ms,
                mbs,
                cpuSettings
            );

            SettingsManager.SaveSettings( hls );
            SettingsManager.SaveSettings( ms );
            SettingsManager.SaveSettings( mbs );
            SettingsManager.SaveSettings( cpuSettings );

            if ( settings.InputFiles == null )
            {
                return;
            }

            string origPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory( settings.WorkingDir );

            foreach ( string f in settings.InputFiles )
            {
                string file = Path.GetFullPath( f );

                file = RunPreRunSteps( settings, file );

                if ( file == null || !File.Exists( file ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( file, true ) );

                    continue;
                }

                Logger.LogMessage( LoggerSystems.Console, "Run File: '{0}'", file );
                uint[] fileCode = File.ReadAllBytes( file ).ToUInt();

                MemoryBus bus = CreateBus( mbs );

                Cpu cpu = new Cpu( bus, cpuSettings.CpuResetAddr, cpuSettings.CpuIntAddr );
                cpu.LoadBinary( fileCode );
                cpu.Run();
            }

            Directory.SetCurrentDirectory( origPath );
        }

        public override void Help()
        {
            CpuSettings cpuSettings = SettingsManager.GetSettings < CpuSettings >();
            RunnerSettings settings = new RunnerSettings();

            HlCompilerSettings hls = SettingsManager.GetSettings < HlCompilerSettings >();
            MemorySettings ms = SettingsManager.GetSettings < MemorySettings >();
            MemoryBusSettings mbs = SettingsManager.GetSettings < MemoryBusSettings >();

            HelpSubSystem.WriteSubsystem( "vis run", settings, hls, ms, mbs, cpuSettings );
        }

        public override void Run( IEnumerable < string > args )
        {
            CpuSettings cpuSettings = SettingsManager.GetSettings < CpuSettings >();
            RunnerSettings settings = new RunnerSettings();
            HlCompilerSettings hls = SettingsManager.GetSettings < HlCompilerSettings >();
            MemorySettings ms = SettingsManager.GetSettings < MemorySettings >();
            MemoryBusSettings mbs = SettingsManager.GetSettings < MemoryBusSettings >();

            ArgumentSyntaxParser.Parse(
                args.ToArray(),
                settings,
                hls,
                ms,
                mbs,
                cpuSettings
            );

            SettingsManager.SaveSettings( hls );
            SettingsManager.SaveSettings( ms );
            SettingsManager.SaveSettings( mbs );
            SettingsManager.SaveSettings( cpuSettings );

            if ( settings.InputFiles == null )
            {
                return;
            }

            string origPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory( settings.WorkingDir );

            foreach ( string f in settings.InputFiles )
            {
                string file = Path.GetFullPath( f );

                file = RunPreRunSteps( settings, file );

                if ( file == null || !File.Exists( file ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( file, true ) );

                    continue;
                }

                Log( $"Run File: '{file}'" );
                uint[] fileCode = File.ReadAllBytes( file ).ToUInt();

                MemoryBus bus = CreateBus( mbs );

                Cpu cpu = new Cpu( bus, cpuSettings.CpuResetAddr, cpuSettings.CpuIntAddr );
                cpu.LoadBinary( fileCode );
                cpu.Run();
            }

            Directory.SetCurrentDirectory( origPath );
        }

        #endregion

        #region Private

        private static MemoryBus CreateBus( MemoryBusSettings settings, params Peripheral[] additionalPeripherals )
        {
            return new MemoryBus(
                settings.MemoryDevices.Select(
                             x => new Memory(
                                 SettingsManager.
                                     GetSettings < MemorySettings >(
                                         x
                                     )
                             )
                         ).
                         Concat( additionalPeripherals ).
                         Concat( Peripheral.GetExtensionPeripherals() )
            );
        }

        private static string RunPreRunSteps( RunnerSettings settings, string file )
        {
            string ret = file;

            foreach ( KeyValuePair < string, Func < string, string > > keyValuePair in settings.PreRunMap )
            {
                if ( Path.GetExtension( ret ) == keyValuePair.Key )
                {
                    ret = RunPreRunSteps( settings, keyValuePair.Value( ret ) );
                }
            }

            return ret;
        }

        #endregion
    }

}
