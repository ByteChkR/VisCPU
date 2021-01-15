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

        private readonly List < Peripheral > m_Peripherals;

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryBus;

        #region Unity Event Functions

        public void Reset()
        {
            m_Peripherals.ForEach( x => x.Reset() );
        }

        #endregion

        #region Public

        public MemoryBus() : this( new List < Peripheral >() )
        {
        }

        public MemoryBus( IEnumerable < Peripheral > peripherals )
        {
            m_Peripherals = peripherals.ToList();
        }

        public MemoryBus( params Peripheral[] peripherals )
        {
            m_Peripherals = peripherals.ToList();
        }

        public void Dump()
        {
            for ( int i = 0; i < m_Peripherals.Count; i++ )
            {
                Peripheral peripheral = m_Peripherals[i];
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

            foreach ( Peripheral peripheral in m_Peripherals )
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
            m_Peripherals.ForEach( x => x.Shutdown() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( uint address, uint data )
        {
            bool hasReceiver = false;

            foreach ( Peripheral peripheral in m_Peripherals )
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
