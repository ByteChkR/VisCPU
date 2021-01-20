using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Settings.Events
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
