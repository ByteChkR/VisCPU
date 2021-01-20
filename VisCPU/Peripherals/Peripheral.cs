using System.Collections.Generic;
using System.IO;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Peripherals
{

    public abstract class Peripheral : VisBase
    {
        public static readonly SettingsCategory s_PeripheralCategory =
            CPUSettings.s_CpuCategory.AddCategory( "peripherals" );

        public static readonly SettingsCategory s_PeripheralExtensions =
            CPUSettings.s_CpuExtensionsCategory.AddCategory( "peripherals" );

        protected override LoggerSystems SubSystem => LoggerSystems.Peripherals;

#if DEBUG
        public static Peripheral[] DebugPeripherals;
        public static IEnumerable < Peripheral > GetExtensionPeripherals()
        {
            return DebugPeripherals;
        }
#else
        public static IEnumerable < Peripheral > GetExtensionPeripherals() =>
            ExtensionLoader.LoadFrom < Peripheral >(s_PeripheralExtensions.GetCategoryDirectory(), true );
#endif

        #region Unity Event Functions

        public virtual void Reset()
        {
        }

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
