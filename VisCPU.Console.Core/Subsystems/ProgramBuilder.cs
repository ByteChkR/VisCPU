using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Console.Core.Settings;
using VisCPU.HL.Importer;
using VisCPU.Instructions;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems
{

    public class ProgramBuilder : ConsoleSubsystem
    {

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            BuilderSettings settings = BuilderSettings.Create();
            AssemblyGeneratorSettings asettings = AssemblyGeneratorSettings.Create();

            LinkerSettings ls = LinkerSettings.Create();

            ArgumentSyntaxParser.Parse(
                                       args.ToArray(),
                                       settings,
                                       asettings,
                                       ls
                                      );

            SettingsSystem.SaveSettings( ls );
            SettingsSystem.SaveSettings( settings );
            SettingsSystem.SaveSettings( asettings );

            ImporterSystem.Add( new InstructionDataImporter( new DefaultSet() ), new LinkerImporter() );

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

                foreach ( ( string stepName, BuildSteps step ) in settings.BuildSteps )
                {
                    string newFile = step( original, file );

                    if ( settings.CleanBuildOutput && file != original )
                    {
                        File.Delete( file );
                    }

                    Log( $"Running Build Step '{stepName}' File: '{file}' => '{newFile}'" );
                    file = newFile;
                }

                Log( $"Steps Completed! File: '{original}' => '{file}'" );
            }
        }

        #endregion

    }

}
