using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Linking.Events
{

    internal class DuplicateLinkerItemEvent : WarningEvent
    {
        #region Public

        public DuplicateLinkerItemEvent( string item ) : base(
            $"The item '{item}' has a duplicated entry.",
            WarningEventKeys.s_LinkerDuplicateItem
        )
        {
        }

        #endregion
    }

}
