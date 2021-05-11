using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCL.Integration;
using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.HL.Importer;
using VisCPU.HL.TextLoader;
using VisCPU.Peripherals;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems
{

    public class ProgramBuilder : ConsoleSubsystem
    {
        #region Public

        public static void Build( Dictionary < string, string > args )
        {
            BuilderSettings settings = new BuilderSettings();
            AssemblyGeneratorSettings asettings = SettingsManager.GetSettings < AssemblyGeneratorSettings >();
            LinkerSettings ls = SettingsManager.GetSettings < LinkerSettings >();
            HlCompilerSettings hls = SettingsManager.GetSettings < HlCompilerSettings >();

            ArgumentSyntaxParser.Parse(
                args,
                settings,
                asettings,
                ls,
                hls
            );
            

            SettingsManager.SaveSettings( ls );
            SettingsManager.SaveSettings( asettings );
            SettingsManager.SaveSettings( hls );
            Build( settings );
        }

        public static void Build( IEnumerable < string > args )
        {
            BuilderSettings settings = new BuilderSettings();
            AssemblyGeneratorSettings asettings = SettingsManager.GetSettings < AssemblyGeneratorSettings >();
            LinkerSettings ls = SettingsManager.GetSettings < LinkerSettings >();
            HlCompilerSettings hls = SettingsManager.GetSettings < HlCompilerSettings >();

            ArgumentSyntaxParser.Parse(
                args.ToArray(),
                settings,
                asettings,
                ls,
                hls
            );

            SettingsManager.SaveSettings( ls );
            SettingsManager.SaveSettings( asettings );
            SettingsManager.SaveSettings( hls );
            Build( settings );
        }

        public static void Build( BuilderSettings settings )
        {
            TextImporter.ParseImporterArgs();
            ImporterSystem.Add( new InstructionDataImporter(), new LinkerImporter(), new OpenCLKernelImporter() );

            foreach ( PeripheralImporter peripheralImporter in Peripheral.GetPeripheralImporters() )
            {
                //Pretend we are starting the CPU to register all external Importers.
                peripheralImporter.GetPeripherals( new List < Peripheral >() );
            }

            if ( settings.InputFiles == null )
            {
                return;
            }

            foreach ( string f in settings.InputFiles )
            {
                string original = Path.GetFullPath( f );
                string file = original;

                if ( !File.Exists( file ) )
                {
                    EventManager < ErrorEvent >.SendEvent(
                        new FileNotFoundEvent( Path.GetFullPath( file ), true )
                    );

                    continue;
                }

                foreach ( ( string stepName, BuildSteps step ) in settings.InstanceBuildSteps )
                {
                    Logger.LogMessage(
                        LoggerSystems.Console,
                        $"Running Build Step '{stepName}'"
                    );

                    string newFile = step( original, file );

                    if ( settings.CleanBuildOutput && file != original )
                    {
                        File.Delete( file );
                    }

                    Logger.LogMessage(
                        LoggerSystems.Console,
                        $"'{file}' => '{newFile}'"
                    );

                    file = newFile;
                }

                Logger.LogMessage( LoggerSystems.Console, $"Steps Completed! File: '{original}' => '{file}'" );
            }
        }

        public override void Help()
        {
            BuilderSettings settings = new BuilderSettings();
            AssemblyGeneratorSettings asettings = SettingsManager.GetSettings < AssemblyGeneratorSettings >();
            LinkerSettings ls = SettingsManager.GetSettings < LinkerSettings >();
            HlCompilerSettings hls = SettingsManager.GetSettings < HlCompilerSettings >();
            HelpSubSystem.WriteSubsystem( "vis build", settings, asettings, ls, hls );
        }

        public override void Run( IEnumerable < string > args )
        {
            Build( args );
        }

        #endregion
    }

}
