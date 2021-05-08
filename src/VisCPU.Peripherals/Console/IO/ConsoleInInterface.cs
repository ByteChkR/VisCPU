using System;
using VisCPU.Peripherals.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.Peripherals.Console.IO
{

    public class ConsoleInInterface : Peripheral
    {
        private readonly ConsoleInInterfaceSettings m_Settings;

        public override string PeripheralName => "Console Input Device";

        public override PeripheralType PeripheralType => PeripheralType.ConsoleInput;

        public override uint PresentPin => m_Settings.InterfacePresentPin;

        public Func < bool > HasConsoleInput { get; set; } = () => System.Console.KeyAvailable;

        public Func < int > ReadConsoleInput { get; set; } = System.Console.Read;

        #region Unity Event Functions

        public override void Reset()
        {
        }

        #endregion

        #region Public

        public ConsoleInInterface()
        {
            m_Settings = SettingsManager.GetSettings < ConsoleInInterfaceSettings >();
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
                return 1u;
            }

            if ( address == m_Settings.InputAvailableAddress )
            {
                return HasConsoleInput() ? 1u : 0u;
            }

            if ( address == m_Settings.ReadInputAddress )
            {
                return ( uint ) ReadConsoleInput();
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
