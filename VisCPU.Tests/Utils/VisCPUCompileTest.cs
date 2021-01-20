using System.IO;
using System.Linq;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Implementations;
using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Tests.Utils
{

    public abstract class VisCpuCompileTest
    {
        protected readonly TestDevice TestDevice = new TestDevice();

        #region Protected

        protected static object[] GetTestAssemblyInstructions( string testFolder )
        {
            return Directory.GetFiles( testFolder, "*.vasm", SearchOption.TopDirectoryOnly ).
                             Cast < object >().
                             ToArray();
        }

        protected static object[] GetTestBinaryInstructions( string testFolder )
        {
            return Directory.GetFiles( testFolder, "*.vbin", SearchOption.TopDirectoryOnly ).
                             Cast < object >().
                             ToArray();
        }

        protected static object[] GetTestExpressionInstructions( string testFolder )
        {
            return Directory.GetFiles( testFolder, "*.vhl", SearchOption.TopDirectoryOnly ).Cast < object >().ToArray();
        }

        protected void Initialize()
        {
            SettingsManager.GetSettings < LinkerSettings >();
            SettingsManager.GetSettings < ConsoleInInterfaceSettings >();
            SettingsManager.GetSettings < ConsoleOutInterfaceSettings >();
            SettingsManager.GetSettings < HlCompilerSettings >();
            SettingsManager.GetSettings < MemorySettings >();
        }

        protected void Vasm2Vbin( string file )
        {
            Compilation comp = new Compilation( new MultiFileStaticLinker(), new DefaultAssemblyGenerator() );
            comp.Compile( file );

            string newFile = Path.Combine(
                                 Path.GetDirectoryName( Path.GetFullPath( file ) ),
                                 Path.GetFileNameWithoutExtension( file )
                             ) +
                             ".vbin";

            if ( SettingsManager.GetSettings < LinkerSettings >().ExportLinkerInfo )
            {
                comp.LinkerInfo.Save( newFile, LinkerInfo.LinkerInfoFormat.Text );
            }

            File.WriteAllBytes( newFile, comp.ByteCode.ToArray() );
        }

        protected void Vhl2Vasm( string file )
        {
            string src = File.ReadAllText( file );
            HlCompilation c = new HlCompilation( src, Path.GetDirectoryName( file ) );
            string outp = c.Parse();

            string outFile = Path.Combine( Path.GetDirectoryName( file ), Path.GetFileNameWithoutExtension( file ) ) +
                             ".vasm";

            File.WriteAllText( outFile, outp );
        }

        #endregion
    }

}
