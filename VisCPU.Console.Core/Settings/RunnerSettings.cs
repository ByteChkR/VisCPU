using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Settings
{

    public class RunnerSettings
    {
        [Argument(Name = "run:load-symbols")]
        public bool LoadDebugSymbols = true;

        [Argument(Name = "run:symbols")]
        [XmlIgnore]
        [JsonIgnore]
        public string[] AdditionalSymbols;

        [Argument( Name = "run:input" )]
        [Argument( Name = "run:i" )]
        [XmlIgnore]
        [JsonIgnore]
        private readonly string[] m_InputFiles;

        [Argument( Name = "run:input-dirs" )]
        [Argument( Name = "run:if" )]
        [XmlIgnore]
        [JsonIgnore]
        private readonly string[] m_InputFolders;

        [XmlIgnore]
        [JsonIgnore]
        public Dictionary < string, Func < string, string > > PreRunMap { get; } =
            new Dictionary < string, Func < string, string > >
            {
                { ".z", UnCompressFile }, { ".vasm", FindBinary }, { ".vhl", FindBinary }
            };

        [field: Argument( Name = "run:working-dir" )]
        [field: Argument( Name = "run:w-dir" )]
        [XmlIgnore]
        [JsonIgnore]
        public string WorkingDir { get; set; } = Path.GetFullPath( "./" );

        [XmlIgnore]
        [JsonIgnore]
        public string[] InputFiles
        {
            get
            {
                List < string > ret = new List < string >();

                if ( m_InputFolders != null )
                {
                    ret.AddRange( m_InputFolders.SelectMany( x => Directory.GetFiles( x, "*.txt" ) ) );
                }

                if ( m_InputFiles != null )
                {
                    ret.AddRange( m_InputFiles );
                }

                return ret.ToArray();
            }
        }

        #region Private

        private static string FindBinary( string arg )
        {
            string name = Path.Combine( Path.GetDirectoryName( arg ), Path.GetFileNameWithoutExtension( arg ) );
            string rawBin = name + ".vbin";
            string compBin = name + ".vbin.z";

            if ( File.Exists( rawBin ) )
            {
                return rawBin;
            }

            if ( File.Exists( compBin ) )
            {
                return compBin;
            }

            return null;
        }

        private static string UnCompressFile( string originalfile )
        {
            string newFile = originalfile.Remove( originalfile.Length - 2, 2 );

            using ( Stream input = File.OpenRead( originalfile ) )
            {
                using ( Stream output = File.Create( newFile ) )
                {
                    using ( Stream s = new GZipStream( input, CompressionMode.Decompress ) )
                    {
                        s.CopyTo( output );
                    }
                }
            }

            return newFile;
        }

        #endregion
    }

}
