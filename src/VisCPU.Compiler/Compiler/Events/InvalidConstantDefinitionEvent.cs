using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Compiler.Events
{

    internal class InvalidConstantDefinitionEvent : ErrorEvent
    {

        #region Public

        public InvalidConstantDefinitionEvent( string message ) : base(
                                                                       message,
                                                                       ErrorEventKeys.s_VasmInvalidConstantDefinition,
                                                                       false
                                                                      )
        {
        }

        #endregion

    }

}
