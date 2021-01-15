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

        private static readonly Dictionary < string, BuildSteps > s_AllBuildSteps =
            new Dictionary < string, BuildSteps >
            {
                { "HL-expr", ( file, lastFile ) => CreateExpressionBuildStep( lastFile ) },
                { "bin", CreateBinary },
                { "compress", ( file, lastFile ) => CompressFile( lastFile ) }
            };

        [field: Argument( Name = "build:steps" )]
        public string[] Steps { get; set; } = { "bin" };

        [field: Argument( Name = "build:clean" )]
        public bool CleanBuildOutput { get; set; } = true;

        [field: Argument( Name = "build:input" )]
        [field: Argument( Name = "build:i" )]
        [XmlIgnore]
        [JsonIgnore]
        public string[] EntryFiles { get; set; }

        [field: Argument( Name = "build:input-dirs" )]
        [field: Argument( Name = "build:if" )]
        [ XmlIgnore]
        [ JsonIgnore]
        public string[] InputFolders { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable < (string, BuildSteps) > InstanceBuildSteps =>
            Steps.Select( x => ( x, s_AllBuildSteps[x] ) );

        [XmlIgnore]
        [JsonIgnore]
        public string[] InputFiles
        {
            get
            {
                List < string > ret = new List < string >();

                if ( InputFolders != null )
                {
                    ret.AddRange( InputFolders.SelectMany( x => Directory.GetFiles( x, "*.vasm" ) ) );
                    ret.AddRange( InputFolders.SelectMany( x => Directory.GetFiles( x, "*.vhl" ) ) );
                }

                if ( EntryFiles != null )
                {
                    ret.AddRange( EntryFiles );
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
