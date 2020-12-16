using System;

using VisCPU.Utility.Events;

namespace VisCPU.Events
{

    public class ReadFromUnmappedAddressEvent : WarningEvent
    {

        public const string EVENT_KEY = "mb-read-unmapped";

        #region Public

        public ReadFromUnmappedAddressEvent( uint address ) : base(
                                                                   $"Tried to read from address 0x{Convert.ToString( address, 16 )} which is not mapped.",
                                                                   EVENT_KEY
                                                                  )
        {
        }

        #endregion

    }

}
