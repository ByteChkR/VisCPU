using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    public class HLTypeRedefinitionEvent : ErrorEvent
    {

        #region Public

        public HLTypeRedefinitionEvent( string typeName ) : base(
                                                                 $"Duplicate definition of type {typeName}",
                                                                 ErrorEventKeys.HL_TYPE_DUPLICATE_DEF,
                                                                 false
                                                                )
        {
        }

        #endregion

    }

}
