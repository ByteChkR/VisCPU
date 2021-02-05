using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.TypeSystem.Events
{

    internal class HlMemberNotFoundEvent : ErrorEvent
    {

        #region Public

        public HlMemberNotFoundEvent(HlTypeDefinition type, string name ) : base(
                                                           $"Can not Find Member '{name}' in type '{type.Name}'",
                                                           ErrorEventKeys.s_HlMemberNotFound,
                                                           false
                                                          )
        {
        }

        #endregion

    }

}
