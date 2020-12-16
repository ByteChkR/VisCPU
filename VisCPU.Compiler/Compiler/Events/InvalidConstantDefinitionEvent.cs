using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Compiler.Events
{

    public class InvalidConstantDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-const-def";

        #region Public

        public InvalidConstantDefinitionEvent( string message ) : base( message, EVENT_KEY, false )
        {
        }

        #endregion

    }

}
