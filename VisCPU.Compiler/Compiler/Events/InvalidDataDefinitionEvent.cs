using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Compiler.Events
{

    public class InvalidDataDefinitionEvent : ErrorEvent
    {

        #region Public

        public InvalidDataDefinitionEvent( string name ) : base(
                                                                $"Invalid Memory Region Arguments: {name}",
                                                                ErrorEventKeys.s_VasmInvalidDataDefinition,
                                                                false
                                                               )
        {
        }

        #endregion

    }

}
