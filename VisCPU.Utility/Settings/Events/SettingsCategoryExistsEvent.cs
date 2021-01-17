using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Settings.Events
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
