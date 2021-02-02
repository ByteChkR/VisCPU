using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.IO.Settings.Events
{

    public class SettingsCategoryExistsEvent : ErrorEvent
    {

        #region Public

        public SettingsCategoryExistsEvent( SettingsCategory parent, string category ) : base(
             $"Parent '{parent.FullCategoryName}' already contains a category '{category}'",
             ErrorEventKeys.s_SettingsDuplicateCategory,
             false
            )
        {
        }

        #endregion

    }

}
