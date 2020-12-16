using System;

using VisCPU.Utility.Events;

namespace VisCPU.Peripherals.Events
{

    public class InvalidPeripheralReadEvent : WarningEvent
    {

        private const string EVENT_KEY = "p_invalid_read";

        #region Public

        public InvalidPeripheralReadEvent( uint address, Peripheral peripheral ) : base(
             $"Can not read address '0x{Convert.ToString( address, 16 )}' mapped to peripheral '{peripheral}'",
             EVENT_KEY
            )
        {
        }

        #endregion

    }

}
