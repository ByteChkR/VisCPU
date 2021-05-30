using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using VisCPU.Events;
using VisCPU.Peripherals;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU
{

    public class MemoryBus : VisBase
    {

        private readonly List < Peripheral > m_Peripherals;
        private readonly CpuSettings m_Settings;

        public int PeripheralCount => m_Peripherals.Count;

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryBus;

        #region Unity Event Functions

        public void Reset()
        {
            m_Peripherals.ForEach( x => x.Reset() );
        }

        #endregion

        #region Public

        public MemoryBus() : this( new List < Peripheral >() { new MemoryBusDriver() } )
        {
        }

        public MemoryBus( IEnumerable < Peripheral > peripherals )
        {
            m_Peripherals = peripherals.ToList();

            if ( m_Peripherals.All( x => x.PeripheralType != PeripheralType.MemoryBusDriver ) )
            {
                m_Peripherals.Add( new MemoryBusDriver() );
            }

            m_Settings=  SettingsManager.GetSettings < CpuSettings >();
        }

        public MemoryBus( params Peripheral[] peripherals ): this((IEnumerable <Peripheral>)peripherals)
        {
            
        }

        public void Add( Peripheral p )
        {
            m_Peripherals.Add( p );
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

        public Peripheral GetPeripheralAt( int index )
        {
            return m_Peripherals[index];
        }

        public IEnumerable < T > GetPeripherals < T >() where T : Peripheral
        {
            return m_Peripherals.Where( x => x is T ).Cast < T >();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Read( uint address )
        {
            uint receivers = 0;
            uint data = 0;

            foreach ( Peripheral peripheral in m_Peripherals )
            {
                if (peripheral.AddressRangeEnd < address ||
                    peripheral.AddressRangeStart > address)
                {
                    continue;
                }

                if ( receivers == 0 )
                {
                    data = peripheral.ReadData( address );
                }

                receivers++;
            }

            if (receivers == 0 && m_Settings.WarnOnUnmappedAccess )
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
                if ( peripheral.AddressRangeEnd < address ||
                     peripheral.AddressRangeStart > address)
                {
                    continue;
                }

                hasReceiver = true;
                peripheral.WriteData( address, data );
            }

            if ( m_Settings.WarnOnUnmappedAccess && !hasReceiver )
            {
                EventManager < WarningEvent >.SendEvent( new WriteToUnmappedAddressEvent( address, data ) );
            }
        }

        internal void SetCpu( Cpu cpu )
        {
            m_Peripherals.ForEach( x => x.SetCpu( cpu ) );
        }

        #endregion

    }

}
