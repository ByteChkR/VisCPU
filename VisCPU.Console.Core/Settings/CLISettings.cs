using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

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
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "config/console.json"
                                                             ),
                                                 new CLISettings()
                                                );
        }

        #endregion

    }

}
