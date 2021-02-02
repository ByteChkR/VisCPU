using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Events
{

    internal class HlTypeRedefinitionEvent : ErrorEvent
    {

        #region Public

        public HlTypeRedefinitionEvent( string typeName ) : base(
                                                                 $"Duplicate definition of type {typeName}",
                                                                 ErrorEventKeys.s_HlTypeDuplicateDef,
                                                                 false
                                                                )
        {
        }

        #endregion

    }

}
