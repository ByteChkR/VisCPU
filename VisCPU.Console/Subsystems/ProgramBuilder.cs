using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU;
using VisCPU.Compiler.Linking;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace viscc
{

    internal class ProgramBuilder : ConsoleSubsystem
    {

        private readonly List < uint > ignored = new List < uint >();

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            BuilderSettings settings = BuilderSettings.Create();
            LinkerSettings ls = LinkerSettings.Create();

            ArgumentSyntaxParser.Parse(
                                       args.ToArray(),
                                       settings,
                                       ls
                                      );

            Settings.SaveSettings(ls);
            Settings.SaveSettings(settings);

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
                    string newFile = step( file );

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
