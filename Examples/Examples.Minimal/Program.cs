using System;
using System.Linq;

using Examples.Shared;

namespace MinimalExample
{

    internal class Program
    {

        #region Private

        private static void Main( string[] args )
        {
            bool compile = args.Any( x => x == "-c" );
            bool clean = args.Any( x => x == "-clean" );
            bool run = args.Any( x => x == "-r" );

            FirstSetup.Start();

            string output = null;

            output = compile ? VisHelper.Compile( clean ) : FirstSetup.DefaultFile;

            if ( run )
            {
                if ( output == null )
                {
                    Console.WriteLine( $"Output file '{output}' not found." );
                }
                VisHelper.Default( output ).Run();
            }

            FirstSetup.End( EndOptions.Default );
        }

        #endregion

    }

}
