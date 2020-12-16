using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU;
using VisCPU.HL;
using VisCPU.Peripherals;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;

namespace viscc
{

    public class ProgramRunner : ConsoleSubsystem
    {



        #region Public

        public override void Run(IEnumerable<string> args)
        {
            RunnerSettings settings = RunnerSettings.Create();
            ConsoleInInterfaceSettings cins = ConsoleInInterfaceSettings.Create();
            ConsoleOutInterfaceSettings couts = ConsoleOutInterfaceSettings.Create();
            HLCompilerSettings hls = HLCompilerSettings.Create();
            ArgumentSyntaxParser.Parse(
                                       args.ToArray(),
                                       settings,
                                       cins,
                                       couts,
                                       hls
                                      );

            Settings.SaveSettings(settings);
            Settings.SaveSettings(cins);
            Settings.SaveSettings(couts);
            Settings.SaveSettings( hls );
            
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
                    EventManager<ErrorEvent>.SendEvent(new FileNotFoundEvent(file, true));
                    continue;
                }

                Log($"Run File: '{file}'");
                uint[] fileCode = File.ReadAllBytes(file).ToUInt();

                Memory memory = new Memory(settings.MemorySize, 0);
                ConsoleInInterface cin = new ConsoleInInterface();

                ConsoleOutInterface cout =
                    new ConsoleOutInterface();

                MemoryBus bus = new MemoryBus(memory, cout, cin);

                CPU cpu = new CPU(bus, settings.CpuResetAddr, settings.CpuIntAddr);
                cpu.LoadBinary(fileCode);
                cpu.Run();
            }
        }

        #endregion

        #region Private

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
