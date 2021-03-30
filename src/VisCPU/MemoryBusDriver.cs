using VisCPU.Peripherals;

namespace VisCPU
{

    public class MemoryBusDriver : Peripheral
    {
        private enum AddressPins : uint
        {
            PeripheralPresent = PeripheralCount - 1,
            PeripheralCount = PeripheralType - 1,
            PeripheralType = PeripheralAddress - 1,
            PeripheralAddress = PeripheralName - 1,
            PeripheralName = PeripheralNameLength - 1,
            PeripheralNameLength = uint.MaxValue - 1
        }

        private uint m_PeripheralTypeStep;
        private uint m_PeripheralTypeId;

        private uint m_PeripheralAddressStep;
        private uint m_PeripheralAddressId;

        private uint m_PeripheralNameLengthStep;
        private uint m_PeripheralNameLengthId;

        private uint m_PeripheralNameStep;
        private uint m_PeripheralNameId;
        private uint m_PeripheralNameIndex;

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

        #region Unity Event Functions

        public override void Reset()
        {
        }

        #endregion

        #region Public

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
                   address == ( uint ) AddressPins.PeripheralNameLength;
        }

        public override uint ReadData( uint address )
        {
            if ( address == ( uint ) AddressPins.PeripheralCount )
            {
                return ( uint ) AttachedCpu.MemoryBus.PeripheralCount;
            }

            if ( address == PresentPin )
            {
                return 1;
            }

            if ( address == ( uint ) AddressPins.PeripheralNameLength )
            {
                if ( m_PeripheralNameLengthStep == 1 )
                {
                    m_PeripheralNameLengthStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt( ( int ) m_PeripheralNameLengthId );

                    return ( uint ) p.PeripheralName.Length;
                }
            }

            if ( address == ( uint ) AddressPins.PeripheralType )
            {
                if ( m_PeripheralTypeStep == 1 )
                {
                    m_PeripheralTypeStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt( ( int ) m_PeripheralTypeId );

                    return ( uint ) p.PeripheralType;
                }
            }
            else if ( address == ( uint ) AddressPins.PeripheralAddress )
            {
                if ( m_PeripheralAddressStep == 1 )
                {
                    m_PeripheralAddressStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt( ( int ) m_PeripheralAddressId );

                    return p.PresentPin;
                }
            }
            else if ( address == ( uint ) AddressPins.PeripheralName )
            {
                if ( m_PeripheralNameStep == 2 )
                {
                    m_PeripheralNameStep = 0;
                    Peripheral p = AttachedCpu.MemoryBus.GetPeripheralAt( ( int ) m_PeripheralNameId );

                    char u =  p.PeripheralName[( int ) m_PeripheralNameIndex];

                    return u;
                }
            }

            return 0;

        }

        public override void WriteData( uint address, uint data )
        {
            if ( address == ( uint ) AddressPins.PeripheralType )
            {
                if ( m_PeripheralTypeStep == 0 )
                {
                    m_PeripheralTypeStep++;
                    m_PeripheralTypeId = data;
                }
            }

            if ( address == ( uint ) AddressPins.PeripheralAddress )
            {
                if ( m_PeripheralAddressStep == 0 )
                {
                    m_PeripheralAddressStep++;
                    m_PeripheralAddressId = data;
                }
            }

            if ( address == ( uint ) AddressPins.PeripheralName )
            {
                if ( m_PeripheralNameStep == 0 )
                {
                    m_PeripheralNameStep++;
                    m_PeripheralNameId = data;
                }
                else if ( m_PeripheralNameStep == 1 )
                {
                    m_PeripheralNameStep++;
                    m_PeripheralNameIndex = data;
                }
            }

            if ( address == ( uint ) AddressPins.PeripheralNameLength )
            {
                if ( m_PeripheralNameLengthStep == 0 )
                {
                    m_PeripheralNameLengthStep++;
                    m_PeripheralNameLengthId = data;

                }
            }
        }

        #endregion
    }

}