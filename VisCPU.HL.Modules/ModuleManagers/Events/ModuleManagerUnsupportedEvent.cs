using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Modules.ModuleManagers.Events
{

    public class ModuleManagerUnsupportedEvent : ErrorEvent
    {

        #region Public

        public ModuleManagerUnsupportedEvent( ModuleManager manager, string feature ) : base(
             $"'{manager}' does not support {feature}",
             ErrorEventKeys.MODULE_MANAGER_UNSUPPORTED_FEATURE,
             false
            )
        {
        }

        #endregion

    }

}
