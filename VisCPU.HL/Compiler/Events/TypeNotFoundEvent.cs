using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class TypeNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "type-not-found";

        #region Public

        public TypeNotFoundEvent( string typeName ) : base(
                                                           $"Can not find type with name {typeName}",
                                                           EVENT_KEY,
                                                           false
                                                          )
        {
        }

        #endregion

    }

}
