﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.Instructions;
using VisCPU.Peripherals;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Console.Core.Subsystems
{

    public class ProgramRunner : ConsoleSubsystem
    {

        #region Public

        public static Cpu Create(
            IEnumerable < string > args,
            CpuSettings cpuSettings = null,
            RunnerSettings settings = null,
            HlCompilerSettings hls = null,
            MemorySettings ms = null,
            MemoryBusSettings mbs = null )
        {
            Logger.LogMessage( LoggerSystems.Console, "Creating CPU..." );
            cpuSettings = cpuSettings ?? SettingsManager.GetSettings < CpuSettings >();
            settings = settings ?? new RunnerSettings();
            hls = hls ?? SettingsManager.GetSettings < HlCompilerSettings >();
            ms = ms ?? SettingsManager.GetSettings < MemorySettings >();
            mbs = mbs ?? SettingsManager.GetSettings < MemoryBusSettings >();

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
                Logger.LogMessage( LoggerSystems.Console, "No Files Specified" );

                return null;
            }

            MemoryBus bus = CreateBus( mbs );

            Cpu cpu = new Cpu( bus, cpuSettings.CpuResetAddr, cpuSettings.CpuIntAddr );

            return cpu;
        }

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

                if ( settings.TrimMemoryToProgram )
                {
                    ms.Size = ( uint ) fileCode.Length;
                    SettingsManager.SaveSettings( ms );
                }

                MemoryBus bus = CreateBus( mbs );

                Cpu cpu = new Cpu( bus, cpuSettings.CpuResetAddr, cpuSettings.CpuIntAddr );

                if ( settings.LoadDebugSymbols )
                {
                    cpu.SymbolServer.LoadSymbols( file );

                    if ( settings.AdditionalSymbols != null )
                    {
                        foreach ( string symPath in settings.AdditionalSymbols )
                        {
                            cpu.SymbolServer.LoadSymbols( symPath );
                        }
                    }

                    cpu.SetInterruptHandler( InterruptHandler );
                }

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

                if ( settings.TrimMemoryToProgram )
                {
                    ms.Size = ( uint ) fileCode.Length;
                    SettingsManager.SaveSettings( ms );
                }

                MemoryBus bus = CreateBus( mbs );

                Cpu cpu = new Cpu( bus, cpuSettings.CpuResetAddr, cpuSettings.CpuIntAddr );

                if ( settings.LoadDebugSymbols )
                {
                    cpu.SymbolServer.LoadSymbols( file );

                    if ( settings.AdditionalSymbols != null )
                    {
                        foreach ( string symPath in settings.AdditionalSymbols )
                        {
                            cpu.SymbolServer.LoadSymbols( symPath );
                        }
                    }

                    cpu.SetInterruptHandler( InterruptHandler );
                }

                cpu.LoadBinary( fileCode );
                cpu.Run();
            }

            Directory.SetCurrentDirectory( origPath );
        }

        #endregion

        #region Private

        private static MemoryBus CreateBus( MemoryBusSettings settings, params Peripheral[] additionalPeripherals )
        {
            List < Peripheral > ps = settings.MemoryDevices.Select(
                                                                   x => new Memory(
                                                                        SettingsManager.
                                                                            GetSettings < MemorySettings >(
                                                                                 x
                                                                                )
                                                                       )
                                                                  ).
                                              Concat( additionalPeripherals ).
                                              Concat( Peripheral.GetExtensionPeripherals() ).
                                              ToList();

            foreach ( PeripheralImporter peripheralImporter in Peripheral.GetPeripheralImporters() )
            {
                ps.AddRange( peripheralImporter.GetPeripherals( ps ) );
            }

            return new MemoryBus(
                                 ps
                                );
        }

        private static void InterruptHandler( Cpu cpu, uint code )
        {
            List < (string Key, AddressItem Value) > mergedInfo = new List < (string, AddressItem) >();

            foreach ( LinkerInfo linkerInfo in cpu.SymbolServer.LoadedSymbols )
            {
                foreach ( KeyValuePair < string, AddressItem > linkerInfoLabel in linkerInfo.Labels )
                {
                    mergedInfo.Add( ( linkerInfoLabel.Key, linkerInfoLabel.Value ) );
                }
            }

            if ( code != 1 )
            {
                Logger.LogMessage( LoggerSystems.StackTrace, "Interrupt Fired." );
            }
            else
            {
                Logger.LogMessage( LoggerSystems.StackTrace, "FATAL ERROR!" );
            }

            IEnumerable < uint > stackStates = cpu.GetCpuStates();
            int stackNum = cpu.StackDepth - 1;

            foreach ( uint pc in stackStates )
            {
                uint callingInstruction = cpu.MemoryBus.Read( pc );

                Instruction instr = CpuSettings.InstructionSet.GetInstruction( callingInstruction );
                uint callee = 0;

                if ( instr.Key == "JMP" || instr.Key == "JSR" )
                {
                    callee = cpu.MemoryBus.Read( pc + 1 );
                }
                else if ( instr.Key == "JSREF" )
                {
                    callee = cpu.MemoryBus.Read( cpu.MemoryBus.Read( pc + 1 ) );
                }

                if ( callee != 0 )
                {
                    (string Key, AddressItem Value) item =
                        mergedInfo.FirstOrDefault( x => x.Value.Address == callee );

                    if ( item.Key == null )
                    {
                        Logger.LogMessage(
                                          LoggerSystems.StackTrace,
                                          "\t[Element {1}] Function at address: {0} was not exported",
                                          callee.ToHexString(),
                                          stackNum
                                         );
                    }
                    else

                    {
                        Logger.LogMessage( LoggerSystems.StackTrace, "\t[Element {1}] {0}", item.Key, stackNum );
                    }
                }

                stackNum--;
            }

            cpu.UnSet( Cpu.Flags.Interrupt );

            if ( code == 1 ) //No Crash, Just Log and Continue
            {
                cpu.Set( Cpu.Flags.Halt );
            }
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
