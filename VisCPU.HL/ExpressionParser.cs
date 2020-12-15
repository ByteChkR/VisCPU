using System;

using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.HL
{
    public class ExpressionParser : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Parser;

        public static void Cmd()
        {
            ExpressionParser p = new ExpressionParser();
            string cmd = null;
            string file = "";
            while (true)
            {
                cmd = Console.ReadLine();
                if (cmd == "exit")
                {
                    return;
                }

                if (cmd == "EOF")
                {
                    p.Parse(file, "./");
                    file = "";
                }
                else
                {
                    file += cmd;
                }
            }
        }

        public HLCompilation Parse(string expr, string dir)
        {
            HLCompilation c = new HLCompilation(expr, dir);

            return c;
        }

    }
}