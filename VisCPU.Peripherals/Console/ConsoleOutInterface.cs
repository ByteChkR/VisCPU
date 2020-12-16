using System;
using System.Text;

using VisCPU.Utility.Events;
using VisCPU.Utility.Settings;

namespace VisCPU.Peripherals
{

    public class ConsoleOutInterface : Peripheral
    {
        
        private ConsoleOutInterfaceSettings settings;
        
        public ConsoleOutInterface()
        {
            settings = Settings.GetSettings<ConsoleOutInterfaceSettings>();
        }

        public override bool CanWrite(uint address)
        {
            return address == settings.WriteOutputAddress || settings.WriteNumOutputAddress == address;
        }

        public override bool CanRead(uint address)
        {
            return settings.InterfacePresentPin==address;
        }

        public override void WriteData(uint address, uint data)
        {
            if (CanWrite(address))
            {
                if (settings.WriteNumOutputAddress == address)
                {
                    Console.Write(data.ToString());
                }
                else
                {
                    Console.Write((char) data);
                }
            }
        }

        public override uint ReadData(uint address)
        {
            if ( address == settings.InterfacePresentPin )
                return 1;
            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralReadEvent( address, this ) );

            return 0;
        }

    }
}