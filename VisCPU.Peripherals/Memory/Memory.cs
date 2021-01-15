using System.IO;

using VisCPU.Peripherals.Events;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Memory
{

    public class Memory : Peripheral
    {

        private readonly uint[] m_InternalMemory;
        private readonly string m_FullPersistentPath;

        private readonly MemorySettings m_Settings;

        public uint EndAddress => m_Settings.Start + ( uint ) m_InternalMemory.Length;

        #region Public

        public Memory() : this( MemorySettings.Create() )
        {
        }

        public Memory( MemorySettings settings )
        {
            m_Settings = settings;

            if ( settings.Persistent && !string.IsNullOrEmpty( settings.PersistentPath ) )
            {
                m_FullPersistentPath = Path.GetFullPath( settings.PersistentPath );
                Directory.CreateDirectory( Path.GetDirectoryName( m_FullPersistentPath ) );

                if ( File.Exists( settings.PersistentPath ) )
                {
                    m_InternalMemory = File.ReadAllBytes( settings.PersistentPath ).ToUInt();

                    if ( m_InternalMemory.Length != settings.Size )
                    {
                        EventManager < ErrorEvent >.SendEvent( new FileInvalidEvent( settings.PersistentPath, true ) );

                        //Persistent File not correct size.
                    }
                }
                else
                {
                    EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( settings.PersistentPath, true ) );
                    m_InternalMemory = new uint[settings.Size];

                    //File does not exist
                }
            }
            else
            {
                if ( settings.Persistent )
                {
                    EventManager < WarningEvent >.SendEvent( new MemoryPersistentPathUnsetEvent() );
                }

                m_InternalMemory = new uint[settings.Size];

                //Not persistent or persistent path not set
            }
        }

        public override bool CanRead( uint address )
        {
            return m_Settings.EnableRead && address < EndAddress && address >= m_Settings.Start;
        }

        public override bool CanWrite( uint address )
        {
            return m_Settings.EnableWrite && address < EndAddress && address >= m_Settings.Start;
        }

        public override void Dump( Stream str )
        {
            str.Write( m_InternalMemory.ToBytes(), 0, m_InternalMemory.Length * sizeof( uint ) );
        }

        public override uint ReadData( uint address )
        {
            return m_InternalMemory[address - m_Settings.Start];
        }

        public override void Shutdown()
        {
            if ( m_Settings.Persistent && m_FullPersistentPath != null )
            {
                File.WriteAllBytes( m_FullPersistentPath, m_InternalMemory.ToBytes() );
            }
        }

        public override void WriteData( uint address, uint data )
        {
            m_InternalMemory[address - m_Settings.Start] = data;
        }

        #endregion

    }

}
