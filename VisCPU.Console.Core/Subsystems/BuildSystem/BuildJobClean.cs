using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Console.Core.Subsystems.Modules;
using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.BuildSystem
{

    public class BuildJobSystem : ConsoleSubsystem
    {

        #region Public

        public override void Help()
        {
            Log("vis make <root> <target>");
        }


        public override void Run(IEnumerable<string> args)
        {
            ModuleResolver.Initialize();
            CommonFiles.GenerateCommonFiles();
            string[] a = args.ToArray();

            string root = a.Length != 0
                              ? Path.GetFullPath(a[0])
                              : Directory.GetCurrentDirectory();

            string src = Path.Combine(root, "project.json");


            ProjectConfig.AddRunner(new BuildJobBuild());
            ProjectConfig.AddRunner(new BuildJobClean());
            ProjectConfig.AddRunner(new BuildJobPublish());
            ProjectConfig.AddRunner(new BuildJobRestore());
            ProjectConfig.AddRunner(new BuildJobExternalBuild());
            ProjectConfig.AddRunner(new BuildJobCombinedJobs());
            ProjectConfig.AddRunner(new BuildJobMoveContent());
            ProjectConfig.AddRunner(new BuildJobCopyContent());
            ProjectConfig.AddRunner(new BuildJobRunJob());
            ProjectConfig.AddRunner(new BuildJobGetDependency());
            ProjectConfig.AddRunner(new BuildJobAddOrigin());
            ProjectConfig.AddRunner(new BuildJobRemoveOrigin());

            ProjectConfig config = ProjectConfig.Load(src);
            string target = config.DefaultTarget;

            if (a.Length > 1)
            {
                target = a[1];
            }

            config.RunTarget(root, target);
        }

        #endregion

    }

    public class BuildJobGetDependency : BuildJobRunner
    {

        public override string RunnerName => "get-dependency";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {

            foreach (KeyValuePair<string, string> keyValuePair in job.Arguments)
            {
                ProjectInfo info = ModuleResolver.Resolve(keyValuePair.Key, keyValuePair.Value);
                info.Manager.Get(info, Path.Combine(projectRoot, info.ProjectName));
            }
        }

        #endregion

    }

    public class BuildJobAddOrigin : BuildJobRunner
    {

        public override string RunnerName => "add-origin";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            foreach (KeyValuePair<string, string> keyValuePair in job.Arguments)
            {
                AddOriginSubSystem.AddOrigin(keyValuePair.Key, keyValuePair.Value);
            }
        }

        #endregion

    }

    public class BuildJobRemoveOrigin : BuildJobRunner
    {

        public override string RunnerName => "remove-origins";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            foreach (string argumentsKey in job.Arguments.Keys)
            {
                RemoveOriginSubSystem.RemoveOrigin(argumentsKey);
            }
        }

        #endregion

    }

    public class BuildJobExternalBuild : BuildJobRunner
    {

        public override string RunnerName => "external-build";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            string path = job.Arguments["path"];
            job.Arguments.TryGetValue("target", out string externalTarget);
            ProjectConfig external = ProjectConfig.Load(path);
            external.RunTarget(Path.GetDirectoryName(path), externalTarget);
        }

        #endregion

    }

    public class BuildJobCombinedJobs : BuildJobRunner
    {

        public override string RunnerName => "combined";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            foreach (KeyValuePair<string, string> buildJobs in job.Arguments)
            {
                BuildJob subJob = BuildJob.Load(buildJobs.Value);
                project.RunJob(projectRoot, target, subJob);
            }
        }

        #endregion

    }

    public class BuildJobRunJob : BuildJobRunner
    {

        public override string RunnerName => "run";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            ProgramRunner.Run(job.Arguments);
        }

        #endregion

    }

    public class BuildJobCopyContent : BuildJobRunner
    {

        public override string RunnerName => "copy";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            string input = job.Arguments["source"];
            string output = job.Arguments["destination"];

            if (Directory.Exists(input))
            {
                Directory.CreateDirectory(output);
                ProjectPackSubSystem.CopyTo(input, output);
            }
            else if (File.Exists(input))
            {
                if (Directory.Exists(output))
                {
                    File.Copy(input, Path.Combine(output, Path.GetFileName(input)), true);
                }
                else
                {
                    File.Copy(input, output, true);
                }
            }
        }

        #endregion

    }

    public class BuildJobMoveContent : BuildJobRunner
    {

        public override string RunnerName => "move";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            string input = job.Arguments["source"];
            string output = job.Arguments["destination"];

            if (Directory.Exists(input))
            {
                if (Directory.Exists(output))
                {
                    Directory.Delete(output);
                }

                Directory.Move(input, output);
            }
            else if (File.Exists(input))
            {
                if (Directory.Exists(output))
                {
                    string path = Path.Combine(output, Path.GetFileName(input));

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    File.Move(input, path);
                }
                else
                {
                    if (File.Exists(output))
                    {
                        File.Delete(output);
                    }

                    File.Move(input, output);
                }
            }
        }

        #endregion

    }

    public class BuildJobClean : BuildJobRunner
    {

        public override string RunnerName => "clean";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            ProjectCleanSubSystem.Clean(projectRoot);
        }

        #endregion

    }

    public class BuildJobRestore : BuildJobRunner
    {

        public override string RunnerName => "restore";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            if (!job.Arguments.TryGetValue("repo", out string repo))
            {
                repo = "local";
            }

            ProjectRestoreSubSystem.Restore(projectRoot, repo);
        }

        #endregion

    }

    public class BuildJobPublish : BuildJobRunner
    {

        public override string RunnerName => "publish";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            if (!job.Arguments.TryGetValue("repo", out string repo))
            {
                repo = "local";
            }

            ProjectPackSubSystem.PackOptions opts = new ProjectPackSubSystem.PackOptions();
            ProjectPublishSubSystem.PublishOptions pops = new ProjectPublishSubSystem.PublishOptions();

            if (job.Arguments.TryGetValue("version", out string ver))
            {
                opts.VersionString = ver;
            }

            if (job.Arguments.TryGetValue("origin", out string origin))
            {
                pops.Repository = origin;
            }

            ProjectPublishSubSystem.Publish(projectRoot, pops, opts);
        }

        #endregion

    }

    public class BuildJobBuild : BuildJobRunner
    {

        public override string RunnerName => "build";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job)
        {
            ProgramBuilder.Build(job.Arguments);
        }

        #endregion

    }

}
