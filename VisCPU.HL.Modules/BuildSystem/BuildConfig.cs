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

    }

    public class ProjectBuildTarget
    {

        public string TargetName;
        public List < BuildJob > Jobs = new List < BuildJob >();

    }

    public abstract class BuildJobRunner
    {

        public abstract string RunnerName { get; }
        public abstract void RunJob(string projectRoot, ProjectConfig project, ProjectBuildTarget target, BuildJob job );

    }


    public class ProjectConfig : ProjectInfo
    {

        private static readonly Dictionary < string, BuildJobRunner > BuildJobRunners =
            new Dictionary < string, BuildJobRunner>();

        public static void AddRunner( BuildJobRunner runner)
        {
            if ( BuildJobRunners.ContainsKey( runner.RunnerName ) )
                return;
            BuildJobRunners[runner.RunnerName] = runner;
        }


        public ProjectConfig(ModuleManager manager, string projectName, string projectVersion, ProjectDependency[] dependencies) : base(manager, projectName, projectVersion, dependencies)
        {

        }
        public ProjectConfig()
        {

        }


        public string DefaultTarget = "Debug";
        public Dictionary < string, ProjectBuildTarget > BuildTargets = new Dictionary < string, ProjectBuildTarget>
                                                                        {

                                                                            {
                                                                                "Debug", new ProjectBuildTarget()
                                                                                    {
                                                                                        TargetName = "Debug",
                                                                                        Jobs = new List < BuildJob > 
                                                                                            {
                                                                                                new BuildJob
                                                                                                {
                                                                                                    JobName = "Clean Project Folder",
                                                                                                    BuildJobRunner = "clean"
                                                                                                }
                                                                                            }
                                                                                    }
                                                                            }
                                                                        };

        public void RunTarget(string rootDir, string target = null)
        {
            string t = target ?? DefaultTarget;
            if(!BuildTargets.ContainsKey(target))
            {
                Logger.LogMessage(LoggerSystems.ModuleSystem, "Can not Find Target: {0}", target);

                return;
            }
            ProjectBuildTarget buildTarget = BuildTargets[t];

            foreach ( BuildJob buildTargetJob in buildTarget.Jobs )
            {
                Logger.LogMessage( LoggerSystems.ModuleSystem, "Running Job: {0}", buildTargetJob.JobName );
                if (!BuildJobRunners.ContainsKey(buildTargetJob.BuildJobRunner))
                {
                    Logger.LogMessage(LoggerSystems.ModuleSystem, "Can not Find Job Runner: {0}", target);
                    continue;
                }
                BuildJobRunners[buildTargetJob.BuildJobRunner].RunJob(rootDir, this, buildTarget, buildTargetJob);
            }
        }

        public static string Serialize(ProjectConfig config)
        {
            return JsonConvert.SerializeObject(config,
                                               Formatting.Indented);
        }
        public static ProjectConfig Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<ProjectConfig>(data);
        }

        public static void Save(string path, ProjectConfig config)
        {
            File.WriteAllText( path, Serialize( config ) );
        }
        public static ProjectConfig Load(string path)
        {
            return Deserialize(File.ReadAllText(path));
        }

    }

}
