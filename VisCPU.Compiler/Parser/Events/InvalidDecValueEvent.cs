using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Parser.Events
{

    internal class InvalidDecValueEvent : ErrorEvent
    {

        #region Public

        public InvalidDecValueEvent( string value, int start ) : base(
                                                                      $"Invalid decimal Value: '{value}' at line {start}",
                                                                      ErrorEventKeys.s_VasmParserInvalidNumberValue,
                                                                      false
                                                                     )
        {
        }

        #endregion

    }

}
