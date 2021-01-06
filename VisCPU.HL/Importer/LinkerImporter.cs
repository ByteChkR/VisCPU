using System;
using System.IO;
using System.Linq;

using VisCPU.HL.Importer.Events;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer
{

    public class LinkerImporter : AImporter, IDataImporter
    {

        #region Public

        public override bool CanImport( string input )
        {
            return input.StartsWith( "link" );
        }

        public IExternalData[] ProcessImport( string input )
        {
            int tagLen = "link".Length + 1;

            if ( input.Length < tagLen )
            {
                EventManager < ErrorEvent >.SendEvent( new InvalidLinkImporterArgumentsEvent( input ) );

                return new IExternalData[0];
            }

            string cmd = input.Remove( 0, tagLen );
            uint offset = 0;

            if ( File.Exists( cmd ) )
            {
                Log( "Reading Offset from Binary: " + cmd );
                byte[] buf = new byte[sizeof( uint )];

                using ( FileStream fs = File.OpenRead( cmd ) )
                {
                    fs.Read( buf, 0, buf.Length );
                }

                offset = BitConverter.ToUInt32( buf, 0 );
                Log( "Detected Offset: " + offset );
            }
            else if ( cmd.StartsWith( "-" ) )
            {
                int end = cmd.IndexOf( ' ', 1 );
                offset = uint.Parse( cmd.Substring( 1, end - 1 ) );
                Log( "Parsed Offset: " + cmd );
                cmd = cmd.Substring( end + 1 );
            }
            else
            {
                EventManager < ErrorEvent >.SendEvent( new LinkImporterOffsetNotSpecifiedEvent( cmd ) );
            }

            LinkerInfo info = LinkerInfo.Load( cmd );

            return info.Labels.ApplyOffset( offset ).
                        Select( x => ( IExternalData ) new LinkedData( x.Key, x.Value ) ).
                        ToArray();
        }

        #endregion

    }

}
