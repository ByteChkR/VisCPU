using System;
using VisCPU.Utility.Events;

namespace VisCPU.Peripherals.Events
{
    public class InvalidPeripheralWriteEvent : WarningEvent
    {
        private const string EVENT_KEY = "p_invalid_write";

        #region Public

        public InvalidPeripheralWriteEvent(uint address, uint data, Peripheral peripheral) : base(
            $"Can not write data '0x{Convert.ToString(data, 16)}' to address '0x{Convert.ToString(address, 16)}' mapped to peripheral '{peripheral}'",
            EVENT_KEY
        )
        {
        }

        #endregion
    }
}