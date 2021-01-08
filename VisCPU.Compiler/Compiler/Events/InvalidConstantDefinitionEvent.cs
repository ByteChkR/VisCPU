using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Compiler.Events
{

    public class InvalidConstantDefinitionEvent : ErrorEvent
    {

        #region Public

        public InvalidConstantDefinitionEvent( string message ) : base(
                                                                       message,
                                                                       ErrorEventKeys.VASM_INVALID_CONSTANT_DEFINITION,
                                                                       false
                                                                      )
        {
        }

        #endregion

    }

}
