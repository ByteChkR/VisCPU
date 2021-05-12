using System.Collections.Generic;

namespace VisCPU.Peripherals
{

    public abstract class PeripheralImporter
    {

        #region Public

        public abstract List < Peripheral > GetPeripherals( IEnumerable < Peripheral > p );

        #endregion

    }

}
