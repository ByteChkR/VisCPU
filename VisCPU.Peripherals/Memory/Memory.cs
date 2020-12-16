using System;
using System.IO;

using VisCPU.Utility;
using VisCPU.Utility.Events;

namespace VisCPU.Peripherals
{
    
    
    
    public class Memory : Peripheral
    {

        public readonly bool EnableRead = true;

        public readonly bool EnableWrite = true;

        public readonly uint[] InternalMemory;

        public Memory(uint memorySize, uint startAddress, bool enableRead = true, bool enableWrite = true)
        {
            StartAddress = startAddress;
            InternalMemory = new uint[memorySize];
            EnableRead = enableRead;
            EnableWrite = enableWrite;
        }

        public uint StartAddress { get; set; }

        public uint EndAddress => StartAddress + (uint) InternalMemory.Length;

        public override bool CanWrite(uint address)
        {
            return EnableWrite && address < EndAddress && address >= StartAddress;
        }

        public override bool CanRead(uint address)
        {
            return EnableRead && address < EndAddress && address >= StartAddress;
        }

        public override void WriteData(uint address, uint data)
        {
            if (CanWrite(address))
            {
                InternalMemory[address - StartAddress] = data;
            }
        }

        public override uint ReadData(uint address)
        {
            if (CanWrite(address))
            {
                return InternalMemory[address - StartAddress];
            }

            return 0;
        }

        public override void Dump(Stream str)
        {
            str.Write(InternalMemory.ToBytes(), 0, InternalMemory.Length * sizeof(uint));
        }

    }
}