using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Parser.Events
{

    internal class InvalidCharValueEvent : ErrorEvent
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
