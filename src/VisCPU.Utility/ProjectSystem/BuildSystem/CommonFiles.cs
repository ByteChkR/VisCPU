using System.IO;

using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Utility.ProjectSystem.BuildSystem
{

    public static class CommonFiles
    {

        #region Public

        public static void GenerateCommonFiles()
        {
            GenerateCommonJobs();
        }

        public static ProjectConfig GenerateProjectConfig()
        {
            return GenerateProjectConfig( null, null );
        }

        public static ProjectConfig GenerateProjectConfig( string name )
        {
            return GenerateProjectConfig( name, null );
        }

        public static ProjectConfig GenerateProjectConfig( string name, string version )
        {
            ProjectConfig config = new ProjectConfig();

            if ( name != null )
            {
                config.ProjectName = name;
            }

            if ( version != null )
            {
                config.ProjectVersion = version;
            }

            ProjectBuildTarget debugTarget = CreateDebugTarget();

            ProjectBuildTarget debugRunTarget = CreateDebugRunTarget();

            ProjectBuildTarget releaseTarget = CreateReleaseTarget();

            ProjectBuildTarget releaseRunTarget = CreateReleaseRunTarget();

            ProjectBuildTarget runTarget = CreateRunTarget();

            ProjectBuildTarget publishTarget = new ProjectBuildTarget();
            publishTarget.TargetName = "Publish";
            ProjectBuildJob publishJob = new ProjectBuildJob();
            publishJob.JobName = "Publish Project";
            publishJob.BuildJobRunner = "merged";
            publishJob.Arguments["merge:include"] = "%VISDIR%common/jobs/newVersion.json";
            publishTarget.Jobs.Add( publishJob );
            config.BuildTargets["Publish"] = publishTarget;
            config.BuildTargets["Debug"] = debugTarget;
            config.BuildTargets["DebugRun"] = debugRunTarget;
            config.BuildTargets["Release"] = releaseTarget;
            config.BuildTargets["ReleaseRun"] = releaseRunTarget;
            config.BuildTargets["Run"] = runTarget;

            return config;
        }

        public static void InitializeProjectFolder( string rootDir )
        {
            string name = Path.GetFileName( rootDir );
            ProjectConfig config = GenerateProjectConfig( name );
            ProjectConfig.Save( Path.Combine( rootDir, "project.json" ), config );

            if ( !File.Exists( Path.Combine( rootDir, "Program.vhl" ) ) )
            {
                File.WriteAllText(
                                  Path.Combine( rootDir, "Program.vhl" ),
                                  @"//Entry Point of Project"
                                 );
            }
        }

        #endregion

        #region Private

        private static ProjectBuildJob CreateDebugBuildJob()
        {
            ProjectBuildJob debugBuildJob = new ProjectBuildJob();
            debugBuildJob.JobName = "Build %NAME%@%VERSION%";
            debugBuildJob.BuildJobRunner = "build";
            debugBuildJob.Arguments["build:input"] = "Program.vhl";
            debugBuildJob.Arguments["build:steps"] = "HL-expr bin";
            debugBuildJob.Arguments["build:clean"] = "false";
            debugBuildJob.Arguments["assembler:offset.global"] = "0";
            debugBuildJob.Arguments["assembler:format"] = "v1";
            debugBuildJob.Arguments["linker:export"] = "true";
            debugBuildJob.Arguments["linker:no-hide"] = "true";
            debugBuildJob.Arguments["compiler:optimize-temp-vars"] = "false";
            debugBuildJob.Arguments["compiler:optimize-const-expr"] = "false";
            debugBuildJob.Arguments["compiler:optimize-reduce-expr"] = "false";
            debugBuildJob.Arguments["compiler:optimize-if-expr"] = "false";
            debugBuildJob.Arguments["compiler:optimize-while-expr"] = "false";
            debugBuildJob.Arguments["compiler:strip-unused-functions"] = "false";

            debugBuildJob.Arguments["compiler:constructor-prolog-mode"] = "Inline";

            return debugBuildJob;
        }

        private static ProjectBuildJob CreateDebugRunJob()
        {
            ProjectBuildJob debugRunJob = new ProjectBuildJob();
            debugRunJob.JobName = "Build %NAME%@%VERSION%";
            debugRunJob.BuildJobRunner = "run";
            debugRunJob.Arguments["run:input"] = "Program.vbin";
            debugRunJob.Arguments["run:cpu.interrupt"] = "0x00000000";
            debugRunJob.Arguments["run:cpu.reset"] = "0x00000000";
            debugRunJob.Arguments["run:working-dir"] = "%PROJDIR%";
            debugRunJob.Arguments["run:trim"] = "false";
            debugRunJob.Arguments["memory-bus:devices"] = "%VISDIR%configs/cpu/peripherals/memory/default.json";

            debugRunJob.Arguments["memory:read"] = "true";
            debugRunJob.Arguments["memory:write"] = "true";
            debugRunJob.Arguments["memory:persistent"] = "false";

            debugRunJob.Arguments["memory:persistent.path"] =
                "%VISDIR%configs/cpu/peripherals/memory/states/default.bin";

            debugRunJob.Arguments["memory:size"] = "262144";

            return debugRunJob;
        }

        private static ProjectBuildTarget CreateDebugRunTarget()
        {
            ProjectBuildTarget debug = new ProjectBuildTarget();
            debug.TargetName = "DebugRun";

            debug.DependsOn = new[] { "Debug" };

            ProjectBuildJob mergeRunJob = new ProjectBuildJob();
            mergeRunJob.JobName = "Merged Debug Run";
            mergeRunJob.BuildJobRunner = "merged";
            mergeRunJob.Arguments["merge:include"] = "%VISDIR%common/jobs/debug_run.json";
            mergeRunJob.Arguments["run:input"] = "Program.vbin";
            debug.Jobs.Add( mergeRunJob );

            return debug;
        }

        private static ProjectBuildTarget CreateDebugTarget()
        {
            ProjectBuildTarget debug = new ProjectBuildTarget();
            debug.TargetName = "Debug";
            ProjectBuildJob restoreJob = new ProjectBuildJob();
            restoreJob.BuildJobRunner = "restore";
            restoreJob.JobName = "Restore %NAME%@%VERSION%";
            restoreJob.Arguments["origin"] = "local";
            debug.Jobs.Add( restoreJob );

            ProjectBuildJob mergeBuildJob = new ProjectBuildJob();
            mergeBuildJob.JobName = "Merged Debug Build";
            mergeBuildJob.BuildJobRunner = "merged";
            mergeBuildJob.Arguments["merge:include"] = "%VISDIR%common/jobs/debug_build.json";
            mergeBuildJob.Arguments["build:input"] = "Program.vhl";
            debug.Jobs.Add( mergeBuildJob );

            return debug;
        }

        private static ProjectBuildJob CreateReleaseBuildJob()
        {
            ProjectBuildJob debugBuildJob = new ProjectBuildJob();
            debugBuildJob.JobName = "Build %NAME%@%VERSION%";
            debugBuildJob.BuildJobRunner = "build";
            debugBuildJob.Arguments["build:input"] = "Program.vhl";
            debugBuildJob.Arguments["build:steps"] = "HL-expr bin";
            debugBuildJob.Arguments["build:clean"] = "false";
            debugBuildJob.Arguments["assembler:offset.global"] = "0";
            debugBuildJob.Arguments["assembler:format"] = "v1";
            debugBuildJob.Arguments["linker:export"] = "true";
            debugBuildJob.Arguments["linker:no-hide"] = "false";
            debugBuildJob.Arguments["compiler:optimize-temp-vars"] = "true";
            debugBuildJob.Arguments["compiler:optimize-const-expr"] = "true";
            debugBuildJob.Arguments["compiler:optimize-reduce-expr"] = "true";
            debugBuildJob.Arguments["compiler:optimize-if-expr"] = "true";
            debugBuildJob.Arguments["compiler:optimize-while-expr"] = "true";
            debugBuildJob.Arguments["compiler:strip-unused-functions"] = "false";
            debugBuildJob.Arguments["compiler:constructor-prolog-mode"] = "Inline";

            return debugBuildJob;
        }

        private static ProjectBuildJob CreateReleaseRunJob()
        {
            ProjectBuildJob runJob = new ProjectBuildJob();
            runJob.JobName = "Run %NAME%@%VERSION%";
            runJob.BuildJobRunner = "run";
            runJob.Arguments["run:input"] = "Program.vbin";
            runJob.Arguments["run:cpu.interrupt"] = "0x00000000";
            runJob.Arguments["run:cpu.reset"] = "0x00000000";
            runJob.Arguments["run:working-dir"] = "%PROJDIR%";
            runJob.Arguments["run:trim"] = "false";
            runJob.Arguments["memory-bus:devices"] = "%VISDIR%configs/cpu/peripherals/memory/default.json";

            runJob.Arguments["memory:read"] = "true";
            runJob.Arguments["memory:write"] = "true";
            runJob.Arguments["memory:persistent"] = "false";
            runJob.Arguments["memory:persistent.path"] = "%VISDIR%configs/cpu/peripherals/memory/states/default.bin";
            runJob.Arguments["memory:size"] = "262144";

            return runJob;
        }

        private static ProjectBuildTarget CreateReleaseRunTarget()
        {
            ProjectBuildTarget release = new ProjectBuildTarget();
            release.TargetName = "ReleaseRun";

            release.DependsOn = new[] { "Release" };

            ProjectBuildJob mergeRunJob = new ProjectBuildJob();
            mergeRunJob.JobName = "Merged Release Run";
            mergeRunJob.BuildJobRunner = "merged";
            mergeRunJob.Arguments["merge:include"] = "%VISDIR%common/jobs/release_run.json";
            mergeRunJob.Arguments["run:input"] = "Program.vbin";
            release.Jobs.Add( mergeRunJob );

            return release;
        }

        private static ProjectBuildTarget CreateReleaseTarget()
        {
            ProjectBuildTarget release = new ProjectBuildTarget();
            release.TargetName = "Release";
            ProjectBuildJob restoreJob = new ProjectBuildJob();
            restoreJob.BuildJobRunner = "restore";
            restoreJob.JobName = "Restore %NAME%@%VERSION%";
            restoreJob.Arguments["origin"] = "local";
            release.Jobs.Add( restoreJob );

            ProjectBuildJob mergeBuildJob = new ProjectBuildJob();
            mergeBuildJob.JobName = "Merged Release Build";
            mergeBuildJob.BuildJobRunner = "merged";
            mergeBuildJob.Arguments["merge:include"] = "%VISDIR%common/jobs/release_build.json";
            mergeBuildJob.Arguments["build:input"] = "Program.vhl";
            release.Jobs.Add( mergeBuildJob );

            return release;
        }

        private static ProjectBuildTarget CreateRunTarget()
        {
            ProjectBuildTarget release = new ProjectBuildTarget();
            release.TargetName = "Run";

            release.DependsOn = new string[0];

            ProjectBuildJob mergeRunJob = new ProjectBuildJob();
            mergeRunJob.JobName = "Merged Run";
            mergeRunJob.BuildJobRunner = "merged";
            mergeRunJob.Arguments["merge:include"] = "%VISDIR%common/jobs/release_run.json";
            mergeRunJob.Arguments["run:input"] = "Program.vbin";
            release.Jobs.Add( mergeRunJob );

            return release;
        }

        private static void GenerateCommonJobs()
        {
            string dir = Path.Combine( AppRootHelper.AppRoot, "common/jobs" );
            Directory.CreateDirectory( dir );
            ProjectBuildJob cleanJob = new ProjectBuildJob();
            cleanJob.BuildJobRunner = "clean";
            cleanJob.JobName = "Clean Project %NAME%@%VERSION%";

            ProjectBuildJob restoreJob = new ProjectBuildJob();
            restoreJob.BuildJobRunner = "restore";
            restoreJob.JobName = "Restore Project %NAME%@%VERSION%";

            ProjectBuildJob publishJob = new ProjectBuildJob();
            publishJob.BuildJobRunner = "publish";
            publishJob.JobName = "Publish Project %NAME%@%VERSION%";
            publishJob.Arguments["repo"] = "local";

            ProjectBuildJob newVersionJob = new ProjectBuildJob();
            newVersionJob.BuildJobRunner = "combined";
            newVersionJob.JobName = "New Version Project %NAME%";
            newVersionJob.Arguments["publish"] = "%VISDIR%common/jobs/publish.json";
            newVersionJob.Arguments["restore"] = "%VISDIR%common/jobs/restore.json";

            ProjectBuildJob dBuildJob = CreateDebugBuildJob();
            ProjectBuildJob rBuildJob = CreateReleaseBuildJob();
            ProjectBuildJob dRunJob = CreateDebugRunJob();
            ProjectBuildJob rRunJob = CreateReleaseRunJob();

            ProjectBuildJob.Save( Path.Combine( dir, "debug_build.json" ), dBuildJob );
            ProjectBuildJob.Save( Path.Combine( dir, "release_build.json" ), rBuildJob );
            ProjectBuildJob.Save( Path.Combine( dir, "debug_run.json" ), dRunJob );
            ProjectBuildJob.Save( Path.Combine( dir, "release_run.json" ), rRunJob );

            ProjectBuildJob.Save( Path.Combine( dir, "clean.json" ), cleanJob );
            ProjectBuildJob.Save( Path.Combine( dir, "restore.json" ), restoreJob );
            ProjectBuildJob.Save( Path.Combine( dir, "publish.json" ), publishJob );
            ProjectBuildJob.Save( Path.Combine( dir, "newVersion.json" ), newVersionJob );
        }

        #endregion

    }

}
