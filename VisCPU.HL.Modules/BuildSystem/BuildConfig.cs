using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.BuildSystem
{

    public class BuildJob
    {

        public string JobName;
        public string BuildJobRunner;
        public Dictionary < string, string > Arguments = new Dictionary < string, string >();

        #region Public

        public static BuildJob Deserialize( string data )
        {
            return JsonConvert.DeserializeObject < BuildJob >( data );
        }

        public static BuildJob Load( string path )
        {
            return Deserialize( File.ReadAllText( path ) );
        }

        public static void Save( string path, BuildJob config )
        {
            File.WriteAllText( path, Serialize( config ) );
        }

        public static string Serialize( BuildJob config )
        {
            return JsonConvert.SerializeObject(
                                               config,
                                               Formatting.Indented
                                              );
        }

        #endregion

    }

    public class ProjectBuildTarget
    {

        public string TargetName;
        public string[] DependsOn = new string[0];
        public List < BuildJob > Jobs = new List < BuildJob >();

        #region Public

        public static ProjectBuildTarget Deserialize( string data )
        {
            return JsonConvert.DeserializeObject < ProjectBuildTarget >( data );
        }

        public static ProjectBuildTarget Load( string path )
        {
            return Deserialize( File.ReadAllText( path ) );
        }

        public static void Save( string path, ProjectBuildTarget config )
        {
            File.WriteAllText( path, Serialize( config ) );
        }

        public static string Serialize( ProjectBuildTarget config )
        {
            return JsonConvert.SerializeObject(
                                               config,
                                               Formatting.Indented
                                              );
        }

        #endregion

    }

    public abstract class BuildJobRunner
    {

        public abstract string RunnerName { get; }

        #region Public

        public abstract void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job );

        #endregion

    }

    public class ProjectConfig : ProjectInfo
    {

        public string DefaultTarget = "Debug";

        public Dictionary < string, ProjectBuildTarget > BuildTargets = new Dictionary < string, ProjectBuildTarget >
                                                                        {
                                                                            {
                                                                                "Debug", new ProjectBuildTarget()
                                                                                    {
                                                                                        TargetName = "Debug",
                                                                                        Jobs = new List < BuildJob >
                                                                                            {
                                                                                                new BuildJob
                                                                                                {
                                                                                                    JobName =
                                                                                                        "Clean Project Folder",
                                                                                                    BuildJobRunner =
                                                                                                        "clean"
                                                                                                }
                                                                                            }
                                                                                    }
                                                                            }
                                                                        };

        private static readonly Dictionary < string, BuildJobRunner > BuildJobRunners =
            new Dictionary < string, BuildJobRunner >();

        #region Public

        public ProjectConfig(
            ModuleManager manager,
            string projectName,
            string projectVersion,
            ProjectDependency[] dependencies ) : base( manager, projectName, projectVersion, dependencies )
        {
        }

        public ProjectConfig()
        {
        }

        public static void AddRunner( BuildJobRunner runner )
        {
            if ( BuildJobRunners.ContainsKey( runner.RunnerName ) )
            {
                return;
            }

            BuildJobRunners[runner.RunnerName] = runner;
        }

        public static ProjectConfig Deserialize( string data )
        {
            return JsonConvert.DeserializeObject < ProjectConfig >( data );
        }

        public static ProjectConfig Load( string path )
        {
            return Deserialize( File.ReadAllText( path ) );
        }

        public static void Save( string path, ProjectConfig config )
        {
            File.WriteAllText( path, Serialize( config ) );
        }

        public static string Serialize( ProjectConfig config )
        {
            return JsonConvert.SerializeObject(
                                               config,
                                               Formatting.Indented
                                              );
        }

        public void RunJob( string rootDir, ProjectBuildTarget buildTarget, BuildJob job, bool writeDebug = false )
        {
            if ( !BuildJobRunners.ContainsKey( job.BuildJobRunner ) )
            {
                Logger.LogMessage( LoggerSystems.ModuleSystem, "Can not Find Job Runner: {0}", job.BuildJobRunner );

                return;
            }

            ResolveBuildJobItems( rootDir, buildTarget, job );

            if ( writeDebug )
            {
                Logger.LogMessage( LoggerSystems.ModuleSystem, "Running Job: {0}", job.JobName );
            }

            BuildJobRunners[job.BuildJobRunner].RunJob( rootDir, this, buildTarget, job );
        }

        public void RunTarget( string rootDir, string target = null )
        {
            string t = target ?? DefaultTarget;

            if ( !BuildTargets.ContainsKey( target ) )
            {
                Logger.LogMessage( LoggerSystems.ModuleSystem, "Can not Find Target: {0}", target );

                return;
            }

            ProjectBuildTarget buildTarget = BuildTargets[t];

            RunTarget( rootDir, buildTarget );
        }

        public void RunTarget( string rootDir, ProjectBuildTarget buildTarget )
        {
            ResolveBuildJobItems( rootDir, buildTarget );

            foreach ( string dependency in buildTarget.DependsOn )
            {
                if ( File.Exists( dependency ) )
                {
                    RunTarget( rootDir, ProjectBuildTarget.Load( dependency ) );
                }
                else
                {
                    RunTarget( rootDir, dependency );
                }
            }

            foreach ( BuildJob buildTargetJob in buildTarget.Jobs )
            {
                RunJob( rootDir, buildTarget, buildTargetJob, true );
            }
        }

        #endregion

        #region Private

        private void ResolveBuildJobItems( string rootDir, ProjectBuildTarget buildTarget, BuildJob job )
        {
            Dictionary < string, string > varMap = new Dictionary < string, string >
                                                   {
                                                       { "VISDIR", AppDomain.CurrentDomain.BaseDirectory },
                                                       {
                                                           "PROJDIR",
                                                           rootDir.EndsWith( "\\" ) || rootDir.EndsWith( "/" )
                                                               ? rootDir
                                                               : rootDir + "/"
                                                       },
                                                       { "NAME", ProjectName },
                                                       { "VERSION", ProjectVersion },
                                                       { "TARGET", buildTarget.TargetName },
                                                   };

            foreach ( string varMapKey in varMap.Keys )
            {
                job.JobName = job.JobName.Replace( $"%{varMapKey}%", varMap[varMapKey] );

                foreach ( string argumentsKey in job.Arguments.Keys )
                {
                    job.Arguments[argumentsKey] =
                        job.Arguments[argumentsKey].Replace( $"%{varMapKey}%", varMap[varMapKey] );
                }
            }
        }

        private void ResolveBuildJobItems( string rootDir, ProjectBuildTarget buildTarget )
        {
            Dictionary < string, string > varMap = new Dictionary < string, string >
                                                   {
                                                       { "VISDIR", AppDomain.CurrentDomain.BaseDirectory },
                                                       { "PROJDIR", rootDir },
                                                       { "NAME", ProjectName },
                                                       { "VERSION", ProjectVersion },
                                                       { "TARGET", buildTarget.TargetName },
                                                   };

            foreach ( string varMapKey in varMap.Keys )
            {
                buildTarget.TargetName = buildTarget.TargetName.Replace( $"%{varMapKey}%", varMap[varMapKey] );

                for ( int i = 0; i < buildTarget.DependsOn.Length; i++ )
                {
                    buildTarget.DependsOn[i] =
                        buildTarget.DependsOn[i].Replace( $"%{varMapKey}%", varMap[varMapKey] );
                }
            }
        }

        #endregion

    }

}
