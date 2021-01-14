using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Events
{

    public class DynamicVariablesNotSupportedEvent : ErrorEvent
    {

        #region Public

        public DynamicVariablesNotSupportedEvent() : base(
                                                          "Dynamic Variables are not supported",
                                                          ErrorEventKeys.HL_COMPILER_DYNAMIC_VARIABLES_NOT_SUPPORTED,
                                                          false
                                                         )
        {
        }

        #endregion

    }

}
