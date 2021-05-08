using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.FileSystemBuilder
{

    public class DriveImageSubsystem : ConsoleSubsystem
    {
        private static readonly List < DriveImageFormat > s_ImageFormats =
            new List < DriveImageFormat > { new DriveImageFormatV1() };

        protected override LoggerSystems SubSystem => LoggerSystems.DriveImageSystems;

        #region Public

        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            if ( a.FirstOrDefault() == "unpack" )
            {
                Unpack( a.Skip( 1 ).ToArray() );
            }
            else if ( a.FirstOrDefault() == "pack" )
            {
                Pack( a.Skip( 1 ).ToArray() );
            }
        }

        #endregion

        #region Private

        private static string GetOrLastFormat( string[] array, int i )
        {
            return array.Length > i ? array[i] : array.LastOrDefault() ?? "FSv1";
        }

        private void Pack( string[] args )
        {
            string[] a = args.ToArray();
            DriveImageBuilderSettings settings = SettingsManager.GetSettings < DriveImageBuilderSettings >();
            ArgumentSyntaxParser.Parse( a, settings );

            for ( int i = 0; i < settings.InputFiles.Length; i++ )
            {
                string inputFile = settings.InputFiles[i];

                string format = GetOrLastFormat( settings.ImageFormats, i );
                DriveImageFormat f = s_ImageFormats.FirstOrDefault( x => x.FormatName == format );

                if ( f == null )
                {
                    Log( "Format '{0}' does not exist!", format );

                    continue;
                }

                ArgumentSyntaxParser.Parse( a, f.GetSettingsObjects() );

                bool isDir = Directory.Exists( inputFile );

                if ( f.SupportsDirectoryInput && isDir )
                {
                    f.Pack( inputFile );
                }
                else
                {
                    string ext = Path.GetExtension( inputFile );

                    if ( f.SupportedExtensions.Contains( ext ) )
                    {
                        if ( !File.Exists( inputFile ) )
                        {
                            Log( "File '{0}' does not exist!", inputFile );

                            continue;
                        }

                        f.Pack( inputFile );
                    }
                    else
                    {
                        Log( "Invalid Extension '{1}' for format '{0}'", f.FormatName, Path.GetExtension( inputFile ) );

                        continue;
                    }
                }
            }
        }

        private void Unpack( string[] args )
        {
            string[] a = args.ToArray();
            DriveImageBuilderSettings settings = SettingsManager.GetSettings < DriveImageBuilderSettings >();
            ArgumentSyntaxParser.Parse( a, settings );

            for ( int i = 0; i < settings.InputFiles.Length; i++ )
            {
                string inputFile = settings.InputFiles[i];

                string format = GetOrLastFormat( settings.ImageFormats, i );
                DriveImageFormat f = s_ImageFormats.FirstOrDefault( x => x.FormatName == format );

                if ( f == null )
                {
                    Log( "Format '{0}' does not exist!", format );

                    continue;
                }

                ArgumentSyntaxParser.Parse( a, f.GetSettingsObjects() );

                bool isDir = Directory.Exists( inputFile );

                if ( f.SupportsDirectoryInput && isDir )
                {
                    f.Unpack( inputFile );
                }
                else
                {
                    string ext = Path.GetExtension( inputFile );

                    if ( f.SupportedExtensions.Contains( ext ) )
                    {
                        if ( !File.Exists( inputFile ) )
                        {
                            Log( "File '{0}' does not exist!", inputFile );

                            continue;
                        }

                        f.Unpack( inputFile );
                    }
                    else
                    {
                        Log( "Invalid Extension '{1}' for format '{0}'", f.FormatName, Path.GetExtension( inputFile ) );

                        continue;
                    }
                }
            }
        }

        #endregion
    }

}
