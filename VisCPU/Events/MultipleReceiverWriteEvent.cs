﻿using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class MultipleReceiverWriteEvent : WarningEvent
    {

        #region Public

        public MultipleReceiverWriteEvent( uint address ) : base(
                                                                 $"Multiple Overlapping Peripherals found at address 0x{address.ToHexString()}",
                                                                 WarningEventKeys.MEMORY_BUS_DEVICE_OVERLAP
                                                                )
        {
        }

        #endregion

    }

}
