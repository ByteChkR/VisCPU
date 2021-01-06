using System;

using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class ReadFromUnmappedAddressEvent : WarningEvent
    {

        #region Public

        public ReadFromUnmappedAddressEvent( uint address ) : base(
                                                                   $"Tried to read from address 0x{Convert.ToString( address, 16 )} which is not mapped.",
                                                                   WarningEventKeys.MEMORY_BUS_READ_UNMAPPED
                                                                  )
        {
        }

        #endregion

    }

}
