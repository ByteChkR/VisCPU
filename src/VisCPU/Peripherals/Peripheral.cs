using System.Collections.Generic;
using System.IO;
using VisCPU.Extensions;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Peripherals
{

    public abstract class Peripheral : VisBase
    {
        protected Cpu AttachedCpu { get; private set; }

        internal void SetCpu( Cpu cpu )
        {
            AttachedCpu = cpu;
        }

        public static readonly SettingsCategory s_PeripheralCategory =
            CpuSettings.s_CpuCategory.AddCategory( "peripherals" );

        public static readonly SettingsCategory s_PeripheralExtensions =
            CpuSettings.s_CpuExtensionsCategory.AddCategory( "peripherals" );

        protected override LoggerSystems SubSystem => LoggerSystems.Peripherals;


        public static IEnumerable < Peripheral > GetExtensionPeripherals() =>
            ExtensionLoader.LoadFrom < Peripheral >(s_PeripheralExtensions.GetCategoryDirectory(), true );


        #region Unity Event Functions

        public abstract void Reset();

        #endregion

        #region Public

        public abstract bool CanRead( uint address );

        public abstract bool CanWrite( uint address );

        public abstract uint ReadData( uint address );

        public abstract void WriteData( uint address, uint data );

        public virtual void Dump( Stream str )
        {
        }

        public virtual void Shutdown()
        {
        }

        #endregion
    }

}
