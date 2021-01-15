using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Parser.Events
{

    public class InvalidCharValueEvent : ErrorEvent
    {

        #region Public

        public InvalidCharValueEvent( string value, int start ) : base(
                                                                       $"Invalid char Value: '{value}' at line {start}",
                                                                       ErrorEventKeys.s_VasmParserInvalidCharValue,
                                                                       false
                                                                      )
        {
        }

        #endregion

    }

}
