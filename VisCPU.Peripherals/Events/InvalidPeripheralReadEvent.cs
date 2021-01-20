using System;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    internal class InvalidPeripheralReadEvent : WarningEvent
    {
        #region Public

        public InvalidPeripheralReadEvent( uint address, Peripheral peripheral ) : base(
            $"Can not read address '0x{Convert.ToString( address, 16 )}' mapped to peripheral '{peripheral}'",
            WarningEventKeys.s_PeripheralInvalidRead
        )
        {
        }

        #endregion
    }

}
