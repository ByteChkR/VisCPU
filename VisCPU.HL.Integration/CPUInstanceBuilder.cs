using System;
using System.Collections.Generic;
using System.Linq;
using VisCPU;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer;
using VisCPU.Peripherals;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

public class CPUInstanceBuilder
{
    private readonly APIImporter m_Importer;
    private readonly List<Peripheral> m_Peripherals;
    private readonly uint m_InterruptAddr;
    private readonly uint m_ResetAddr;
    private List<APIImporterDevice> m_ApiDevs = new List<APIImporterDevice>();
    private uint m_NextApiAddr = 0xFFFFAAAA;
    
    public CPUInstanceBuilder WithPeripherals(params Peripheral[] peripherals)
    {
        m_Peripherals.AddRange(peripherals);
        return this;
    }

    public CPUInstanceBuilder WithExposedAPI(Func<CPU, uint> api, string name, int argC)
    { 
        m_Importer.AddApi(m_NextApiAddr, new FunctionData(name, false, null, argC, true));

        m_ApiDevs.Add(new APIImporterDevice(m_NextApiAddr, api));
        m_NextApiAddr++;

        return this;
    }

    public CPU Build()
    {
        MemoryBus bus = new MemoryBus(m_Peripherals.Concat(m_ApiDevs));
        CPU cpu = new CPU(bus, m_ResetAddr, m_InterruptAddr);
        m_ApiDevs.ForEach(x=>x.SetExecutingCPU(cpu));
        return cpu;
    }

    public CPUInstanceBuilder(uint resetAddr = 0, uint intAddr = 0)
    {

        m_Importer = new APIImporter();
        ImporterSystem.Add(m_Importer);
        m_ResetAddr = resetAddr;
        m_InterruptAddr = intAddr;
        m_Peripherals = new List<Peripheral>();
    }
}
