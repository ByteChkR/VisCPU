using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Database;
using VisCPU.Utility;
using VisCPU.Utility.Logging;

namespace VisCPU.ProjectSystem.Data
{

    public class ProjectConfig
    {
        private static readonly Dictionary < string, BuildJobRunner > s_BuildJobRunners =
            new Dictionary < string, BuildJobRunner >();

        [JsonIgnore]
        [XmlIgnore]
        public ProjectDatabase Manager { get; }

        public string ProjectName { get; set; } = "MyProject";

        public string ProjectVersion { get; set; } = "0.0.0.1";

        public List < ProjectDependency > Dependencies { get; } = new List < ProjectDependency >();

        public string DefaultTarget { get; } = "Debug";

        public Dictionary < string, ProjectBuildTarget > BuildTargets { get; } =
            new Dictionary < string, ProjectBuildTarget >
            {
                {
                    "Debug", new ProjectBuildTarget
                    {
                        TargetName = "Debug",
                        Jobs = new List < ProjectBuildJob >
                        {
                            new ProjectBuildJob
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

        #region Public

        public ProjectConfig(
            ProjectDatabase manager,
            string projectName,
            string projectVersion,
            ProjectDependency[] dependencies )
        {
            ProjectName = projectName;
            ProjectVersion = projectVersion;
            Manager = manager;
            Dependencies = dependencies.ToList();
        }

        public ProjectConfig()
        {
        }

        public static void AddRunner( BuildJobRunner runner )
        {
            if ( s_BuildJobRunners.ContainsKey( runner.RunnerName ) )
            {
                return;
            }

            s_BuildJobRunners[runner.RunnerName] = runner;
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

        public void RunJob(
            string rootDir,
            ProjectBuildTarget buildTarget,
            ProjectBuildJob job,
            bool writeDebug = false )
        {
            if ( !s_BuildJobRunners.ContainsKey( job.BuildJobRunner ) )
            {
                Logger.LogMessage( LoggerSystems.ModuleSystem, "Can not Find Job Runner: {0}", job.BuildJobRunner );

                return;
            }

            ResolveBuildJobItems( rootDir, buildTarget, job );

            if ( writeDebug )
            {
                Logger.LogMessage( LoggerSystems.ModuleSystem, "Running Job: {0}", job.JobName );
            }

            s_BuildJobRunners[job.BuildJobRunner].RunJob( rootDir, this, buildTarget, job );
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

            foreach ( ProjectBuildJob buildTargetJob in buildTarget.Jobs )
            {
                RunJob( rootDir, buildTarget, buildTargetJob, true );
            }
        }

        #endregion

        #region Private

        private void ResolveBuildJobItems( string rootDir, ProjectBuildTarget buildTarget, ProjectBuildJob job )
        {
            Dictionary < string, string > varMap = new Dictionary < string, string >
            {
                { "VISDIR",  UnityIsAPieceOfShitHelper.AppRoot  },
                {
                    "PROJDIR", rootDir.EndsWith( "\\" ) ||
                               rootDir.EndsWith( "/" )
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
                { "VISDIR",  UnityIsAPieceOfShitHelper.AppRoot  },
                {
                    "PROJDIR", rootDir.EndsWith( "\\" ) ||
                               rootDir.EndsWith( "/" )
                        ? rootDir
                        : rootDir + "/"
                },
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
