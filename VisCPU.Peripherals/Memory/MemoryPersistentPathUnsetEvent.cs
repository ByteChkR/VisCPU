using VisCPU.Utility.Events;

namespace VisCPU.Peripherals.Memory
{
    public class MemoryPersistentPathUnsetEvent:WarningEvent
    {

        private const string EVENT_KEY = "memory-path-not-set";
        public MemoryPersistentPathUnsetEvent() : base( "The Memory has the 'Persistent' flag set to true but the PersistentPath is not set.", EVENT_KEY)
        {
        }

    }
}