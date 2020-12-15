using System;

using VisCPU.Utility.Events;

namespace VisCPU.Peripherals
{
    public class ConsoleInInterface : Peripheral
    {

        private readonly uint ReadInputAddress;

        public ConsoleInInterface(uint readInputAddress)
        {
            ReadInputAddress = readInputAddress;
        }

        public override bool CanWrite(uint address)
        {
            return false;
        }

        public override bool CanRead(uint address)
        {
            return address == ReadInputAddress;
        }

        public override void WriteData(uint address, uint data)
        {
            EventManager < WarningEvent >.SendEvent( new InvalidPeripheralWriteEvent( address, data, this ) );
        }

        public override uint ReadData(uint address)
        {
            if (CanRead(address))
            {
                return (uint) Console.Read();
            }

            return 0;
        }

    }
}