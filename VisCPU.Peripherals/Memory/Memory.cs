using System.IO;

using VisCPU.Peripherals.Console;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Memory
{
    public class Memory : Peripheral
    {

        private MemorySettings settings;

        public readonly uint[] InternalMemory;
        private readonly string fullPersistentPath;

        public uint EndAddress => settings.Start + ( uint ) InternalMemory.Length;

        #region Public

        public Memory() :this(MemorySettings.Create()) { }
        
        public Memory(MemorySettings settings)
        {
            this.settings = settings;

            if ( settings.Persistent && !string.IsNullOrEmpty(settings.PersistentPath))
            {
                fullPersistentPath = Path.GetFullPath(settings.PersistentPath);
                Directory.CreateDirectory( Path.GetDirectoryName( fullPersistentPath ) );
                if ( File.Exists( settings.PersistentPath ) )
                {
                    InternalMemory = File.ReadAllBytes( settings.PersistentPath ).ToUInt();

                    if ( InternalMemory.Length != settings.Size )
                    {
                        EventManager<ErrorEvent>.SendEvent(new FileInvalidEvent(settings.PersistentPath, true));
                        //Persistent File not correct size.
                    }
                }
                else
                {
                    EventManager <ErrorEvent>.SendEvent(new FileNotFoundEvent(settings.PersistentPath, true));
                    InternalMemory = new uint[settings.Size];
                    //File does not exist
                }
            }
            else
            {
                if (settings.Persistent)
                {
                    EventManager<WarningEvent>.SendEvent(new MemoryPersistentPathUnsetEvent());
                }
                InternalMemory = new uint[settings.Size];
                //Not persistent or persistent path not set
            }
        }

        public override void Shutdown()
        {
            if ( settings.Persistent && fullPersistentPath != null )
            {
                File.WriteAllBytes( fullPersistentPath, InternalMemory.ToBytes() );
            }
        }

        public override bool CanRead( uint address )
        {
            return settings.EnableRead && address < EndAddress && address >= settings.Start;
        }

        public override bool CanWrite( uint address )
        {
            return settings.EnableWrite && address < EndAddress && address >= settings.Start;
        }

        public override void Dump( Stream str )
        {
            str.Write( InternalMemory.ToBytes(), 0, InternalMemory.Length * sizeof( uint ) );
        }

        public override uint ReadData( uint address )
        {
            if ( CanWrite( address ) )
            {
                return InternalMemory[address - settings.Start];
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( CanWrite( address ) )
            {
                InternalMemory[address - settings.Start] = data;
            }
        }
        
        

        #endregion

    }

}
