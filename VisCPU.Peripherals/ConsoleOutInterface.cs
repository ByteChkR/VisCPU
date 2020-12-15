using System;
using System.Text;

using VisCPU.Utility.Events;

namespace VisCPU.Peripherals
{

    public class InvalidPeripheralReadEvent : WarningEvent
    {

        private const string EVENT_KEY = "p_invalid_read";

        public InvalidPeripheralReadEvent(uint address, Peripheral peripheral) : base($"Can not read address '0x{Convert.ToString(address, 16)}' mapped to peripheral '{peripheral}'", EVENT_KEY)
        {
        }

    }

    public class InvalidPeripheralWriteEvent : WarningEvent
    {

        private const string EVENT_KEY = "p_invalid_write";

        public InvalidPeripheralWriteEvent(uint address, uint data, Peripheral peripheral) : base($"Can not write data '0x{Convert.ToString(data, 16)}' to address '0x{Convert.ToString(address, 16)}' mapped to peripheral '{peripheral}'", EVENT_KEY)
        {
        }

    }

    public class ConsoleOutInterface : Peripheral
    {

        private readonly uint ReadInputAddress;
        private readonly uint ReadNumInputAddress;

        public ConsoleOutInterface(uint readInputAddress, uint readNumInputAddress)
        {
            ReadInputAddress = readInputAddress;
            ReadNumInputAddress = readNumInputAddress;
        }

        public override bool CanWrite(uint address)
        {
            return address == ReadInputAddress || ReadNumInputAddress == address;
        }

        public override bool CanRead(uint address)
        {
            return false;
        }

        public override void WriteData(uint address, uint data)
        {
            if (CanWrite(address))
            {
                if (ReadNumInputAddress == address)
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

            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralReadEvent( address, this ) );

            return 0;
        }

    }
}