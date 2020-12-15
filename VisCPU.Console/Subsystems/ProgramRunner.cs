using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using VisCPU;
using VisCPU.Peripherals;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace viscc
{
    

    public class ProgramRunner : ConsoleSubsystem
    {

        public class RunnerSettings
        {

            [Argument(Name = "console.in")]
            public uint ConsoleInAddr = 0xFFFF1000;

            [Argument(Name = "console.out.char")]
            public uint ConsoleOutAddr = 0xFFFF1001;

            [Argument(Name = "console.out.num")]
            public uint ConsoleOutNumAddr = 0xFFFF1002;

            [Argument(Name = "cpu.interrupt")]
            public uint CpuIntAddr = 0x00000000;

            [Argument(Name = "cpu.reset")]
            public uint CpuResetAddr = 0x00000000;

            public Dictionary<string, Func<string, string>> PreRunMap = new()
                                                                        {
                                                                            { ".z", UnCompressFile },
                                                                            { ".vasm", FindBinary },
                                                                            { ".vhl", FindBinary },
                                                                        };

            private static string FindBinary( string arg )
            {
                string name = Path.Combine( Path.GetDirectoryName( arg ), Path.GetFileNameWithoutExtension( arg ) );
                string rawBin =name + ".vbin";
                string compBin = name + ".vbin.z";

                if ( File.Exists( rawBin ) )
                    return rawBin;

                if ( File.Exists( compBin ) )
                    return compBin;

                return null;
            }

            [Argument(Name = "memory.size")]
            public uint MemorySize = 0xFFFF + 1;

            [Argument(Name = "help")]
            [Argument(Name = "h")]
            public bool PrintHelp;
            

            [Argument(Name = "input-files")]
            [Argument(Name = "i")]
            private string[] inputFiles;

            [Argument(Name = "input-folders")]
            [Argument(Name = "if")]
            private string[] inputFolders;

            public string[] InputFiles
            {
                get
                {
                    List<string> ret = new List<string>();

                    if (inputFolders != null)
                    {
                        ret.AddRange(inputFolders.SelectMany(x => Directory.GetFiles(x, "*.txt")));
                    }

                    if (inputFiles != null)
                    {
                        ret.AddRange(inputFiles);
                    }

                    return ret.ToArray();
                }
            }

            #region Private

            private static string UnCompressFile(string originalfile)
            {
                string newFile = originalfile.Remove(originalfile.Length - 2, 2);

                using Stream input = File.OpenRead(originalfile);

                using Stream output = File.Create(newFile);

                using Stream s = new GZipStream(input, CompressionMode.Decompress);

                s.CopyTo(output);

                return newFile;
            }

            #endregion

        }

        #region Public

        public override void Run(IEnumerable<string> args)
        {
            RunnerSettings settings = new RunnerSettings();

            ArgumentSyntaxParser.Parse(
                                       args.ToArray(),
                                       settings
                                      );
            

            if (settings.PrintHelp)
            {
                PrintHelp(settings);
            }

            if (settings.InputFiles == null)
            {
                return;
            }

            foreach (string f in settings.InputFiles)
            {
                string file = Path.GetFullPath(f);

                file = RunPreRunSteps(settings, file);

                if (file == null || !File.Exists(file))
                {
                    EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( file, true ) );
                    continue;
                }

                Log($"Run File: '{file}'");
                uint[] fileCode = File.ReadAllBytes(file).ToUInt();

                Memory memory = new Memory(settings.MemorySize, 0);
                ConsoleInInterface cin = new ConsoleInInterface(settings.ConsoleInAddr);

                ConsoleOutInterface cout =
                    new ConsoleOutInterface(settings.ConsoleOutAddr, settings.ConsoleOutNumAddr);

                MemoryBus bus = new MemoryBus(memory, cout, cin);

                CPU cpu = new CPU(bus, settings.CpuResetAddr, settings.CpuIntAddr);
                cpu.LoadBinary(fileCode);
                cpu.Run();
            }
        }

        #endregion

        #region Private

        private static void PrintHelp(RunnerSettings s)
        {
            IEnumerable<string> args = ArgumentSyntaxParser.GetArgNames(s);

            foreach (string s1 in args)
            {
                Console.WriteLine("Arg Name: " + s1);
            }
            Console.WriteLine("-log Subsystems: ");
            string[] names = Enum.GetNames<LoggerSystems>();

            foreach (string name in names)
            {
                Console.WriteLine("\t" + name);
            }
        }

        private string RunPreRunSteps(RunnerSettings settings, string file)
        {
            string ret = file;

            foreach (KeyValuePair<string, Func<string, string>> keyValuePair in settings.PreRunMap)
            {
                if (Path.GetExtension(ret) == keyValuePair.Key)
                {
                    ret = RunPreRunSteps(settings, keyValuePair.Value(ret));
                }
            }

            return ret;
        }

        #endregion

    }

}
