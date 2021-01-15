using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.HL.Modules.Resolvers
{

    public static class CommonFiles
    {

        public static void GenerateCommonFiles()
        {
            GenerateCommonTargets();
            GenerateCommonJobs();
        }


        public static void InitializeProjectFolder(string rootDir)
        {
            string name = Path.GetFileName(rootDir);
            ProjectConfig config = GenerateProjectConfig( name );
            ProjectConfig.Save( Path.Combine( rootDir, "project.json" ), config );
            if(!File.Exists(Path.Combine(rootDir, "Program.vhl")))
            File.WriteAllText(
                              Path.Combine( rootDir, "Program.vhl" ),
                              @"//Entry Point of Project"
                             );
        }



        public static ProjectConfig GenerateProjectConfig(string name=null, string version=null)
        {

            ProjectConfig config = new ProjectConfig();

            if ( name != null )
                config.ProjectName = name;

            if ( version != null )
                config.ProjectVersion = version;

            ProjectBuildTarget debugTarget = new ProjectBuildTarget();
            debugTarget.TargetName = "Debug";
            debugTarget.DependsOn = new[] { "%VISDIR%common/targets/debug.json" };
            ProjectBuildTarget debugRunTarget = new ProjectBuildTarget();
            debugRunTarget.TargetName = "DebugRun";
            debugRunTarget.DependsOn = new[] { "%VISDIR%common/targets/debugRun.json" };
            ProjectBuildTarget releaseTarget = new ProjectBuildTarget();
            releaseTarget.TargetName = "Release";
            releaseTarget.DependsOn = new[] { "%VISDIR%common/targets/release.json" };
            ProjectBuildTarget releaseRunTarget = new ProjectBuildTarget();
            releaseRunTarget.TargetName = "ReleaseRun";
            releaseRunTarget.DependsOn = new[] { "%VISDIR%common/targets/releaseRun.json" };

            ProjectBuildTarget publishTarget = new ProjectBuildTarget();
            publishTarget.TargetName = "Publish";
            publishTarget.DependsOn = new string[0];
            BuildJob publishJob = new BuildJob();
            publishJob.JobName = "Publish Project";
            publishJob.BuildJobRunner = "merged";
            publishJob.Arguments["merge:include"] = "%VISDIR%common/jobs/newVersion.json";
            publishTarget.Jobs.Add( publishJob );
            config.BuildTargets["Publish"] = publishTarget;
            config.BuildTargets["Debug"] = debugTarget;
            config.BuildTargets["DebugRun"] = debugRunTarget;
            config.BuildTargets["Release"] = releaseTarget;
            config.BuildTargets["ReleaseRun"] = releaseRunTarget;

            return config;
        }

        private static void GenerateCommonJobs()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "common/jobs");
            Directory.CreateDirectory( dir );
            BuildJob cleanJob = new BuildJob();
            cleanJob.BuildJobRunner = "clean";
            cleanJob.JobName = "Clean Project %NAME%@%VERSION%";

            BuildJob restoreJob = new BuildJob();
            restoreJob.BuildJobRunner = "restore";
            restoreJob.JobName = "Restore Project %NAME%@%VERSION%";

            BuildJob publishJob = new BuildJob();
            publishJob.BuildJobRunner = "publish";
            publishJob.JobName = "Publish Project %NAME%@%VERSION%";
            publishJob.Arguments["repo"] = "local";

            BuildJob newVersionJob = new BuildJob();
            newVersionJob.BuildJobRunner = "combined";
            newVersionJob.JobName = "New Version Project %NAME%";
            newVersionJob.Arguments["publish"] = "%VISDIR%common/jobs/publish.json";
            newVersionJob.Arguments["restore"] = "%VISDIR%common/jobs/restore.json";


            BuildJob dBuildJob = CreateDebugBuildJob();
            BuildJob rBuildJob = CreateReleaseBuildJob();
            BuildJob dRunJob = CreateDebugRunJob();
            BuildJob rRunJob = CreateReleaseRunJob();

            BuildJob.Save(Path.Combine(dir, "debug_build.json"), dBuildJob);
            BuildJob.Save(Path.Combine(dir, "release_build.json"), rBuildJob);
            BuildJob.Save(Path.Combine(dir, "debug_run.json"), dRunJob);
            BuildJob.Save(Path.Combine(dir, "release_run.json"), rRunJob);

            BuildJob.Save(Path.Combine(dir, "clean.json"), cleanJob);
            BuildJob.Save(Path.Combine(dir, "restore.json"), restoreJob);
            BuildJob.Save(Path.Combine(dir, "publish.json"), publishJob);
            BuildJob.Save(Path.Combine(dir, "newVersion.json"), newVersionJob);

        }


        private static void GenerateCommonTargets()
        {
            string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "common/targets");
            Directory.CreateDirectory(dir);
            ProjectBuildTarget debug = CreateDebugTarget();
            ProjectBuildTarget release = CreateReleaseTarget();
            ProjectBuildTarget debugRun = CreateDebugRunTarget();
            ProjectBuildTarget releaseRun = CreateReleaseRunTarget();
            ProjectBuildTarget.Save(Path.Combine(dir, "debug.json"), debug);
            ProjectBuildTarget.Save(Path.Combine(dir, "debugRun.json"), debugRun);
            ProjectBuildTarget.Save(Path.Combine(dir, "release.json"), release);
            ProjectBuildTarget.Save(Path.Combine(dir, "releaseRun.json"), releaseRun);
        }

        private static BuildJob CreateDebugRunJob()
        {
            BuildJob debugRunJob = new BuildJob();
            debugRunJob.JobName = "Build %NAME%@%VERSION%";
            debugRunJob.BuildJobRunner = "run";
            debugRunJob.Arguments["run:input"] = "Program.vbin";
            debugRunJob.Arguments["run:cpu.interrupt"] = "0x00000000";
            debugRunJob.Arguments["run:cpu.reset"] = "0x00000000";
            debugRunJob.Arguments["run:working-dir"] = "%PROJDIR%";
            debugRunJob.Arguments["memory-bus:devices"] = "%VISDIR%config/memory/default.json";

            debugRunJob.Arguments["memory:read"] = "true";
            debugRunJob.Arguments["memory:write"] = "true";
            debugRunJob.Arguments["memory:persistent"] = "false";
            debugRunJob.Arguments["memory:persistent.path"] = "%VISDIR%config/memory/states/default.bin";
            debugRunJob.Arguments["memory:size"] = "262144";

            return debugRunJob;
        }
        private static BuildJob CreateDebugBuildJob()
        {
            BuildJob debugBuildJob = new BuildJob();
            debugBuildJob.JobName = "Build %NAME%@%VERSION%";
            debugBuildJob.BuildJobRunner = "build";
            debugBuildJob.Arguments["build:input"] = "Program.vhl";
            debugBuildJob.Arguments["build:steps"] = "HL-expr bin";
            debugBuildJob.Arguments["build:clean"] = "false";
            debugBuildJob.Arguments["assembler:offset.global"] = "0";
            debugBuildJob.Arguments["linker:export"] = "false";
            debugBuildJob.Arguments["compiler:optimize-temp-vars"] = "false";
            debugBuildJob.Arguments["compiler:optimize-const-expr"] = "false";

            return debugBuildJob;
        }
        private static BuildJob CreateReleaseRunJob()
        {
            BuildJob runJob = new BuildJob();
            runJob.JobName = "Run %NAME%@%VERSION%";
            runJob.BuildJobRunner = "run";
            runJob.Arguments["run:input"] = "Program.vbin";
            runJob.Arguments["run:cpu.interrupt"] = "0x00000000";
            runJob.Arguments["run:cpu.reset"] = "0x00000000";
            runJob.Arguments["run:working-dir"] = "%PROJDIR%";
            runJob.Arguments["memory-bus:devices"] = "%VISDIR%config/memory/default.json";

            runJob.Arguments["memory:read"] = "true";
            runJob.Arguments["memory:write"] = "true";
            runJob.Arguments["memory:persistent"] = "false";
            runJob.Arguments["memory:persistent.path"] = "%VISDIR%config/memory/states/default.bin";
            runJob.Arguments["memory:size"] = "262144";

            return runJob;
        }
        private static BuildJob CreateReleaseBuildJob()
        {
            BuildJob debugBuildJob = new BuildJob();
            debugBuildJob.JobName = "Build %NAME%@%VERSION%";
            debugBuildJob.BuildJobRunner = "build";
            debugBuildJob.Arguments["build:input"] = "Program.vhl";
            debugBuildJob.Arguments["build:steps"] = "HL-expr bin";
            debugBuildJob.Arguments["build:clean"] = "false";
            debugBuildJob.Arguments["assembler:offset.global"] = "0";
            debugBuildJob.Arguments["linker:export"] = "false";
            debugBuildJob.Arguments["compiler:optimize-temp-vars"] = "false";
            debugBuildJob.Arguments["compiler:optimize-const-expr"] = "false";

            return debugBuildJob;
        }


        private static ProjectBuildTarget CreateDebugRunTarget()
        {
            ProjectBuildTarget debug = new ProjectBuildTarget();
            debug.TargetName = "DebugRun";

            debug.DependsOn = new[] { "%VISDIR%common/targets/debug.json" };
            
            BuildJob mergeRunJob = new BuildJob();
            mergeRunJob.JobName = "Merged Debug Run";
            mergeRunJob.BuildJobRunner = "merged";
            mergeRunJob.Arguments["merge:include"] = "%VISDIR%common/jobs/debug_run.json";
            debug.Jobs.Add(mergeRunJob);

            return debug;
        }

        private static ProjectBuildTarget CreateDebugTarget()
        {
            ProjectBuildTarget debug = new ProjectBuildTarget();
            debug.TargetName = "Debug";
            BuildJob restoreJob = new BuildJob();
            restoreJob.BuildJobRunner = "restore";
            restoreJob.JobName = "Restore %NAME%@%VERSION%";
            restoreJob.Arguments["origin"] = "local";
            debug.Jobs.Add(restoreJob);


            BuildJob mergeBuildJob = new BuildJob();
            mergeBuildJob.JobName = "Merged Debug Build";
            mergeBuildJob.BuildJobRunner = "merged";
            mergeBuildJob.Arguments["merge:include"] = "%VISDIR%common/jobs/debug_build.json";
            debug.Jobs.Add(mergeBuildJob);

            return debug;
        }

        


        private static ProjectBuildTarget CreateReleaseRunTarget()
        {
            ProjectBuildTarget release = new ProjectBuildTarget();
            release.TargetName = "ReleaseRun";

            release.DependsOn = new[] { "%VISDIR%common/targets/release.json" };

            BuildJob mergeRunJob = new BuildJob();
            mergeRunJob.JobName = "Merged Release Run";
            mergeRunJob.BuildJobRunner = "merged";
            mergeRunJob.Arguments["merge:include"] = "%VISDIR%common/jobs/release_run.json";
            release.Jobs.Add(mergeRunJob);

            return release;
        }
        private static ProjectBuildTarget CreateReleaseTarget()
        {
            ProjectBuildTarget release = new ProjectBuildTarget();
            release.TargetName = "Release";
            BuildJob restoreJob = new BuildJob();
            restoreJob.BuildJobRunner = "restore";
            restoreJob.JobName = "Restore %NAME%@%VERSION%";
            restoreJob.Arguments["origin"] = "local";
            release.Jobs.Add(restoreJob);

            BuildJob mergeBuildJob = new BuildJob();
            mergeBuildJob.JobName = "Merged Release Build";
            mergeBuildJob.BuildJobRunner = "merged";
            mergeBuildJob.Arguments["merge:include"] = "%VISDIR%common/jobs/release_build.json";
            release.Jobs.Add(mergeBuildJob); ;


            return release;
        }

    }
    public static class ModuleResolver
    {

        public static ModuleResolverSettings ResolverSettings;

        private static Dictionary<string, ModuleManager> Managers;

        public static void AddManager(string name, string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri u))
            {
                if (u.Scheme == "http" || u.Scheme == "https")
                {
                    Managers[name]= new HttpModuleManager(name, u.OriginalString);
                }
                else if (u.Scheme == "dev")
                {
                    Managers[name] = new TCPUploadModuleManager(u.OriginalString);
                }
                else
                {
                    Managers[name] = new LocalModuleManager(url);
                }
            }
            else
            {
                Managers[name] = new LocalModuleManager(url);
            }
        }

        #region Public

        public static ModuleManager GetManager(string name)
        {
            return Managers[name];
        }

        public static IEnumerable<KeyValuePair<string, ModuleManager>> GetManagers()
        {
            return Managers;
        }

        public static void Initialize()
        {
            if (ResolverSettings == null && Managers == null)
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/module"));
                ResolverSettings = ModuleResolverSettings.Create();
                Managers = new Dictionary<string, ModuleManager>();

                foreach (KeyValuePair<string, string> origin in ResolverSettings.ModuleOrigins)
                {
                    AddManager( origin.Key, origin.Value );
                }
            }
        }

        public static ProjectInfo Resolve(string name, string version= "ANY")
        {
            return Managers.First(x=>x.Value.HasPackage(name)).Value.
                   GetPackage(name).
                   GetInstallTarget(version == "ANY" ? null :version);
        }


        public static ProjectInfo Resolve(string repository, ProjectDependency dependency)
        {
            return Managers[repository].
                   GetPackage(dependency.ProjectName).
                   GetInstallTarget(dependency.ProjectVersion == "ANY" ? null : dependency.ProjectVersion);
        }

        public static ProjectInfo Resolve(ModuleManager manager, ProjectDependency dependency)
        {
            return manager.GetPackage(dependency.ProjectName).
                           GetInstallTarget(dependency.ProjectVersion == "ANY" ? null : dependency.ProjectVersion);
        }

        #endregion

        #region Private

        static ModuleResolver()
        {
            Initialize();
        }

        #endregion

        //public static ModuleManager Manager;

    }

}
