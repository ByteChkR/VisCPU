using System;

using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Time
{

    public class TimeDevice : Peripheral
    {

        private readonly TimeDeviceSettings m_Settings;

        public override string PeripheralName => "Time Provider Device";

        public override PeripheralType PeripheralType => PeripheralType.Time;

        public override uint PresentPin => m_Settings.PresentPin;

        public override uint AddressRangeStart => m_Settings.PresentPin;
        public override uint AddressRangeEnd => m_Settings.TimePin;

        #region Unity Event Functions

        public override void Reset()
        {
        }

        #endregion

        #region Public

        public TimeDevice()
        {
            m_Settings = SettingsManager.GetSettings < TimeDeviceSettings >();
        }

        public TimeDevice( TimeDeviceSettings settings )
        {
            m_Settings = settings;
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.PresentPin )
            {
                return 1;
            }

            if ( address == m_Settings.TimePin )
            {
                DateTimeOffset o = DateTimeOffset.Now;

                if ( !TimeZoneInfo.Local.IsDaylightSavingTime( o ) )
                {
                    return ( uint ) ( DateTimeOffset.Now + DateTimeOffset.Now.Offset ).ToUnixTimeSeconds() +
                           3600; //Add 2 hour because of Time Shift
                }

                return ( uint ) ( DateTimeOffset.Now + DateTimeOffset.Now.Offset ).ToUnixTimeSeconds();
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
        }

        #endregion

    }

}
