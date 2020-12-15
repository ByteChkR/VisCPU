using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU
{
    public class ReadFromUnmappedAddressEvent : WarningEvent
    {

        public const string EVENT_KEY = "mb-read-unmapped";
        public ReadFromUnmappedAddressEvent(uint address) : base($"Tried to read from address 0x{Convert.ToString(address, 16)} which is not mapped.", EVENT_KEY)
        {
        }

    }

    public class MultipleReceiverWriteEvent : WarningEvent
    {

        public const string EVENT_KEY = "mb-write-multiple";
        public MultipleReceiverWriteEvent(uint address) : base($"Multiple Overlapping Peripherals found at address 0x{address.ToHexString()}", EVENT_KEY)
        {
        }

    }

    public class WriteToUnmappedAddressEvent : WarningEvent
    {

        public const string EVENT_KEY = "mb-write-unmapped";
        public WriteToUnmappedAddressEvent(uint address, uint data) : base($"Tried to write value '0x{Convert.ToString(data, 16)}' to address 0x{Convert.ToString(address, 16)} which is not mapped.", EVENT_KEY)
        {
        }

    }
    
    public class MemoryBus : VisBase
    {

        private readonly List<Peripheral> peripherals;

        public MemoryBus() : this(new List<Peripheral>())
        {
        }

        public MemoryBus(IEnumerable<Peripheral> peripherals)
        {
            this.peripherals = peripherals.ToList();
        }

        public MemoryBus(params Peripheral[] peripherals)
        {
            this.peripherals = peripherals.ToList();
        }

        public void Reset()
        {
            peripherals.ForEach(x => x.Reset());
        }

        public void Write(uint address, uint data)
        {
            bool hasReceiver = false;
            foreach (Peripheral peripheral in peripherals.Where(x => x.CanWrite(address)))
            {
                hasReceiver = true;
                peripheral.WriteData(address, data);
            }

            if (!hasReceiver)
            {
                EventManager < WarningEvent >.SendEvent( new WriteToUnmappedAddressEvent( address, data ) );
            }

            Log($"W ADDR: {address.ToHexString()} VAL: {data.ToHexString()}");
        }

        public uint Read(uint address)
        {
            uint receivers = 0;
            uint data = 0;
            foreach (Peripheral peripheral in peripherals.Where(x => x.CanRead(address)))
            {
                if (receivers == 0)
                {
                    data = peripheral.ReadData(address);
                }

                receivers++;
            }

            if ( receivers == 0 )
                EventManager < WarningEvent >.SendEvent( new ReadFromUnmappedAddressEvent( address ) );
            else if ( receivers > 1)
                EventManager<WarningEvent>.SendEvent(new MultipleReceiverWriteEvent(address));


            Log($"R ADDR: 0x{address.ToHexString()} VAL: 0x{data.ToHexString()}");
            
            return data;
        }

        public void Dump()
        {
            for (int i = 0; i < peripherals.Count; i++)
            {
                Peripheral peripheral = peripherals[i];
                FileStream fs = File.Create(".\\crash.per_" + i + ".dump");
                peripheral.Dump(fs);
                fs.Close();
            }
        }

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryBus;

    }
}