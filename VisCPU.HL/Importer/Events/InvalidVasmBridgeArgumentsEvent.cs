using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
