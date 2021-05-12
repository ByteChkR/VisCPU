using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Peripherals.Memory
{

    internal class MemoryPersistentPathUnsetEvent : WarningEvent
    {

        #region Public

        public MemoryPersistentPathUnsetEvent() : base(
                                                       "The Memory has the 'Persistent' flag set to true but the PersistentPath is not set.",
                                                       WarningEventKeys.s_PeripheralMemoryPathNotSet
                                                      )
        {
        }

        #endregion

    }

}
