﻿using VisCPU.Peripherals.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleInInterface : Peripheral
    {

        private readonly ConsoleInInterfaceSettings m_Settings;

        #region Public

        public ConsoleInInterface()
        {
            m_Settings = SettingsSystem.GetSettings < ConsoleInInterfaceSettings >();
        }

        public override bool CanRead( uint address )
        {
            return address == m_Settings.ReadInputAddress || address == m_Settings.InterfacePresentPin;
        }

        public override bool CanWrite( uint address )
        {
            return false;
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.InterfacePresentPin )
            {
                return 1;
            }

            if ( address == m_Settings.ReadInputAddress )
            {
                return ( uint ) System.Console.Read();
            }

            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralReadEvent( address, this ) );

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralWriteEvent( address, data, this ) );
        }

        #endregion

    }

}
