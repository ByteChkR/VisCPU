using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Console.Core.Settings
{

    public class CLISettings
    {
        [field: Argument( Name = "cli:loop" )]
        public bool Continuous { get; set; }

        [field: Argument( Name = "cli:waitOnExit" )]
        public bool WaitOnExit { get; set; }

        [field: Argument( Name = "cli:configs" )]
        public string[] Configs { get; set; } = new[] { "./default.args" };

        #region Private

        static CLISettings()
        {
            SettingsCategory cliCategory = SettingsCategories.Get( "sdk", true );

            SettingsManager.RegisterDefaultLoader(
                new JSONSettingsLoader(),
                cliCategory,
                "cli.json",
                new CLISettings()
            );
        }

        #endregion
    }

}
