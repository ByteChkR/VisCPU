using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    public class MemoryPersistentPathUnsetEvent : WarningEvent
    {

        #region Public

        public MemoryPersistentPathUnsetEvent() : base(
                                                       "The Memory has the 'Persistent' flag set to true but the PersistentPath is not set.",
                                                       WarningEventKeys.PERIPHERAL_MEMORY_PATH_NOT_SET
                                                      )
        {
        }

        #endregion

    }

}
