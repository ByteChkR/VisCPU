using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class DynamicVariablesNotSupportedEvent : ErrorEvent
    {

        #region Public

        public DynamicVariablesNotSupportedEvent() : base(
                                                          "Dynamic Variables are not supported",
                                                          ErrorEventKeys.s_HlCompilerDynamicVariablesNotSupported,
                                                          false
                                                         )
        {
        }

        #endregion

    }

}
