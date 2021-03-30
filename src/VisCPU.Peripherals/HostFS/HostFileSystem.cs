using System;
using System.IO;
using System.Text;
using VisCPU.Peripherals.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.HostFS
{

    public class HostFileSystem : Peripheral
    {
        private HostFileSystemStatus m_Status = HostFileSystemStatus.HfsStatusReady;
        private readonly HostFileSystemSettings m_Settings;
        private readonly StringBuilder m_SbPath = new StringBuilder();
        private FileInfo m_CurrentFile;
        private FileStream m_CurrentFileStream;
        private bool m_ReadFileSize;
        private bool m_ReadFileExists;

        public override string PeripheralName => "Benchmark Device";

        public override PeripheralType PeripheralType => PeripheralType.Custom;

        public override uint PresentPin => m_Settings.PinPresent;

        #region Unity Event Functions

        public override void Reset()
        {
            m_CurrentFile = null;
            m_CurrentFileStream?.Dispose();
            m_CurrentFileStream = null;
            m_SbPath.Clear();
            m_ReadFileSize = false;
            m_ReadFileExists = false;
            m_Status = HostFileSystemStatus.HfsStatusReady;
        }

        #endregion

        #region Public

        public HostFileSystem( HostFileSystemSettings settings )
        {
            m_Settings = settings;
        }

        public HostFileSystem() : this( SettingsManager.GetSettings < HostFileSystemSettings >() )
        {
        }

        public override bool CanRead( uint address )
        {
            return address == m_Settings.PinData || address == m_Settings.PinPresent || address == m_Settings.PinStatus;
        }

        public override bool CanWrite( uint address )
        {
            return address == m_Settings.PinCmd || address == m_Settings.PinData;
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.PinPresent )
            {
                return 1;
            }

            if ( address == m_Settings.PinStatus )
            {
                return ( uint ) m_Status;
            }

            if ( address == m_Settings.PinData )
            {
                if ( m_ReadFileSize )
                {
                    m_ReadFileSize = false;

                    return ( uint ) m_CurrentFile.Length / sizeof( uint );
                }

                if ( m_ReadFileExists )
                {
                    m_ReadFileExists = false;

                    return ( uint ) ( m_CurrentFile.Exists ? 1 : 0 );
                }

                if ( m_CurrentFileStream.Length <= m_CurrentFileStream.Position )
                {
                    m_Status = HostFileSystemStatus.HfsStatusReady;
                    m_CurrentFileStream.Close();
                    m_CurrentFileStream = null;
                    m_CurrentFile = null;

                    return 0;
                }

                if ( m_Status == HostFileSystemStatus.HfsStatusFileOpen )
                {
                    byte[] buf = new byte[sizeof( uint )];
                    int read = m_CurrentFileStream.Read( buf, 0, sizeof( uint ) );

                    if ( read != sizeof( uint ) )
                    {
                        EventManager < ErrorEvent >.SendEvent(
                            new HostFileSystemReadFailureEvent(
                                "Did not read full uint size."
                            )
                        );
                    }

                    return BitConverter.ToUInt32( buf, 0 );
                }
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == m_Settings.PinData )
            {
                if ( m_Status == HostFileSystemStatus.HfsStatusFileOpen )
                {
                    m_CurrentFileStream.Write( BitConverter.GetBytes( data ), 0, sizeof( uint ) );
                }
                else if ( m_Status == HostFileSystemStatus.HfsStatusReady )
                {
                    char chr = ( char ) data;
                    m_SbPath.Append( chr );
                }
            }

            if ( address == m_Settings.PinCmd )
            {
                HostFileSystemCommands cmd = ( HostFileSystemCommands ) data;

                switch ( cmd )
                {
                    case HostFileSystemCommands.HfsDeviceReset:
                        m_Status = HostFileSystemStatus.HfsStatusReady;
                        m_SbPath.Clear();
                        m_CurrentFileStream?.Close();
                        m_CurrentFileStream = null;
                        m_CurrentFile = null;

                        break;

                    case HostFileSystemCommands.HfsOpenRead:
                        m_Status = HostFileSystemStatus.HfsStatusFileOpen;
                        string pathR = GetPath( m_SbPath.ToString() );
                        Log( "Opening File(READ): " + pathR );

                        if ( !File.Exists( pathR ) )
                        {
                            EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( pathR, false ) );
                        }
                        else
                        {
                            m_CurrentFileStream = File.OpenRead( pathR );
                            m_CurrentFile = new FileInfo( pathR );
                        }

                        m_SbPath.Clear();

                        break;

                    case HostFileSystemCommands.HfsOpenWrite:
                        m_Status = HostFileSystemStatus.HfsStatusFileOpen;
                        string pathW = GetPath( m_SbPath.ToString() );
                        Log( "Opening File(WRITE): " + pathW );

                        if ( !File.Exists( m_SbPath.ToString() ) )
                        {
                            m_CurrentFileStream = File.Create( pathW );
                            m_CurrentFile = new FileInfo( pathW );
                        }
                        else
                        {
                            m_CurrentFileStream = File.OpenWrite( pathW );
                            m_CurrentFile = new FileInfo( pathW );
                        }

                        m_SbPath.Clear();

                        break;

                    case HostFileSystemCommands.HfsClose:
                        Log( "Closing File: " + m_CurrentFile.FullName );
                        m_Status = HostFileSystemStatus.HfsStatusReady;
                        m_SbPath.Clear();
                        m_CurrentFileStream?.Close();
                        m_CurrentFileStream = null;
                        m_CurrentFile = null;

                        break;

                    case HostFileSystemCommands.HfsFileSize:
                        string p = GetPath( m_SbPath.ToString() );
                        m_CurrentFile = new FileInfo( p );
                        m_SbPath.Clear();
                        m_ReadFileSize = true;

                        break;

                    case HostFileSystemCommands.HfsFileExist:
                        string testFile = GetPath( m_SbPath.ToString() );
                        m_CurrentFile = new FileInfo( testFile );
                        m_SbPath.Clear();
                        m_ReadFileExists = true;

                        break;

                    case HostFileSystemCommands.HfsChangeDir:
                        string dir = GetPath( m_SbPath.ToString() );
                        Directory.SetCurrentDirectory( dir );
                        m_SbPath.Clear();

                        break;

                    case HostFileSystemCommands.HfsMakeDir:
                        string newDir = GetPath( m_SbPath.ToString() );
                        Directory.CreateDirectory( newDir );
                        m_SbPath.Clear();

                        break;

                    case HostFileSystemCommands.HfsFileDelete:
                        string targetFile = GetPath( m_SbPath.ToString() );
                        m_SbPath.Clear();

                        if ( !m_Settings.EnableDeleteFiles )
                        {
                            break;
                        }

                        File.Delete( targetFile );

                        break;

                    case HostFileSystemCommands.HfsLoadSymbols:
                        string target = GetPath( m_SbPath.ToString() );
                        m_SbPath.Clear();
                        AttachedCpu.SymbolServer.LoadSymbols( target );

                        break;

                    default:
                        EventManager < ErrorEvent >.SendEvent( new InvalidHfsCommandEvent( cmd ) );

                        break;
                }
            }
        }

        #endregion

        #region Private

        private string GetPath( string p )
        {
            if ( m_Settings.UseRootPath )
            {
                return Path.Combine( m_Settings.RootPath, p );
            }

            return p;
        }

        #endregion
    }

}
