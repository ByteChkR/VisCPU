using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using VisCPU.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU
{

    public class MemoryBus : VisBase
    {

        private readonly List < Peripheral > peripherals;

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryBus;

        #region Unity Event Functions

        public void Reset()
        {
            peripherals.ForEach( x => x.Reset() );
        }

        #endregion

        #region Public

        public MemoryBus() : this( new List < Peripheral >() )
        {
        }

        public MemoryBus( IEnumerable < Peripheral > peripherals )
        {
            this.peripherals = peripherals.ToList();
        }

        public MemoryBus( params Peripheral[] peripherals )
        {
            this.peripherals = peripherals.ToList();
        }

        public void Dump()
        {
            for ( int i = 0; i < peripherals.Count; i++ )
            {
                Peripheral peripheral = peripherals[i];
                FileStream fs = File.Create( ".\\crash.per_" + i + ".dump" );
                peripheral.Dump( fs );
                fs.Close();
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Read( uint address )
        {
            uint receivers = 0;
            uint data = 0;

            foreach ( Peripheral peripheral in peripherals )
            {
                if ( !peripheral.CanRead( address ) )
                {
                    continue;
                }

                if ( receivers == 0 )
                {
                    data = peripheral.ReadData( address );
                }

                receivers++;
            }

            if ( CPUSettings.WarnOnUnmappedAccess && receivers == 0 )
            {
                EventManager < WarningEvent >.SendEvent( new ReadFromUnmappedAddressEvent( address ) );
            }
            else if ( receivers > 1 )
            {
                EventManager < WarningEvent >.SendEvent( new MultipleReceiverWriteEvent( address ) );
            }

            return data;
        }

        public void Shutdown()
        {
            peripherals.ForEach( x => x.Shutdown() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( uint address, uint data )
        {
            bool hasReceiver = false;

            foreach ( Peripheral peripheral in peripherals )
            {
                if ( !peripheral.CanWrite( address ) )
                {
                    continue;
                }

                hasReceiver = true;
                peripheral.WriteData( address, data );
            }

            if ( CPUSettings.WarnOnUnmappedAccess && !hasReceiver )
            {
                EventManager < WarningEvent >.SendEvent( new WriteToUnmappedAddressEvent( address, data ) );
            }
        }

        #endregion

    }

}
