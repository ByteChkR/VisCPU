using System;
using System.Collections.Generic;
using System.Text;

using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Time
{

    public class TimeDevice : Peripheral
    {
        private readonly TimeDeviceSettings m_Settings;

        public TimeDevice()
        {
            m_Settings = SettingsManager.GetSettings<TimeDeviceSettings>();
        }
        public TimeDevice(TimeDeviceSettings settings)
        {
            m_Settings = settings;
        }

        public override void Reset()
        {
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
                return ( uint )DateTimeOffset.Now.ToUnixTimeSeconds();
            }

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
        }

    }
}
