﻿using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobBuild : BuildJobRunner
    {

        public override string RunnerName => "build";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job )
        {
            ProgramBuilder.Build( job.Arguments );
        }

        #endregion

    }

}