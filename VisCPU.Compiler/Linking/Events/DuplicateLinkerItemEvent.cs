using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Linking.Events
{

    public class DuplicateLinkerItemEvent : WarningEvent
    {

        #region Public

        public DuplicateLinkerItemEvent( string item ) : base(
                                                              $"The item '{item}' has a duplicated entry.",
                                                              WarningEventKeys.LINKER_DUPLICATE_ITEM
                                                             )
        {
        }

        #endregion

    }

}
