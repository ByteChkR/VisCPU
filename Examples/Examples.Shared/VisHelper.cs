using System.IO;

using VisCPU;
using VisCPU.Integration;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.Console.IO;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;

namespace Examples.Shared
{

    public static class VisHelper
    {

        #region Public

        public static string Compile( string file )
        {
            return Compile( file, false );
        }

        public static string Compile( string file, bool clean )
        {
            return CompilerHelper.Compile(
                                          file,
                                          FirstSetup.OutputDirectory,
                                          FirstSetup.InternalDirectory,
                                          clean,
                                          new[] { "HL-expr", "bin" }
                                         );
        }

        public static string Compile()
        {
            return Compile( FirstSetup.DefaultFile, false );
        }

        public static string Compile( bool clean )
        {
            return Compile( FirstSetup.DefaultFile, clean );
        }

        public static Cpu Default()
        {
            return Default( FirstSetup.DefaultOutput );
        }

        public static Cpu Default( string binary )
        {
            Cpu instance = DefaultBuilder().Build();

            instance.LoadBinary( File.ReadAllBytes( binary ).ToUInt() );

            return instance;
        }

        public static CpuInstanceBuilder DefaultBuilder()
        {
            return new CpuInstanceBuilder( 0, 0 ).WithPeripherals(
                                                                  new Memory(),
                                                                  new ConsoleInInterface(),
                                                                  new ConsoleInterface(),
                                                                  new ConsoleOutInterface()
                                                                 );
        }

        #endregion

    }

}
