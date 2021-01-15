using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectPackSubSystem : ConsoleSubsystem
    {

        public class PackOptions
        {

            [Argument( Name = "version" )]
            public string VersionString = "X.X.X.+";

        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static Version ChangeVersion( Version version, string changeStr )
        {
            string[] subVersions = changeStr.Split( '.' );
            int[] wrapValues = { ushort.MaxValue, 9, 99, ushort.MaxValue };
            int[] versions = { version.Major, version.Minor, version.Build, version.Revision };

            for ( int i = 4 - 1; i >= 0; i-- )
            {
                string current = subVersions[i];

                if ( current.StartsWith( "(" ) )
                {
                    if ( i == 0 )
                    {
                        continue; //Can not wrap the last digit
                    }

                    int j = 0;

                    for ( ; j < current.Length; j++ )
                    {
                        if ( current[j] == ')' )
                        {
                            break;
                        }
                    }

                    if ( j == current.Length )
                    {
                        continue; //Broken. No number left. better ignore
                    }

                    string max = current.Substring( 1, j - 1 );

                    if ( int.TryParse( max, out int newMax ) )
                    {
                        wrapValues[i] = newMax;
                    }

                    current = current.Remove( 0, j + 1 );
                }

                if ( i != 0 ) //Check if we wrapped
                {
                    if ( versions[i] >= wrapValues[i] )
                    {
                        versions[i] = 0;
                        versions[i - 1]++;
                    }
                }

                if ( current == "+" )
                {
                    versions[i]++;
                }
                else if ( current == "-" && versions[i] != 0 )
                {
                    versions[i]--;
                }
                else if ( current.ToLower( CultureInfo.InvariantCulture ) == "x" )
                {
                }
                else if ( current.StartsWith( "{" ) && current.EndsWith( "}" ) )
                {
                    string format = current.Remove( current.Length - 1, 1 ).Remove( 0, 1 );

                    string value = DateTime.Now.ToString( format );

                    if ( long.TryParse( value, out long newValue ) )
                    {
                        versions[i] = ( int ) ( newValue % ushort.MaxValue );
                    }
                }
                else if ( int.TryParse( current, out int v ) )
                {
                    versions[i] = v;
                }
            }

            return new Version(
                               versions[0],
                               versions[1] < 0 ? 0 : versions[1],
                               versions[2] < 0 ? 0 : versions[2],
                               versions[3] < 0 ? 0 : versions[3]
                              );
        }

        public static void CopyTo( string src, string dst )
        {
            foreach ( string dirPath in Directory.GetDirectories(
                                                                 src,
                                                                 "*",
                                                                 SearchOption.AllDirectories
                                                                ) )
            {
                Directory.CreateDirectory( dirPath.Replace( src, dst ) );
            }

            foreach ( string newPath in Directory.GetFiles(
                                                           src,
                                                           "*.*",
                                                           SearchOption.AllDirectories
                                                          ) )
            {
                File.Copy( newPath, newPath.Replace( src, dst ), true );
            }
        }

        public static void Pack( string projectRoot, PackOptions options )
        {
            string src = Path.Combine( Path.GetFullPath( projectRoot ), "project.json" );
            ProjectCleanSubSystem.Clean( Path.GetDirectoryName( src ) );
            Logger.LogMessage( LoggerSystems.ModuleSystem, "Packing '{0}'", src );

            string outDir = Path.Combine( Path.GetDirectoryName( src ), "build" );

            ProjectConfig t = ProjectConfig.Load( src );

            Version v = Version.Parse( t.ProjectVersion );

            t.ProjectVersion = ChangeVersion( v, options.VersionString ).ToString();

            string temp = Path.Combine(
                                       Path.GetDirectoryName( Directory.GetCurrentDirectory() ),
                                       "temp_" + t.ProjectName
                                      );

            Directory.CreateDirectory( temp );

            CopyTo( Path.GetDirectoryName( src ), temp );

            foreach ( ProjectDependency moduleDependency in t.Dependencies )
            {
                string p = Path.Combine( temp, moduleDependency.ProjectName );

                if ( Directory.Exists( p ) )
                {
                    Directory.Delete( p, true );
                }
            }

            Directory.CreateDirectory( outDir );
            ZipFile.CreateFromDirectory( temp, Path.Combine( outDir, "module.zip" ) );
            Directory.Delete( temp, true );
            ProjectConfig.Save( Path.Combine( outDir, "module.json" ), t );
            ProjectConfig.Save( src, t );
        }

        public static void WriteHelp()
        {
            HelpSubSystem.WriteSubsystem( "vis project pack", new PackOptions() );
        }

        public override void Help()
        {
            WriteHelp();
        }

        public override void Run( IEnumerable < string > args )
        {
            PackOptions op = new PackOptions();
            ArgumentSyntaxParser.Parse( args.Skip( 1 ).ToArray(), op );
            Pack( args.First(), op );
        }

        #endregion

    }

}
