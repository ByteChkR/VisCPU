using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class FunctionNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "func-not-found";

        #region Public

        public FunctionNotFoundEvent( string funcName ) : base( $"Function '{funcName}' not found", EVENT_KEY, false )
        {
        }

        #endregion

    }

}
