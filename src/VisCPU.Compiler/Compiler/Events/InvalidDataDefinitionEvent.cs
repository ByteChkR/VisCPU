﻿using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Compiler.Events
{

    internal class InvalidDataDefinitionEvent : ErrorEvent
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
