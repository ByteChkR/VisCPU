using System.IO;
using System.Linq;

using NUnit.Framework.Internal;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Utility.Settings;

namespace VisCPU.Tests
{

    public abstract class VisCPUCompileTest
    {
        protected readonly TestDevice TestDevice = new TestDevice();

        protected static object[] GetTestExpressionInstructions(string testFolder) =>
            Directory.GetFiles(testFolder, "*.vhl", SearchOption.TopDirectoryOnly).Cast<object>().ToArray();
        protected static object[] GetTestAssemblyInstructions(string testFolder) =>
            Directory.GetFiles(testFolder, "*.vasm", SearchOption.TopDirectoryOnly).Cast<object>().ToArray();
        protected static object[] GetTestBinaryInstructions(string testFolder) =>
            Directory.GetFiles(testFolder, "*.vbin", SearchOption.TopDirectoryOnly).Cast<object>().ToArray();
        protected void VHL2VASM(string file)
        {
            string src = File.ReadAllText(file);
            HLCompilation c = new HLCompilation(src, Path.GetDirectoryName(file));
            string outp = c.Parse();
            string outFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".vasm";
            File.WriteAllText(outFile, outp);
        }

        protected void VASM2VBIN(string file)
        {
            Compilation comp = new Compilation(new MultiFileStaticLinker(), new DefaultAssemblyGenerator());
            comp.Compile(file);
            string newFile = Path.Combine(
                                          Path.GetDirectoryName(Path.GetFullPath(file)),
                                          Path.GetFileNameWithoutExtension(file)
                                         ) + ".vbin";

            if (Settings.GetSettings<LinkerSettings>().ExportLinkerInfo)
            {
                comp.LinkerInfo.Save(newFile, LinkerInfo.LinkerInfoFormat.Text);
            }

            File.WriteAllBytes(newFile, comp.ByteCode.ToArray());
        }
    }

}