using System;
using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer;
using VisCPU.Peripherals;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Integration
{

    public class CpuInstanceBuilder
    {
        private readonly ApiImporter m_Importer;
        private readonly List < Peripheral > m_Peripherals;
        private readonly uint m_InterruptAddr;
        private readonly uint m_ResetAddr;
        private readonly List < ApiImporterDevice > m_ApiDevs = new List < ApiImporterDevice >();
        private uint m_NextApiAddr = 0xFFFFAAAA;

        #region Public

        public CpuInstanceBuilder() : this( 0 )
        {
        }

        public CpuInstanceBuilder( uint resetAddr ) : this( resetAddr, 0 )
        {
        }

        public CpuInstanceBuilder( uint resetAddr, uint intAddr )
        {

            m_Importer = new ApiImporter();
            ImporterSystem.Add( m_Importer );
            m_ResetAddr = resetAddr;
            m_InterruptAddr = intAddr;
            m_Peripherals = new List < Peripheral >();
        }

        public Cpu Build()
        {
            MemoryBus bus = new MemoryBus( m_Peripherals.Concat( m_ApiDevs ) );
            Cpu cpu = new Cpu( bus, m_ResetAddr, m_InterruptAddr );
            m_ApiDevs.ForEach( x => x.SetExecutingCpu( cpu ) );

            return cpu;
        }

        public CpuInstanceBuilder WithExposedApi( Func < Cpu, uint > api, string name, int argC )
        {
            m_Importer.AddApi( m_NextApiAddr, new FunctionData( name, false, null, argC, HLBaseTypeNames.s_UintTypeName ) );

            m_ApiDevs.Add( new ApiImporterDevice( m_NextApiAddr, api ) );
            m_NextApiAddr++;

            return this;
        }

        public CpuInstanceBuilder WithPeripherals( params Peripheral[] peripherals )
        {
            m_Peripherals.AddRange( peripherals );

            return this;
        }

        #endregion
    }

}
