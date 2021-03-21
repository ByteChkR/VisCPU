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

    public class MemoryBusDriver : Peripheral
    {
        //Peripheral Info
        // Name
        // Device Type
        // Address Start
        //BusDriver
        // GetPeripheralCount()
        // GetPeripheralNameChar(uint peripheralId, uint charId)
        // GetPeripheralType()
        // GetPeripheralAddress()

        public override string PeripheralName => "Default Memory Bus Driver";
        public override PeripheralType PeripheralType => PeripheralType.MemoryBusDriver;

        public override uint PresentPin => ( uint ) AddressPins.PeripheralPresent;

        private uint m_PeripheralTypeStep;
        private uint m_PeripheralTypeId;

        private uint m_PeripheralAddressStep;
        private uint m_PeripheralAddressId;

        private uint m_PeripheralNameLengthStep;
        private uint m_PeripheralNameLengthId;

        private uint m_PeripheralNameStep;
        private uint m_PeripheralNameId;
        private uint m_PeripheralNameIndex;

        private enum AddressPins : uint
        {
            PeripheralPresent = PeripheralCount - 1,
            PeripheralCount = PeripheralType - 1,
            PeripheralType = PeripheralAddress - 1,
            PeripheralAddress =PeripheralName-1,
            PeripheralName = PeripheralNameLength - 1,
            PeripheralNameLength = uint.MaxValue - 1
        }

        public override void Reset()
        {
        }

        public override bool CanRead( uint address )
        {
            return address == PresentPin ||
                   address == ( uint ) AddressPins.PeripheralCount ||
                   address == ( uint ) AddressPins.PeripheralAddress ||
                   address == ( uint ) AddressPins.PeripheralType ||
                   address == ( uint ) AddressPins.PeripheralName ||
                   address == ( uint ) AddressPins.PeripheralNameLength;
        }   

        public override bool CanWrite( uint address )
        {
            return address == ( uint ) AddressPins.PeripheralAddress ||
                   address == ( uint ) AddressPins.PeripheralType ||
                   address == ( uint ) AddressPins.PeripheralName ||
                   address == (uint)AddressPins.PeripheralNameLength;
        }

        public override uint ReadData( uint address )
        {
            if (address == (uint)AddressPins.PeripheralCount)
                return (uint)AttachedCpu.MemoryBus.PeripheralCount;

            if (address == PresentPin)
                return 1;

            if ( address == ( uint ) AddressPins.PeripheralNameLength )
            {
                if ( m_PeripheralNameLengthStep == 1 )
                {
                    m_PeripheralNameLengthStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt((int)m_PeripheralNameLengthId);

                    return ( uint ) p.PeripheralName.Length;
                }
            }

            if (address == (uint)AddressPins.PeripheralType)
            {
                if (m_PeripheralTypeStep == 1)
                {
                    m_PeripheralTypeStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt((int)m_PeripheralTypeId);

                    return ( uint ) p.PeripheralType;
                }
            }
            else if (address == (uint)AddressPins.PeripheralAddress)
            {
                if (m_PeripheralAddressStep == 1)
                {
                    m_PeripheralAddressStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt((int)m_PeripheralAddressId);

                    return p.PresentPin;
                }
            }
            else if (address == (uint)AddressPins.PeripheralName)
            {
                if (m_PeripheralNameStep == 2)
                {
                    m_PeripheralNameStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt((int)m_PeripheralNameId);

                    return p.PeripheralName[( int ) m_PeripheralNameIndex];
                }
            }

            return 0;

        }

        public override void WriteData( uint address, uint data )
        {
            if (address == (uint)AddressPins.PeripheralType)
            {
                if (m_PeripheralTypeStep == 0)
                {
                    m_PeripheralTypeStep++;
                    m_PeripheralTypeId = data;
                }
            }
            if (address == (uint)AddressPins.PeripheralAddress)
            {
                if (m_PeripheralAddressStep == 0)
                {
                    m_PeripheralAddressStep++;
                    m_PeripheralAddressId = data;
                }
            }
            if (address == (uint)AddressPins.PeripheralName)
            {
                if (m_PeripheralNameStep == 0)
                {
                    m_PeripheralNameStep++;
                    m_PeripheralNameId = data;
                }
                else if (m_PeripheralNameStep == 1)
                {
                    m_PeripheralNameStep++;
                    m_PeripheralNameIndex = data;
                }
            }
            if (address == (uint)AddressPins.PeripheralNameLength)
            {
                if (m_PeripheralNameLengthStep == 1)
                {
                    m_PeripheralNameLengthStep++;
                    m_PeripheralNameLengthId = data;

                }
            }
        }

    }

    public class MemoryBus : VisBase
    {

        private readonly List < Peripheral > m_Peripherals;

        public Peripheral GetPeripheralAt( int index ) => m_Peripherals[index];

        public int PeripheralCount => m_Peripherals.Count;

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryBus;

        #region Unity Event Functions

        public void Reset()
        {
            m_Peripherals.ForEach( x => x.Reset() );
        }

        #endregion

        #region Public

        public MemoryBus() : this( new List < Peripheral >(){new MemoryBusDriver()} )
        {
        }

        public MemoryBus( IEnumerable < Peripheral > peripherals )
        {
            m_Peripherals = peripherals.ToList();

            if ( m_Peripherals.All( x => x.PeripheralType != PeripheralType.MemoryBusDriver ) )
            {
                m_Peripherals.Add( new MemoryBusDriver() );
            }
        }

        public MemoryBus( params Peripheral[] peripherals )
        {
            m_Peripherals = peripherals.ToList();
            if (m_Peripherals.All(x => x.PeripheralType != PeripheralType.MemoryBusDriver))
            {
                m_Peripherals.Add(new MemoryBusDriver());
            }
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

            if ( SettingsManager.GetSettings < CpuSettings >().WarnOnUnmappedAccess && receivers == 0 )
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

            if ( SettingsManager.GetSettings < CpuSettings >().WarnOnUnmappedAccess && !hasReceiver )
            {
                EventManager < WarningEvent >.SendEvent( new WriteToUnmappedAddressEvent( address, data ) );
            }
        }

        public IEnumerable < T > GetPeripherals < T >() where T : Peripheral =>
            m_Peripherals.Where( x => x is T ).Cast<T>();

        internal void SetCpu( Cpu cpu )
        {
            m_Peripherals.ForEach( x => x.SetCpu( cpu ) );
        }

        #endregion

    }

}
