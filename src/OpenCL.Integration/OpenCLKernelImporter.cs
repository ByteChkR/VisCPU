using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using OpenCL.Wrapper;
using OpenCL.Wrapper.TypeEnums;

using VisCPU.HL;
using VisCPU.HL.Importer;

namespace OpenCL.Integration
{

    public class OpenCLKernelImporter : AImporter, IFileImporter
    {

        private readonly string m_DeviceDriverDirectory;

        #region Public

        public OpenCLKernelImporter()
        {
            m_DeviceDriverDirectory = Path.Combine( CacheDirectory, "cl-cache" );
            Directory.CreateDirectory( m_DeviceDriverDirectory );
        }

        public override bool CanImport( string input )
        {
            return input.StartsWith( "CL" );
        }

        public IncludedItem ProcessImport( string input )
        {
            string dir = input.Remove( 0, 3 ); //Remove "CL "

            Log( "Compiling CL Programs in directory {0}", dir );
            KernelDatabase dB = new KernelDatabase( CLAPI.MainThread, dir, DataVectorTypes.Uchar1 );

            Log( "Generating HL Wrapper for {0} Kernels in {1} Programs", dB.KernelCount, dB.ProgramCount );
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( "public class CL\n{" );
            int count = 0;

            foreach ( CLProgram clProgram in dB.Programs )
            {
                string progInitFunc = $"_CL_INIT_{count}";
                sb.Append( EmbedProgramSource( clProgram, progInitFunc ) );
                count++;

                foreach ( KeyValuePair < string, CLKernel > clProgramContainedKernel in clProgram.ContainedKernels )
                {
                    sb.Append( GenerateKernelWrapper( progInitFunc, clProgramContainedKernel.Value ) );
                }
            }

            sb.AppendLine( "\n}\n" );

            sb.AppendLine( GetStaticTypes() );

            string file = Path.Combine( m_DeviceDriverDirectory, "FL_CL_Integration.vhl" );
            File.WriteAllText( file, sb.ToString() );

            return new IncludedItem( file, false );
        }

        #endregion

        #region Private

        private string EmbedProgramSource( CLProgram prog, string progInit )
        {
            string header = $"private void {progInit}()";

            string body = "string src = \"{0}\";";

            return header +
                   "\n{\n" +
                   string.Format( body, Convert.ToBase64String( Encoding.UTF8.GetBytes( prog.Source ) ) ) +
                   "\n}\n";
        }

        private string GenerateKernelWrapper( string programSourceHandle, CLKernel kernel )
        {
            string header = $"public void {kernel.Name}(";

            foreach ( KeyValuePair < string, KernelParameter > kernelParameter in kernel.Parameter )
            {
                header += $"{( kernelParameter.Value.IsArray ? "FLBuffer" : "float" )} {kernelParameter.Key},";
            }

            if ( kernel.Parameter.Count != 0 )
            {
                header = header.Remove( header.Length - 1, 1 );
            }

            header += ")";

            string body = $"//Initialize CL Program\nthis.{programSourceHandle}();\n// Execute Kernel: {kernel.Name}";

            return header + "\n{\n" + body + "\n}\n";
        }

        private string GetStaticTypes()
        {
            return "public class FLBuffer \n{ \n}\n";
        }

        #endregion

    }

}
