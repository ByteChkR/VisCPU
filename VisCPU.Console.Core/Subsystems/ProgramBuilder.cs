using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.HL.Importer;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

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
            HLCompilerSettings hls = SettingsManager.GetSettings < HLCompilerSettings >();

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
            HLCompilerSettings hls = SettingsManager.GetSettings < HLCompilerSettings >();

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

        public override void Help()
        {
            BuilderSettings settings = new BuilderSettings();
            AssemblyGeneratorSettings asettings = SettingsManager.GetSettings < AssemblyGeneratorSettings >();
            LinkerSettings ls = SettingsManager.GetSettings < LinkerSettings >();
            HLCompilerSettings hls = SettingsManager.GetSettings < HLCompilerSettings >();
            HelpSubSystem.WriteSubsystem( "vis build", settings, asettings, ls, hls );
        }

        public override void Run( IEnumerable < string > args )
        {
            Build( args );
        }

        #endregion

        #region Private

        private static void Build( BuilderSettings settings )
        {
            ImporterSystem.Add( new InstructionDataImporter(), new LinkerImporter() );

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
                    string newFile = step( original, file );

                    if ( settings.CleanBuildOutput && file != original )
                    {
                        File.Delete( file );
                    }

                    Logger.LogMessage(
                                      LoggerSystems.Console,
                                      $"Running Build Step '{stepName}' File: '{file}' => '{newFile}'"
                                     );

                    file = newFile;
                }

                Logger.LogMessage( LoggerSystems.Console, $"Steps Completed! File: '{original}' => '{file}'" );
            }
        }

        #endregion

    }

}
