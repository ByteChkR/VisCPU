using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Events
{

    public class TypeNotFoundEvent : ErrorEvent
    {

        #region Public

        public TypeNotFoundEvent( string typeName ) : base(
                                                           $"Can not find type with name {typeName}",
                                                           ErrorEventKeys.s_HlTypeNotFound,
                                                           false
                                                          )
        {
        }

        #endregion

    }

}
