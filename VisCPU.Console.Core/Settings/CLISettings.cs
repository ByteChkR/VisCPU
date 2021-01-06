using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
{

    public class CLISettings
    {

        [Argument( Name = "cli:loop" )]
        public bool Continuous = false;

        [Argument( Name = "cli:waitOnExit" )]
        public bool WaitOnExit = false;

        #region Public

        public static CLISettings Create()
        {
            return SettingsSystem.GetSettings < CLISettings >();
        }

        #endregion

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
