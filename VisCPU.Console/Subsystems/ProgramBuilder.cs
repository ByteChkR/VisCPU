using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using VisCPU;
using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace viscc
{

    internal class ProgramBuilder : ConsoleSubsystem
    {

        public class BuilderSettings
        {

            public static readonly Dictionary < string, BuildSteps > AllBuildSteps =
                new Dictionary < string, BuildSteps >
                {
                    { "HL-expr", CreateExpressionBuildStep },
                    { "bin", CreateBinary },
                    { "compress", CompressFile }
                };

            [Argument( Name = "clean" )]
            public bool CleanBuildOutput = true;

            [Argument( Name = "export-info" )]
            [Argument( Name = "export" )]
            public bool ExportLinkerInfo = false;

            [Argument( Name = "help" )]
            [Argument( Name = "h" )]
            public bool PrintHelp;

            [Argument( Name = "build-steps" )]
            [Argument( Name = "steps" )]
            private readonly string[] buildSteps = { "bin" };

            [Argument( Name = "input-files" )]
            [Argument( Name = "i" )]
            private string[] inputFiles;

            [Argument( Name = "input-folders" )]
            [Argument( Name = "if" )]
            private string[] inputFolders;

            public IEnumerable < (string, BuildSteps) > BuildSteps => buildSteps.Select( x => ( x, AllBuildSteps[x] ) );

            public string[] InputFiles
            {
                get
                {
                    List < string > ret = new List < string >();

                    if ( inputFolders != null )
                    {
                        ret.AddRange( inputFolders.SelectMany( x => Directory.GetFiles( x, "*.vasm" ) ) );
                    }

                    if ( inputFiles != null )
                    {
                        ret.AddRange( inputFiles );
                    }

                    return ret.ToArray();
                }
            }

            #region Private

            private static string CompressFile(string originalfile)
            {
                string newFile = originalfile + ".z";

                using Stream input = File.OpenRead(originalfile);
                
                using Stream output = File.Create(newFile);

                using Stream s = new GZipStream(output, CompressionLevel.Optimal);


                input.CopyTo(s);

                return newFile;
            }

            private static string CreateBinary( string originalFile )
            {
                if ( Path.GetExtension( originalFile ) != ".vasm" )
                {
                    EventManager < ErrorEvent >.SendEvent( new FileInvalidEvent( originalFile, true ) );

                    return originalFile;
                }

                Compilation comp = new Compilation( new MultiFileStaticLinker(), new DefaultAssemblyGenerator() );
                comp.Compile( originalFile );

                string newFile = Path.Combine(
                                              Path.GetDirectoryName( Path.GetFullPath( originalFile ) ),
                                              Path.GetFileNameWithoutExtension( originalFile )
                                             ) +
                                 ".vbin";

                if ( CPUSettings.ExportLinkerInfo )
                {
                    comp.LinkerInfo.Save( newFile, LinkerInfo.LinkerInfoFormat.Text );
                }

                File.WriteAllBytes( newFile, comp.ByteCode.ToArray() );

                return newFile;
            }

            private static string CreateExpressionBuildStep( string originalFile )
            {
                if ( Path.GetExtension( originalFile ) != ".vhl" )
                {
                    EventManager < ErrorEvent >.SendEvent( new FileInvalidEvent( originalFile, true ) );

                    return originalFile;
                }

                ExpressionParser p = new ExpressionParser ();
                string file = File.ReadAllText( originalFile );

                string newFile = Path.Combine(
                                              Path.GetDirectoryName( Path.GetFullPath( originalFile ) ),
                                              Path.GetFileNameWithoutExtension( originalFile )
                                             ) +
                                 ".vasm";

                File.WriteAllText(
                                  newFile,
                                  p.Parse( file, Path.GetDirectoryName( Path.GetFullPath( originalFile ) ) ).Parse()
                                 );

                return newFile;
            }

            #endregion

        }

        private readonly List < uint > ignored = new List < uint >();

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            BuilderSettings settings = new BuilderSettings();

            ArgumentSyntaxParser.Parse(
                                       args.ToArray(),
                                       settings
                                      );
            
            CPUSettings.ExportLinkerInfo = settings.ExportLinkerInfo;

            if ( settings.PrintHelp )
            {
                PrintHelp();
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

        #region Private

        private static void PrintHelp()
        {
            BuilderSettings s = new BuilderSettings();
            IEnumerable < string > args = ArgumentSyntaxParser.GetArgNames( s );

            foreach ( string s1 in args )
            {
                Console.WriteLine( "Arg Name: " + s1 );
            }


            Console.WriteLine("-log Subsystems: ");
            string[] names = Enum.GetNames < LoggerSystems >();

            foreach ( string name in names )
            {
                Console.WriteLine( "\t" + name );
            }
        }

        private void ConsoleDebugger( CPU cpu )
        {
            if ( ignored.Contains( cpu.ProgramCounter ) )
            {
                cpu.UnSet( CPU.Flags.BREAK );
            }

            Console.WriteLine();
            Console.Write( $"[0x{Convert.ToString( cpu.ProgramCounter, 16 )}]Breakpoint Hit :> " );
            string cmd = Console.ReadLine();

            if ( cmd == "ignore" )
            {
                if ( ignored.Contains( cpu.ProgramCounter ) )
                {
                    ignored.Remove( cpu.ProgramCounter );
                    Console.WriteLine( "UnIgnored" );
                }
                else
                {
                    ignored.Add( cpu.ProgramCounter );
                    Console.WriteLine( "Ignored" );
                }
            }

            cpu.UnSet( CPU.Flags.BREAK );
        }

        #endregion

    }

}
