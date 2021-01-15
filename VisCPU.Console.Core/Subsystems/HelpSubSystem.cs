using System;
using System.Collections.Generic;
using System.Text;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Console.Core.Settings;
using VisCPU.HL;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.HostFS;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems
{

    public class HelpSubSystem : ConsoleSubsystem
    {

        private readonly ConsoleSystem owner;

        #region Public

        public HelpSubSystem( ConsoleSystem owner )
        {
            this.owner = owner;
        }

        public override void Run( IEnumerable < string > arguments )
        {
            owner.Help();
        }

        public override void Help()
        {
        }

        #endregion

        #region Private
        
        
        public static void WriteSubsystem(string subName, params object[] settings)
        {
            System.Console.WriteLine($"Arguments: {subName}");

            IEnumerable<string> args = ArgumentSyntaxParser.GetArgNames(settings);

            foreach (string s1 in args)
            {
                System.Console.WriteLine("\tArg Name: " + s1);
            }
        }

        #endregion

    }

}
