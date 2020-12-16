using System;


using VisCPU.Utility.Events;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals
{

    public class ConsoleInInterface : Peripheral
    {

        private ConsoleInInterfaceSettings settings;
        
        public ConsoleInInterface()
        {
            settings = Settings.GetSettings < ConsoleInInterfaceSettings >();
        }

        public override bool CanWrite(uint address)
        {
            return false;
        }

        public override bool CanRead(uint address)
        {
            return address == settings.ReadInputAddress || address == settings.InterfacePresentPin;
        }

        public override void WriteData(uint address, uint data)
        {
            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralWriteEvent( address, data, this ) );
        }

        public override uint ReadData(uint address)
        {
            if ( address == settings.InterfacePresentPin )
                return 1;
            if (address == settings.ReadInputAddress)
            {
                return (uint) Console.Read();
            }
            
            EventManager<WarningEvent>.SendEvent(new InvalidPeripheralReadEvent(address, this));

            return 0;
        }

    }
}