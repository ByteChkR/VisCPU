﻿using System;
using System.Collections.Generic;
using System.IO;

using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Settings
{

    [Serializable]
    public class OriginSettings
    {

        public Dictionary < string, string > origins = new Dictionary < string, string >
                                                       {
                                                           {
                                                               "local", Path.Combine(
                                                                    AppDomain.CurrentDomain.BaseDirectory,
                                                                    "config/module/local"
                                                                   )
                                                           }
                                                       };

        #region Public

        public static OriginSettings Create()
        {
            return SettingsSystem.GetSettings < OriginSettings >();
        }

        #endregion

        #region Private

        static OriginSettings()
        {
            SettingsSystem.RegisterDefaultLoader(
                                                 new JSONSettingsLoader(),
                                                 Path.Combine(
                                                              AppDomain.CurrentDomain.BaseDirectory,
                                                              "config/origins.json"
                                                             ),
                                                 new OriginSettings()
                                                );
        }

        #endregion

    }

}
