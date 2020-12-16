using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Parser.Events
{

    public class InvalidDecValueEvent : ErrorEvent
    {

        private const string EVENT_KEY = "invalid-dec-value";

        #region Public

        public InvalidDecValueEvent( string value, int start ) : base(
                                                                      $"Invalid decimal Value: '{value}' at line {start}",
                                                                      EVENT_KEY,
                                                                      false
                                                                     )
        {
        }

        #endregion

    }

}
