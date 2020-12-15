using System.IO;

using NUnit.Framework;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.HL;
using VisCPU.Instructions;
using VisCPU.Peripherals;
using VisCPU.Utility;

namespace VisCPU.Tests
{

    [TestFixture, SingleThreaded]
    public class InstructionCompileTest : VisCPUCompileTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CPUSettings.InstructionSet = new DefaultSet();
            TestDevice.OnFail += (name, reason) => Assert.Fail($"Test '{name}' failed with Reason '{reason}'");
        }


        [Test, Order(1), TestCaseSource(nameof(GetTestExpressionInstructions), new object[] { "tests/instructions" })]
        public void CompileHL(string file)
        {
            VHL2VASM(file);
        }

        [Test, Order(2), TestCaseSource(nameof(GetTestAssemblyInstructions), new object[] { "tests/instructions" })]
        public void CompileVASM(string file)
        {
            VASM2VBIN(file);
        }

        [Test, Order(3), TestCaseSource(nameof(GetTestBinaryInstructions), new object[] { "tests/instructions" })]
        public void RunTests(string file)
        {
            VisCPURun.Run(file, TestDevice);
        }

    }

}