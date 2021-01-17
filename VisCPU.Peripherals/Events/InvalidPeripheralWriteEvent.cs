using System;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    internal class InvalidPeripheralWriteEvent : WarningEvent
    {

        #region Public

        public InvalidPeripheralWriteEvent( uint address, uint data, Peripheral peripheral ) : base(
             $"Can not write data '0x{Convert.ToString( data, 16 )}' to address '0x{Convert.ToString( address, 16 )}' mapped to peripheral '{peripheral}'",
             WarningEventKeys.s_PeripheralInvalidWrite
            )
        {
        }

        #endregion

    }

}
