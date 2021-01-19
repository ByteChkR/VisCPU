﻿using System;
using System.Diagnostics;
using System.IO;
using VisCPU.Console.Core.Settings;
using VisCPU.Console.Core.Subsystems;
using VisCPU.Utility.Logging;



public class CompilerHelper
{
    
    public static string Compile(string file, string outputBuildFolder, string tempBuildFolder, bool cleanOutput, string[] buildSteps)
    {

        BuilderSettings.BuildTempDirectory = Path.GetFullPath(tempBuildFolder);

        if (!Directory.Exists(outputBuildFolder)) Directory.CreateDirectory(outputBuildFolder);
        Logger.LogMessage(LoggerSystems.HlIntegration,"Current Working Dir: " + AppDomain.CurrentDomain.BaseDirectory);
        Logger.LogMessage(LoggerSystems.HlIntegration, "Vis API Build Result Directory: " + Path.GetFullPath(outputBuildFolder));
        if (!Directory.Exists(outputBuildFolder)) Directory.CreateDirectory(outputBuildFolder);

        BuilderSettings bs = new BuilderSettings();
        bs.CleanBuildOutput = cleanOutput;
        bs.EntryFiles = new[] { file };
        bs.Steps = buildSteps;
        ProgramBuilder.Build(bs);
        string buildOut = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".vbin");
        string buildTarget = Path.Combine(outputBuildFolder, Path.GetFileNameWithoutExtension(file) + ".vbin");
        if(File.Exists(buildTarget))File.Delete(buildTarget);
        File.Move(buildOut, buildTarget);
        return buildTarget;
    }
}