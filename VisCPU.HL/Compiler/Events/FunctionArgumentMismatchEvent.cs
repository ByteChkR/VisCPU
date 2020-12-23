using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler.Events
{
    public class FunctionArgumentMismatchEvent : ErrorEvent
    {
        private const string EVENT_KEY = "func-arg-mismatch";

        #region Public

        public FunctionArgumentMismatchEvent(string errMessage) : base(errMessage, EVENT_KEY, false)
        {
        }

        #endregion
    }
}