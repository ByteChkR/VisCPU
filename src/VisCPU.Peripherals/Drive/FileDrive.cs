using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Drive
{

    public class FileDrive : Drive
    {
        private FileStream m_FileStream;

        public override string PeripheralName => "File Drive";

        #region Unity Event Functions

        public override void Reset()
        {
            m_FileStream.Close();
            FileDriveSettings ds = ( FileDriveSettings ) m_Settings;
            m_FileStream = File.Open( ds.FileDrive, FileMode.OpenOrCreate, FileAccess.ReadWrite );
        }

        #endregion

        #region Public

        public FileDrive() : this( SettingsManager.GetSettings < FileDriveSettings >() )
        {
        }

        public FileDrive( FileDriveSettings settings ) : base( settings )
        {
            m_FileStream = File.Open( settings.FileDrive, FileMode.OpenOrCreate, FileAccess.ReadWrite );
            m_FileStream.SetLength( settings.FileLength );
            Log( "File Drive is Present" );
        }

        public override uint GetSize()
        {
            return ( uint ) m_FileStream.Length / sizeof( uint );
        }

        public override uint Read( uint address )
        {
            m_FileStream.Position = address * sizeof( uint );
            byte[] d = new byte[sizeof( uint )];
            m_FileStream.Read( d, 0, sizeof( uint ) );

            return BitConverter.ToUInt32( d, 0 );
        }

        public override void ReadBuffer( uint address, uint destination, uint start, uint length )
        {
            IEnumerable < Memory.Memory > n = AttachedCpu.MemoryBus.GetPeripherals < Memory.Memory >();

            foreach ( Memory.Memory memory in n )
            {
                if ( memory.StartAddress <= destination &&
                     memory.EndAddress >= destination + destination )
                {
                    m_FileStream.Position = address * sizeof( uint );

                    byte[] buffer = new byte[length * sizeof( uint )];

                    m_FileStream.Read( buffer, 0, buffer.Length );

                    Array.Copy(
                        buffer.ToUInt(),
                        0,
                        memory.GetInternalBuffer(),
                        ( int ) ( destination - memory.StartAddress + start ),
                        length
                    );
                }
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_FileStream.Close();
        }

        public override void Write( uint address, uint data )
        {
            m_FileStream.Position = address * sizeof( uint );
            m_FileStream.Write( BitConverter.GetBytes( data ), 0, sizeof( uint ) );
        }

        public override void WriteBuffer( uint address, uint destination, uint start, uint length )
        {
            IEnumerable < Memory.Memory > n = AttachedCpu.MemoryBus.GetPeripherals < Memory.Memory >();

            foreach ( Memory.Memory memory in n )
            {
                if ( memory.StartAddress <= address &&
                     memory.EndAddress >= address + start + length )
                {
                    m_FileStream.Position = destination * sizeof( uint );

                    byte[] buffer = memory.GetInternalBuffer().
                                           Skip( ( int ) ( address - memory.StartAddress + start ) ).
                                           Take( ( int ) length ).
                                           SelectMany( BitConverter.GetBytes ).
                                           ToArray();

                    m_FileStream.Write( buffer, 0, buffer.Length );
                }
            }
        }

        #endregion
    }

}
