﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
{
    public class RunnerSettings
    {

        [Argument( Name = "cpu.interrupt" )]
        public uint CpuIntAddr = 0x00000000;

        [Argument( Name = "cpu.reset" )]
        public uint CpuResetAddr = 0x00000000;

        [XmlIgnore]
        [JsonIgnore]
        public Dictionary < string, Func < string, string > > PreRunMap =
            new Dictionary < string, Func < string, string > >
            {
                { ".z", UnCompressFile },
                { ".vasm", FindBinary },
                { ".vhl", FindBinary }
            };

        [Argument( Name = "input-files" )]
        [Argument( Name = "i" )]
        [XmlIgnore]
        [JsonIgnore]
        private string[] inputFiles;

        [Argument( Name = "input-folders" )]
        [Argument( Name = "if" )]
        [XmlIgnore]
        [JsonIgnore]
        private string[] inputFolders;

        [XmlIgnore]
        [JsonIgnore]
        public string[] InputFiles
        {
            get
            {
                List < string > ret = new List < string >();

                if ( inputFolders != null )
                {
                    ret.AddRange( inputFolders.SelectMany( x => Directory.GetFiles( x, "*.txt" ) ) );
                }

                if ( inputFiles != null )
                {
                    ret.AddRange( inputFiles );
                }

                return ret.ToArray();
            }
        }

        #region Public

        public static RunnerSettings Create()
        {
            return Utility.Settings.SettingsSystem.GetSettings < RunnerSettings >();
        }

        #endregion

        #region Private

        static RunnerSettings()
        {
            Utility.Settings.SettingsSystem.RegisterDefaultLoader( new JSONSettingsLoader(), Path.Combine(
                                                 AppDomain.CurrentDomain.BaseDirectory,
                                                 "config/runner.json"
                                                ), new RunnerSettings() );
        }

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
