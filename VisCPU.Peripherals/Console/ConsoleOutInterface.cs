using VisCPU.Peripherals.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals.Console
{

    public class ConsoleOutInterface : Peripheral
    {

        private readonly ConsoleOutInterfaceSettings settings;

        #region Public

        public ConsoleOutInterface()
        {
            settings = SettingsSystem.GetSettings < ConsoleOutInterfaceSettings >();
        }

        public override bool CanRead( uint address )
        {
            return settings.InterfacePresentPin == address;
        }

        public override bool CanWrite( uint address )
        {
            return address == settings.WriteOutputAddress ||
                   settings.WriteNumOutputAddress == address ||
                   settings.InterfaceClearPin == address;
        }

        public override uint ReadData( uint address )
        {
            if ( address == settings.InterfacePresentPin )
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
                if ( settings.InterfaceClearPin == address )
                {
                    System.Console.Clear();
                }
                else if ( settings.WriteNumOutputAddress == address )
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
