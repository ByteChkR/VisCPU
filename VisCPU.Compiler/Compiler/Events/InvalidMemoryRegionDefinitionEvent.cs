using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Compiler.Events
{

    public class InvalidMemoryRegionDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-mem-region-def";

        #region Public

        public InvalidMemoryRegionDefinitionEvent( string name ) : base(
                                                                        $"Invalid Memory Region Arguments: {name}",
                                                                        EVENT_KEY,
                                                                        false
                                                                       )
        {
        }

        #endregion

    }

}
