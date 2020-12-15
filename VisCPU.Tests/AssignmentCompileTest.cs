using NUnit.Framework;

using VisCPU.Instructions;

namespace VisCPU.Tests
{
    [TestFixture, SingleThreaded]
    public class AssignmentPointerCompileTest : VisCPUCompileTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CPUSettings.InstructionSet = new DefaultSet();
            TestDevice.OnFail += (name, reason) => Assert.Fail($"Test '{name}' failed with Reason '{reason}'");
        }


        [Test, Order(1), TestCaseSource(nameof(GetTestExpressionInstructions), new object[] { "tests/assignments_pointer" })]
        public void CompileHL(string file)
        {
            VHL2VASM(file);
        }

        [Test, Order(2), TestCaseSource(nameof(GetTestAssemblyInstructions), new object[] { "tests/assignments_pointer" })]
        public void CompileVASM(string file)
        {
            VASM2VBIN(file); 
        }

        [Test, Order(3), TestCaseSource(nameof(GetTestBinaryInstructions), new object[] { "tests/assignments_pointer" })]
        public void RunTests(string file)
        {
            VisCPURun.Run(file, TestDevice);
        }

    }

    [TestFixture, SingleThreaded]
    public class AssignmentCompileTest : VisCPUCompileTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CPUSettings.InstructionSet = new DefaultSet();
            TestDevice.OnFail += (name, reason) => Assert.Fail($"Test '{name}' failed with Reason '{reason}'");
        }


        [Test, Order(1), TestCaseSource(nameof(GetTestExpressionInstructions), new object[] { "tests/assignments" })]
        public void CompileHL(string file)
        {
            VHL2VASM(file);
        }

        [Test, Order(2), TestCaseSource(nameof(GetTestAssemblyInstructions), new object[] { "tests/assignments" })]
        public void CompileVASM(string file)
        {
            VASM2VBIN(file);
        }

        [Test, Order(3), TestCaseSource(nameof(GetTestBinaryInstructions), new object[] { "tests/assignments" })]
        public void RunTests(string file)
        {
            VisCPURun.Run(file, TestDevice);
        }

    }

}