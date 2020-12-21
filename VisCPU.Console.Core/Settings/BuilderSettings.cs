using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
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

        [Argument( Name = "build-steps" )]
        [Argument( Name = "steps" )]
        public readonly string[] buildSteps = { "bin" };

        [Argument( Name = "input-files" )]
        [Argument( Name = "i" )]
        [XmlIgnore]
        [JsonIgnore]
        public string[] inputFiles;

        [Argument( Name = "input-folders" )]
        [Argument( Name = "if" )]
        [XmlIgnore]
        [JsonIgnore]
        public string[] inputFolders;

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable < (string, BuildSteps) > BuildSteps => buildSteps.Select( x => ( x, AllBuildSteps[x] ) );

        [XmlIgnore]
        [JsonIgnore]
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

        #region Public

        public static BuilderSettings Create()
        {
            return Utility.Settings.Settings.GetSettings < BuilderSettings >();
        }

        #endregion

        #region Private

        static BuilderSettings()
        {
            
            Utility.Settings.Settings.RegisterDefaultLoader( new JSONSettingsLoader(), Path.Combine(
                                                 AppDomain.CurrentDomain.BaseDirectory,
                                                 "config/build.json"
                                                ), new BuilderSettings() );
        }

        private static string CompressFile( string originalfile )
        {
            string newFile = originalfile + ".z";

            using ( Stream input = File.OpenRead( originalfile ) )
            {
                using ( Stream output = File.Create( newFile ) )
                {
                    using ( Stream s = new GZipStream( output, CompressionLevel.Optimal ) )
                    {
                        input.CopyTo( s );
                    }
                }
            }

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

            if ( Utility.Settings.Settings.GetSettings < LinkerSettings >().ExportLinkerInfo )
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

            ExpressionParser p = new ExpressionParser();
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

}
