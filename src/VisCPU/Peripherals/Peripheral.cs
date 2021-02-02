using System.Collections.Generic;
using System.IO;

using VisCPU.Utility.Extensions;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Peripherals
{

    public abstract class Peripheral : VisBase
    {

        public static readonly SettingsCategory s_PeripheralCategory =
            CpuSettings.s_CpuCategory.AddCategory( "peripherals" );

        public static readonly SettingsCategory s_PeripheralExtensions =
            CpuSettings.s_CpuExtensionsCategory.AddCategory( "peripherals" );

        protected Cpu AttachedCpu { get; private set; }

        protected override LoggerSystems SubSystem => LoggerSystems.Peripherals;

        #region Unity Event Functions

        public abstract void Reset();

        #endregion

        #region Public

        public static IEnumerable < Peripheral > GetExtensionPeripherals()
        {
            return ExtensionLoader.LoadFrom < Peripheral >( s_PeripheralExtensions.GetCategoryDirectory(), true );
        }

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

        internal void SetCpu( Cpu cpu )
        {
            AttachedCpu = cpu;
        }

        #endregion

    }

}
