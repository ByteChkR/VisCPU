using VisCPU.Peripherals.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleOutInterface : Peripheral
    {

        private readonly ConsoleOutInterfaceSettings m_Settings;

        #region Public

        public ConsoleOutInterface()
        {
            m_Settings = SettingsSystem.GetSettings < ConsoleOutInterfaceSettings >();
        }

        public override bool CanRead( uint address )
        {
            return m_Settings.InterfacePresentPin == address;
        }

        public override bool CanWrite( uint address )
        {
            return address == m_Settings.WriteOutputAddress ||
                   m_Settings.WriteNumOutputAddress == address ||
                   m_Settings.InterfaceClearPin == address;
        }

        public override uint ReadData( uint address )
        {
            if ( address == m_Settings.InterfacePresentPin )
            {
                return 1;
            }

            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralReadEvent( address, this ) );

            return 0;
        }

        public override void WriteData( uint address, uint data )
        {
            if ( CanWrite( address ) )
            {
                if ( m_Settings.InterfaceClearPin == address )
                {
                    System.Console.Clear();
                }
                else if ( m_Settings.WriteNumOutputAddress == address )
                {
                    System.Console.Write( data.ToString() );
                }
                else
                {
                    System.Console.Write( ( char ) data );
                }
            }
        }

        #endregion

    }

}
