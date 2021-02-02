using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    internal class MultipleReceiverWriteEvent : WarningEvent
    {
        #region Public

        public MultipleReceiverWriteEvent( uint address ) : base(
            $"Multiple Overlapping Peripherals found at address 0x{address.ToHexString()}",
            WarningEventKeys.s_MemoryBusDeviceOverlap
        )
        {
        }

        #endregion
    }

}
