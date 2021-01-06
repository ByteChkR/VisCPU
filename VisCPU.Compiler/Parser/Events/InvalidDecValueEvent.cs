using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Parser.Events
{

    public class InvalidDecValueEvent : ErrorEvent
    {

        #region Public

        public InvalidDecValueEvent( string value, int start ) : base(
                                                                      $"Invalid decimal Value: '{value}' at line {start}",
                                                                      ErrorEventKeys.VASM_PARSER_INVALID_NUMBER_VALUE,
                                                                      false
                                                                     )
        {
        }

        #endregion

    }

}
