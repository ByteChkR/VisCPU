﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ProjectAddDependencySubSystem : ConsoleSubsystem
    {
        private class AddDependencyConfig
        {
            [Argument(Name="name")]
            public string Name = "ModuleDependency";
            [Argument(Name = "version")]
            public string Version = "ANY";

        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();
            ProjectConfig t = ProjectConfig.Load(Path.Combine(Path.GetFullPath(a[0]), "project.json"));

            AddDependencyConfig config = new AddDependencyConfig();
            ArgumentSyntaxParser.Parse( a.Skip( 1 ).ToArray(), config );

            t.Dependencies.Add(
                               new ProjectDependency
                               {
                                   ProjectName = config.Name,
                                   ProjectVersion = config.Version
                               }
                              );

            ModuleManager.SaveModuleTarget( t, Path.Combine( Directory.GetCurrentDirectory(), "project.json" ) );
        }

        public override void Help()
        {
            HelpSubSystem.WriteSubsystem("vis dependency add <projectRoot>", new AddDependencyConfig()  );
        }

        #endregion

    }

}
