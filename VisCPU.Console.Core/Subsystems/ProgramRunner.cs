using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.Peripherals.Benchmarking;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.HostFS;
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

        public override void Help()
        {
            RunnerSettings settings = RunnerSettings.Create();
            ConsoleInInterfaceSettings cins = ConsoleInInterfaceSettings.Create();
            ConsoleOutInterfaceSettings couts = ConsoleOutInterfaceSettings.Create();
            HLCompilerSettings hls = HLCompilerSettings.Create();
            MemorySettings ms = MemorySettings.Create();
            MemoryBusSettings mbs = MemoryBusSettings.Create();
            HostFileSystemSettings hfs = HostFileSystemSettings.Create();

            HelpSubSystem.WriteSubsystem("vis run", settings, cins, couts, hls, ms, mbs, hfs);
        }

        #region Public

        public static void Run(Dictionary <string, string> args)
        {
            RunnerSettings settings = RunnerSettings.Create();
            ConsoleInInterfaceSettings cins = ConsoleInInterfaceSettings.Create();
            ConsoleOutInterfaceSettings couts = ConsoleOutInterfaceSettings.Create();
            HLCompilerSettings hls = HLCompilerSettings.Create();
            MemorySettings ms = MemorySettings.Create();
            MemoryBusSettings mbs = MemoryBusSettings.Create();
            HostFileSystemSettings hfs = HostFileSystemSettings.Create();

            ArgumentSyntaxParser.Parse(
                                       args,
                                       settings,
                                       cins,
                                       couts,
                                       hls,
                                       ms,
                                       mbs
                                      );

            SettingsSystem.SaveSettings(settings);
            SettingsSystem.SaveSettings(cins);
            SettingsSystem.SaveSettings(couts);
            SettingsSystem.SaveSettings(hls);
            SettingsSystem.SaveSettings(ms);
            SettingsSystem.SaveSettings(mbs);
            SettingsSystem.SaveSettings(hfs);

            if (settings.InputFiles == null)
            {
                return;
            }

            foreach (string f in settings.InputFiles)
            {
                string file = Path.GetFullPath(f);

                file = RunPreRunSteps(settings, file);

                if (file == null || !File.Exists(file))
                {
                    EventManager<ErrorEvent>.SendEvent(new FileNotFoundEvent(file, true));

                    continue;
                }

                Logger.LogMessage(LoggerSystems.Console,"Run File: '{0}'", file);
                uint[] fileCode = File.ReadAllBytes(file).ToUInt();

                ConsoleInInterface cin = new ConsoleInInterface();

                ConsoleOutInterface cout =
                    new ConsoleOutInterface();

                HostFileSystem hostFS = new HostFileSystem();
                BenchmarkDevice benchDev = new BenchmarkDevice();

                MemoryBus bus = mbs.CreateBus(cout, cin, hostFS, benchDev);

                CPU cpu = new CPU(bus, settings.CpuResetAddr, settings.CpuIntAddr);
                cpu.LoadBinary(fileCode);
                cpu.Run();
            }
        }

        public override void Run( IEnumerable < string > args )
        {
            RunnerSettings settings = RunnerSettings.Create();
            ConsoleInInterfaceSettings cins = ConsoleInInterfaceSettings.Create();
            ConsoleOutInterfaceSettings couts = ConsoleOutInterfaceSettings.Create();
            HLCompilerSettings hls = HLCompilerSettings.Create();
            MemorySettings ms = MemorySettings.Create();
            MemoryBusSettings mbs = MemoryBusSettings.Create();
            HostFileSystemSettings hfs = HostFileSystemSettings.Create();

            ArgumentSyntaxParser.Parse(
                                       args.ToArray(),
                                       settings,
                                       cins,
                                       couts,
                                       hls,
                                       ms,
                                       mbs
                                      );

            SettingsSystem.SaveSettings( settings );
            SettingsSystem.SaveSettings( cins );
            SettingsSystem.SaveSettings( couts );
            SettingsSystem.SaveSettings( hls );
            SettingsSystem.SaveSettings( ms );
            SettingsSystem.SaveSettings( mbs );
            SettingsSystem.SaveSettings( hfs );

            if ( settings.InputFiles == null )
            {
                return;
            }

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

                ConsoleInInterface cin = new ConsoleInInterface();

                ConsoleOutInterface cout =
                    new ConsoleOutInterface();

                HostFileSystem hostFS = new HostFileSystem();
                BenchmarkDevice benchDev = new BenchmarkDevice();

                MemoryBus bus = mbs.CreateBus( cout, cin, hostFS, benchDev );

                CPU cpu = new CPU( bus, settings.CpuResetAddr, settings.CpuIntAddr );
                cpu.LoadBinary( fileCode );
                cpu.Run();
            }
        }

        #endregion

        #region Private

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
