using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.IO.Settings.Events
{

    public class SettingsRootCategoryExistsEvent : ErrorEvent
    {
        #region Public

        public SettingsRootCategoryExistsEvent( string category ) : base(
            $"Root already contains a category '{category}'",
            ErrorEventKeys.s_SettingsDuplicateCategory,
            false
        )
        {
        }

        #endregion
    }

}
