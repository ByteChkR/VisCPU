using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Utility.Extensions;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Peripherals
{

    public abstract class Peripheral : VisBase
    {

        public bool ContainsInRange( uint addr ) => AddressRangeStart <= addr && AddressRangeEnd >= addr;

        public static readonly List < PeripheralImporter > DefaultPeripheralImporters =
            new List < PeripheralImporter >();

        public static SettingsCategory PeripheralCategory => CpuSettings.CpuCategory.AddCategory( "peripherals" );

        public static SettingsCategory PeripheralExtensions =>
            CpuSettings.CpuExtensionsCategory.AddCategory( "peripherals" );

        public abstract string PeripheralName { get; }

        public abstract PeripheralType PeripheralType { get; }

        public abstract uint PresentPin { get; }

        public abstract uint AddressRangeStart { get; }
        public abstract uint AddressRangeEnd { get; }

        protected Cpu AttachedCpu { get; private set; }

        protected override LoggerSystems SubSystem => LoggerSystems.Peripherals;

        #region Unity Event Functions

        public abstract void Reset();

        #endregion

        #region Public

        public static IEnumerable < Peripheral > GetExtensionPeripherals()
        {
            return ExtensionLoader.LoadFrom < Peripheral >( PeripheralExtensions.GetCategoryDirectory(), true );
        }

        public static IEnumerable < PeripheralImporter > GetPeripheralImporters()
        {
            return ExtensionLoader.LoadFrom < PeripheralImporter >(
                                                                   PeripheralExtensions.GetCategoryDirectory(),
                                                                   true
                                                                  ).
                                   Concat( DefaultPeripheralImporters );
        }
        

        //public abstract bool CanRead( uint address );

        //public abstract bool CanWrite( uint address );

        public abstract uint ReadData( uint address );

        public abstract void WriteData( uint address, uint data );

        public virtual void Dump( Stream str )
        {
        }

        public void SetCpu( Cpu cpu )
        {
            AttachedCpu = cpu;
        }

        public virtual void Shutdown()
        {
        }

        #endregion

    }

}
