using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

using Newtonsoft.Json;

using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Implementations;
using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.IO;
using VisCPU.Utility.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Console.Core.Settings
{

    public class BuilderSettings
    {

        public static readonly Dictionary < string, BuildSteps > AllBuildSteps =
            new Dictionary < string, BuildSteps >
            {
                { "HL-expr", ( file, lastFile ) => CreateExpressionBuildStep( lastFile ) },
                { "bin", CreateBinary },
                { "compress", ( file, lastFile ) => CompressFile( lastFile ) }
            };

        [Argument( Name = "build:steps" )]
        public readonly string[] buildSteps = { "bin" };

        [Argument( Name = "build:clean" )]
        public bool CleanBuildOutput = true;

        [Argument( Name = "build:input" )]
        [Argument( Name = "build:i" )]
        [XmlIgnore]
        [JsonIgnore]
        public string[] inputFiles;

        [Argument( Name = "build:input-dirs" )]
        [Argument( Name = "build:if" )]
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
            return SettingsSystem.GetSettings < BuilderSettings >();
        }

        #endregion

        #region Private

        static BuilderSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "config/build.json"
                                                             ),
                                                 new BuilderSettings()
                                                );
        }

        private static string CompressFile( string lastStepFile )
        {
            string newFile = lastStepFile + ".z";

            using ( Stream input = File.OpenRead( lastStepFile ) )
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

        private static string CreateBinary( string originalFile, string lastStepFile )
        {
            if ( Path.GetExtension( lastStepFile ) != ".vasm" )
            {
                EventManager < ErrorEvent >.SendEvent( new FileInvalidEvent( lastStepFile, true ) );

                return lastStepFile;
            }

            Compilation comp = new Compilation( new MultiFileStaticLinker(), new DefaultAssemblyGenerator() );
            comp.Compile( lastStepFile );

            string newFile = Path.Combine(
                                          Path.GetDirectoryName( Path.GetFullPath( originalFile ) ),
                                          Path.GetFileNameWithoutExtension( originalFile )
                                         ) +
                             ".vbin";

            if ( SettingsSystem.GetSettings < LinkerSettings >().ExportLinkerInfo )
            {
                comp.LinkerInfo.Save( newFile, LinkerInfo.LinkerInfoFormat.Text );
            }

            File.WriteAllBytes( newFile, comp.ByteCode.ToArray() );

            return newFile;
        }

        private static string CreateExpressionBuildStep( string lastStepFile )
        {
            if ( Path.GetExtension( lastStepFile ) != ".vhl" )
            {
                EventManager < ErrorEvent >.SendEvent( new FileInvalidEvent( lastStepFile, true ) );

                return lastStepFile;
            }

            ExpressionParser p = new ExpressionParser();
            string file = File.ReadAllText( lastStepFile );

            BuildDataStore ds = new BuildDataStore(
                                                   Path.GetDirectoryName( Path.GetFullPath( lastStepFile ) ),
                                                   new HLBuildDataStore()
                                                  );

            string newFile = ds.GetStorePath(
                                             "HL2VASM",
                                             Path.GetFileNameWithoutExtension( Path.GetFullPath( lastStepFile ) )
                                            );

            File.WriteAllText(
                              newFile,
                              p.Parse( file, Path.GetDirectoryName( Path.GetFullPath( lastStepFile ) ), ds ).Parse()
                             );

            return newFile;
        }

        #endregion

    }

}
