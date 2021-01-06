﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VisCPU.Utility
{

    [Serializable]
    public struct LinkerInfo
    {

        private static bool IsAutoGenerated( object[] args )
        {
            return args.Contains( "linker:autogen" );
        }

        private static bool IsHidden( object[] args )
        {
            return args.Contains( "linker:hide" );
        }

        [Flags]
        public enum LinkerInfoFormat
        {

            Text = 1,
            XML = 2,
            IncludeSource = 4

        }

        public string Source;
        public Dictionary < string, AddressItem > Constants;
        public Dictionary < string, AddressItem > Labels;
        public Dictionary < string, AddressItem > DataSectionHeader;

        public static LinkerInfo CreateEmpty()
        {
            return new LinkerInfo
                   {
                       Constants = new Dictionary < string, AddressItem >(),
                       DataSectionHeader = new Dictionary < string, AddressItem >(),
                       Labels = new Dictionary < string, AddressItem >(),
                       Source = null
                   };
        }

        public void Save( string outputFile, LinkerInfoFormat format )
        {
            bool saveSource = ( format & LinkerInfoFormat.IncludeSource ) != 0;

            if ( ( format & LinkerInfoFormat.XML ) != 0 )
            {
                SaveXml( outputFile + ".linkerxml", saveSource );
            }
            else
            {
                SaveText( outputFile + ".linkertext", saveSource );
            }
        }

        public static LinkerInfo Load( string programFile )
        {
            if ( File.Exists( programFile + ".linkerxml" ) )
            {
                return LoadXml( programFile + ".linkerxml" );
            }

            if ( File.Exists( programFile + ".linkertext" ) )
            {
                return LoadText( programFile + ".linkertext" );
            }

            return CreateEmpty();
        }

        private static LinkerInfo LoadXml( string file )
        {
            XmlSerializer xs = new XmlSerializer( typeof( LinkerInfo ) );
            LinkerInfo li = CreateEmpty();

            using ( FileStream fs = File.Create( file ) )
            {
                li = ( LinkerInfo ) xs.Deserialize( fs );
            }

            return li;
        }

        private void SaveXml( string file, bool saveSource )
        {
            XmlSerializer xs = new XmlSerializer( typeof( LinkerInfo ) );
            string source = Source;

            if ( !saveSource )
            {
                Source = null;
            }

            using ( FileStream fs = File.Create( file ) )
            {
                xs.Serialize( fs, this );
            }

            Source = source;
        }

        private void SaveText( string file, bool saveSource )
        {
            StringBuilder sb = new StringBuilder( saveSource ? Source : "\nSourcecode was not included.\n" );
            sb.AppendLine( "$src-end" );
            sb.AppendLine( "$start-constants" );
            Serialize( sb, Constants );
            sb.AppendLine( "$start-labels" );
            Serialize( sb, Labels );
            sb.AppendLine( "$start-data-section" );
            Serialize( sb, DataSectionHeader );
            File.WriteAllText( file, sb.ToString() );
        }

        private static void Serialize( StringBuilder sb, Dictionary < string, AddressItem > data )
        {
            foreach ( KeyValuePair < string, AddressItem > keyValuePair in data )
            {
                sb.AppendLine( $"{keyValuePair.Key} : 0x{Convert.ToString( keyValuePair.Value.Address, 16 )}" );
            }
        }

        private static LinkerInfo LoadText( string file )
        {
            List < string > text = File.ReadAllLines( file ).ToList();
            int dataStart = text.IndexOf( "$src-end" ) + 1;

            if ( dataStart == 0 )
            {
                return CreateEmpty();
            }

            int constStart = text.IndexOf( "$start-constants" ) + 1;
            int labelStart = text.IndexOf( "$start-labels" ) + 1;
            int dataSectionStart = text.IndexOf( "$start-data-section" ) + 1;
            LinkerInfo info = new LinkerInfo();

            if ( constStart >= dataStart )
            {
                int i = constStart;

                for ( ; i < text.Count; i++ )
                {
                    if ( text[i].StartsWith( "$" ) )
                    {
                        break;
                    }
                }

                info.Constants = DeserializeBlock( text.GetRange( constStart, i - constStart ) );
            }

            if ( labelStart >= dataStart )
            {
                int i = labelStart;

                for ( ; i < text.Count; i++ )
                {
                    if ( text[i].StartsWith( "$" ) )
                    {
                        break;
                    }
                }

                info.Labels = DeserializeBlock( text.GetRange( labelStart, i - labelStart ) );
            }

            if ( dataSectionStart >= dataStart )
            {
                int i = dataSectionStart;

                for ( ; i < text.Count; i++ )
                {
                    if ( text[i].StartsWith( "$" ) )
                    {
                        break;
                    }
                }

                info.DataSectionHeader = DeserializeBlock( text.GetRange( dataSectionStart, i - dataSectionStart ) );
            }

            return info;
        }

        private static Dictionary < string, AddressItem > DeserializeBlock( List < string > lines )
        {
            Dictionary < string, AddressItem > ret = new Dictionary < string, AddressItem >();

            foreach ( string line in lines )
            {
                string[] kvp = line.Split( ':' );

                ret[kvp[0].Trim()] = new AddressItem
                                     {
                                         Address = uint.Parse( kvp[1].Trim().Remove( 0, 2 ), NumberStyles.HexNumber ),
                                         LinkerArguments = new object[0]
                                     };
            }

            return ret;
        }

    }

}