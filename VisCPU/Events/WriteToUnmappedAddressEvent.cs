using System;

using VisCPU.Utility.Events;

namespace VisCPU.Events
{

    public class WriteToUnmappedAddressEvent : WarningEvent
    {

        public const string EVENT_KEY = "mb-write-unmapped";

        #region Public

        public WriteToUnmappedAddressEvent( uint address, uint data ) : base(
                                                                             $"Tried to write value '0x{Convert.ToString( data, 16 )}' to address 0x{Convert.ToString( address, 16 )} which is not mapped.",
                                                                             EVENT_KEY
                                                                            )
        {
        }

        #endregion

    }

}
