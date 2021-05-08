using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using VisCPU.Utility;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Console.Core.Subsystems.FileSystemBuilder
{

    public class DriveImageFormatV1 : DriveImageFormat
    {
        private DriveImageFormatV1Settings m_Settings;

        public override string FormatName => "FSv1";

        public override string[] SupportedExtensions => new[] { ".zip", ".bin" };

        public override bool SupportsDirectoryInput => true;

        #region Public

        public override object[] GetSettingsObjects()
        {
            m_Settings = SettingsManager.GetSettings < DriveImageFormatV1Settings >();

            return new object[] { m_Settings };
        }

        public override void Pack( string input )
        {
            if ( Directory.Exists( input ) )
            {
                string cacheDir = Path.Combine( VisConsole.GetCacheDirectory( SubSystem ), "zip-temp" );
                Directory.CreateDirectory( cacheDir );
                string newFile = Path.Combine( cacheDir, Path.GetRandomFileName() + ".zip" );
                ZipFile.CreateFromDirectory( input, newFile );
                input = newFile;
            }
            else
            {
                Log( "Invalid Format: {0}", Path.GetExtension( input ) );

                return;
            }

            ZipArchive archive = ZipFile.OpenRead( input );
            string fullPath = Path.GetFullPath( input );

            string output = Path.Combine(
                Path.GetDirectoryName( fullPath ),
                Path.GetFileNameWithoutExtension( fullPath ) + ".bin"
            );

            FileStream fs = File.Create( output );

            if ( m_Settings.DiskSize != 0 )
            {
                fs.SetLength( m_Settings.DiskSize );
            }

            foreach ( ZipArchiveEntry file in archive.Entries )
            {
                string fsPath = m_Settings.FileSystemPrefix + file.FullName;

                Log( "File: '{0}' => '{1}'", file.FullName, fsPath );
                uint fileSize = m_Settings.UnpackData ? ( uint ) file.Length : ( uint ) file.Length / sizeof( uint );
                fs.Write( BitConverter.GetBytes( ( uint ) fsPath.Length ), 0, sizeof( uint ) );
                fs.Write( BitConverter.GetBytes( fileSize ), 0, sizeof( uint ) );
                fs.Write( BitConverter.GetBytes( 0U ), 0, sizeof( uint ) );

                byte[] name = fsPath.Select( x => ( uint ) x ).SelectMany( BitConverter.GetBytes ).ToArray();
                fs.Write( name, 0, name.Length );

                Stream s = file.Open();
                byte[] buffer = new byte[1024 * 1024];
                int read = buffer.Length;

                do
                {
                    read = s.Read( buffer, 0, buffer.Length );

                    if ( m_Settings.UnpackData &&
                         !m_Settings.UnpackIgnoreExtensions.Contains( Path.GetExtension( file.FullName ) ) )
                    {
                        byte[] tempBuffer = buffer.Take( read ).
                                                   Select( x => ( uint ) x ).
                                                   SelectMany( BitConverter.GetBytes ).
                                                   ToArray();

                        fs.Write( tempBuffer, 0, tempBuffer.Length );
                    }
                    else
                    {
                        fs.Write( buffer, 0, read );
                    }
                }
                while ( read == buffer.Length );

                s.Close();
            }

            fs.Close();
            archive.Dispose();
        }

        public override void Unpack( string input )
        {
            if ( !File.Exists( input ) )
            {
                Log( "Invalid Format: {0}", Path.GetExtension( input ) );

                return;
            }

            string fullPath = Path.GetFullPath( input );

            string output = Path.Combine(
                Path.GetDirectoryName( fullPath ),
                Path.GetFileNameWithoutExtension( fullPath ) + ".zip"
            );

            if ( File.Exists( output ) )
            {
                File.Delete( output );
            }

            ZipArchive ret = ZipFile.Open( output, ZipArchiveMode.Create );

            FileStream fs = File.OpenRead( input );
            long startPos;

            while ( true )
            {
                if ( fs.Position == fs.Length )
                {
                    break;
                }

                startPos = fs.Position;
                byte[] numBuffer = new byte[sizeof( uint ) * 3];
                fs.Read( numBuffer, 0, numBuffer.Length );
                uint strLen = BitConverter.ToUInt32( numBuffer, 0 ) * sizeof( uint );

                if ( strLen == 0 )
                {
                    break;
                }

                uint dataLen = BitConverter.ToUInt32( numBuffer, sizeof( uint ) ) * sizeof( uint );
                uint deleted = BitConverter.ToUInt32( numBuffer, sizeof( uint ) * 2 );

                if ( deleted != 0 )
                {
                    continue;
                }

                byte[] strBuffer = new byte[strLen];
                fs.Read( strBuffer, 0, strBuffer.Length );
                string name = new string( strBuffer.ToUInt().Select( x => ( char ) x ).ToArray() );

                ZipArchiveEntry e = ret.CreateEntry( name );
                Stream s = e.Open();
                byte[] buffer = new byte[1024 * 1024];
                int read = 0;

                do
                {
                    int r = fs.Read( buffer, 0, Math.Min( buffer.Length, ( int ) dataLen - read ) );

                    if ( r == 0 )
                    {
                        break;
                    }

                    read = +r;

                    s.Write( buffer, 0, read );
                }
                while ( read < dataLen );

                fs.Position = startPos + 3 * sizeof( uint ) + strLen + dataLen;

                s.Close();
            }

            fs.Close();
            ret.Dispose();
        }

        #endregion
    }

}
