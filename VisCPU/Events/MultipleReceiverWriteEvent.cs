using VisCPU.Utility;
using VisCPU.Utility.Events;

namespace VisCPU
{

    public class MultipleReceiverWriteEvent : WarningEvent
    {

        public const string EVENT_KEY = "mb-write-multiple";

        #region Public

        public MultipleReceiverWriteEvent( uint address ) : base(
                                                                 $"Multiple Overlapping Peripherals found at address 0x{address.ToHexString()}",
                                                                 EVENT_KEY
                                                                )
        {
        }

        #endregion

    }

}