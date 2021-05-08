using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

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
