using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Importer.Events
{

    internal class InvalidVasmBridgeArgumentsEvent : ErrorEvent
    {

        #region Public

        public InvalidVasmBridgeArgumentsEvent( string args ) : base(
                                                                     $"Arguments Invalid: '{args}'",
                                                                     ErrorEventKeys.s_VasmBridgeInvalidArguments,
                                                                     false
                                                                    )
        {
        }

        #endregion

    }

}
