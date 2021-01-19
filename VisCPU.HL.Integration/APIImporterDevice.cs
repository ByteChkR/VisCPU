using System;
using VisCPU;
using VisCPU.Peripherals;

public class APIImporterDevice : Peripheral
{
    private CPU ExecutingCPU;
    private uint ListenAddr;
    private Func<CPU, uint> InvokeExec;

    public void SetExecutingCPU(CPU executingCPU)
    {
        ExecutingCPU = executingCPU;
    }

    public APIImporterDevice(uint addr, Func<CPU, uint> invokeExec)
    {
        ListenAddr = addr;
        InvokeExec = invokeExec;
    }

    public override bool CanRead(uint address)
    {
        return address == ListenAddr;
    }

    public override uint ReadData(uint address)
    {
        ExecutingCPU.Push(InvokeExec(ExecutingCPU));

        return CPUSettings.InstructionSet.GetInstruction(CPUSettings.InstructionSet.GetInstruction("RET", 0));
    }

    public override bool CanWrite(uint address)
    {
        return address == ListenAddr;
    }

    public override void WriteData(uint address, uint data)
    {
    }
}