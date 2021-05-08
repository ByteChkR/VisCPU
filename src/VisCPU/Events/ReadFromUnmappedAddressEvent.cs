using System;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Events
{

    internal class ReadFromUnmappedAddressEvent : WarningEvent
    {
        #region Public

        public ReadFromUnmappedAddressEvent( uint address ) : base(
            $"Tried to read from address 0x{Convert.ToString( address, 16 )} which is not mapped.",
            WarningEventKeys.s_MemoryBusReadUnmapped
        )
        {
        }

        #endregion
    }

}
