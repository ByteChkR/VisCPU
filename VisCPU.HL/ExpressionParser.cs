using System;

using VisCPU.Utility;
using VisCPU.Utility.Logging;

namespace VisCPU.HL
{

    public class ExpressionParser : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Parser;

        #region Public

        public static void Cmd()
        {
            ExpressionParser p = new ExpressionParser();
            string cmd = null;
            string file = "";


            while ( true )
            {
                cmd = Console.ReadLine();

                if ( cmd == "exit" )
                {
                    return;
                }

                if ( cmd == "EOF" )
                {
                    BuildDataStore ds = new BuildDataStore("./", new HLBuildDataStore());
                    p.Parse( file, "./" , ds);
                    file = "";
                }
                else
                {
                    file += cmd;
                }
            }
        }

        public HLCompilation Parse( string expr, string dir, BuildDataStore store )
        {
            HLCompilation c = new HLCompilation( expr, dir, store );

            return c;
        }

        #endregion

    }

}
