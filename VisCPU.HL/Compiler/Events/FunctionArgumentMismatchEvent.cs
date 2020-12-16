using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler
{

    public class FunctionArgumentMismatchEvent : ErrorEvent
    {

        private const string EVENT_KEY = "func-arg-mismatch";
        public FunctionArgumentMismatchEvent(string errMessage) : base(errMessage, EVENT_KEY, false)
        {
        }

    }

}