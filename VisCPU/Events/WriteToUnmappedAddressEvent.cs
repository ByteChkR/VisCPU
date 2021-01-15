using System;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class WriteToUnmappedAddressEvent : WarningEvent
    {

        #region Public

        public WriteToUnmappedAddressEvent( uint address, uint data ) : base(
                                                                             $"Tried to write value '0x{Convert.ToString( data, 16 )}' to address 0x{Convert.ToString( address, 16 )} which is not mapped.",
                                                                             WarningEventKeys.s_MemoryBusWriteUnmapped
                                                                            )
        {
        }

        #endregion

    }

}
