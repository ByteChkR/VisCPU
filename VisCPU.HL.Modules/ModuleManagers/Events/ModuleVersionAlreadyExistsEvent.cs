using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Modules.ModuleManagers.Events
{

    public class ModuleVersionAlreadyExistsEvent : ErrorEvent
    {

        #region Public

        public ModuleVersionAlreadyExistsEvent( string moduleName, string version ) : base(
             $"Module {moduleName} already has a build published for version {version}",
             ErrorEventKeys.MODULE_VERSION_EXISTS,
             false
            )
        {
        }

        #endregion

    }

}
