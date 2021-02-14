using System;

using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Time
{

    public class TimeDevice : Peripheral
    {

        private readonly TimeDeviceSettings m_Settings;

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

        public override bool CanRead( uint address )
        {
            return m_Settings.PresentPin == address || m_Settings.TimePin == address;
        }

        public override bool CanWrite( uint address )
        {
            return false;
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.PresentPin )
            {
                return 1;
            }

            if ( address == m_Settings.TimePin )
            {
                DateTimeOffset o=DateTimeOffset.Now;
                if(!TimeZoneInfo.Local.IsDaylightSavingTime(o))
                    return (uint)DateTimeOffset.Now.ToUnixTimeSeconds() + 3600; //Add 1 hour because of Time Shift
                return (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
        }

        #endregion

    }

}
