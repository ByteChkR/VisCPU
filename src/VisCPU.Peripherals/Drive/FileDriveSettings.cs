using System.IO;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Peripherals.Drive
{

    public class FileDriveSettings : DrivePinSettings
    {
        public uint FileLength = 1024 * 1024 * 32;

        public string FileDrive = Path.Combine(
            Peripheral.PeripheralCategory.
                       AddCategory( "FileDrive" ).
                       AddCategory( "states" ).
                       GetCategoryDirectory(),
            "default.bin"
        );

        #region Private

        static FileDriveSettings()
        {
            SettingsCategory fileDriveCategory = Peripheral.PeripheralCategory.AddCategory( "FileDrive" );

            SettingsManager.RegisterDefaultLoader < FileDriveSettings >(
                new JsonSettingsLoader(),
                fileDriveCategory,
                "default.json",
                new FileDriveSettings()
            );
        }

        #endregion
    }

}
