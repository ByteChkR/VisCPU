using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Console.Core.Subsystems.Modules;
using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Console.Core.Subsystems.BuildSystem
{
    public class BuildJobSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
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

            ProjectConfig config = ProjectConfig.Load(src);
            string target = config.DefaultTarget;
            if (a.Length > 1)
            {
                target = a[1];
            }

            config.RunTarget(root, target);
        }

        public override void Help()
        {
            Log( "vis make <root> <target>" );
        }

    }

    public class BuildJobExternalBuild:BuildJobRunner
    {

        public override string RunnerName => "external";

        public override void RunJob( string projectRoot, ProjectConfig project, ProjectBuildTarget target, BuildJob job )
        {
            string path = job.Arguments["path"];
            job.Arguments.TryGetValue( "target", out string externalTarget );
            ProjectConfig external = ProjectConfig.Load( path);
            external.RunTarget( Path.GetDirectoryName( path ), externalTarget );
        }

    }

    public class BuildJobClean : BuildJobRunner
    {

        public override string RunnerName => "clean";

        public override void RunJob(string projectRoot, ProjectConfig project, ProjectBuildTarget target, BuildJob job)
        {
            ProjectCleanSubSystem.Clean(projectRoot);
        }

    }
    public class BuildJobRestore : BuildJobRunner
    {

        public override string RunnerName => "restore";

        public override void RunJob(string projectRoot, ProjectConfig project, ProjectBuildTarget target, BuildJob job)
        {
            if (!job.Arguments.TryGetValue("repo", out string repo))
            {
                repo = "local";
            }

            ProjectRestoreSubSystem.Restore(projectRoot, repo);
        }

    }

    public class BuildJobPublish : BuildJobRunner
    {

        public override string RunnerName => "publish";

        public override void RunJob(string projectRoot, ProjectConfig project, ProjectBuildTarget target, BuildJob job)
        {
            if (!job.Arguments.TryGetValue("repo", out string repo))
            {
                repo = "local";
            }

            ProjectPackSubSystem.PackOptions opts = new ProjectPackSubSystem.PackOptions();
            ProjectPublishSubSystem.PublishOptions pops = new ProjectPublishSubSystem.PublishOptions();
            if (job.Arguments.TryGetValue("version", out string ver))
                opts.VersionString = ver; 
            if (job.Arguments.TryGetValue("origin", out string origin))
                pops.Repository = origin;
            ProjectPublishSubSystem.Publish(projectRoot, pops, opts);
        }

    }
    public class BuildJobBuild : BuildJobRunner
    {

        public override string RunnerName => "build";

        public override void RunJob(string projectRoot, ProjectConfig project, ProjectBuildTarget target, BuildJob job)
        {

            ProgramBuilder.Build(job.Arguments);

        }

    }

}
