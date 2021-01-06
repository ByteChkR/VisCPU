using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    public class InvalidVasmBridgeArgumentsEvent : ErrorEvent
    {

        #region Public

        public InvalidVasmBridgeArgumentsEvent( string args ) : base(
                                                                     $"Arguments Invalid: '{args}'",
                                                                     ErrorEventKeys.VASM_BRIDGE_INVALID_ARGUMENTS,
                                                                     false
                                                                    )
        {
        }

        #endregion

    }

}
