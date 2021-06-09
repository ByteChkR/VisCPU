using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.Compiler.Linking;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;
using VisCPU.Utility.SharedBase;

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
            ProjectBuildJob job)
        {
            string old = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(projectRoot);

            

            ProgramBuilder.Build(job.Arguments);



            if ( job.Arguments.TryGetValue( "export:labels", out string headerFiles ) )
            {
                string headerFile = Path.Combine(projectRoot, "header.vhlheader");
                string[] list = headerFiles.Split( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries ).
                                            Select( x => x.Trim() ).
                                            ToArray();

                BuildJobMakeHeader.MakeHeader( headerFile, list );
            }

            Directory.SetCurrentDirectory(old);
        }

        #endregion

    }
    public class BuildJobMakeHeader : BuildJobRunner
    {

        public override string RunnerName => "make-header";

        #region Public

        public static void MakeHeader( string outputFile, string[] linkerFiles )
        {
            List<LinkerInfo> infos = new List<LinkerInfo>();

            foreach (string path in linkerFiles)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    infos.Add(LinkerInfo.Load(path));
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (LinkerInfo linkerInfo in infos)
            {
                foreach (KeyValuePair<string, AddressItem> linkerInfoLabel in linkerInfo.Labels)
                {
                    sb.AppendLine(linkerInfoLabel.Key);
                }
            }
            File.WriteAllText(outputFile, sb.ToString());
        }

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job)
        {
            if ( job.Arguments.TryGetValue( "files", out string linkerPath ) )
            {
                string[] list = linkerPath.Split( new []{';'}, StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).ToArray();


                string headerFile = Path.Combine(projectRoot, "header.vhlheader");
                MakeHeader( headerFile, list );
            }
        }

        #endregion

    }

}
