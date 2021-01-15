using NUnit.Framework;

using VisCPU.Instructions;
using VisCPU.Tests.Utils;

namespace VisCPU.Tests
{

    [TestFixture]
    [SingleThreaded]
    public class AssignmentCompileTest : VisCPUCompileTest
    {

        [OneTimeSetUp]
        public void Setup()
        {
            Initialize();
            CPUSettings.InstructionSet = new DefaultSet();
            TestDevice.OnFail += ( name, reason ) => Assert.Fail( $"Test '{name}' failed with Reason '{reason}'" );
        }

        [Test]
        [Order( 1 )]
        [TestCaseSource( nameof( GetTestExpressionInstructions ), new object[] { "tests/assignments" } )]
        public void CompileHl( string file )
        {
            Vhl2Vasm( file );
        }

        [Test]
        [Order( 2 )]
        [TestCaseSource( nameof( GetTestAssemblyInstructions ), new object[] { "tests/assignments" } )]
        public void CompileVasm( string file )
        {
            Vasm2Vbin( file );
        }

        [Test]
        [Order( 3 )]
        [TestCaseSource( nameof( GetTestBinaryInstructions ), new object[] { "tests/assignments" } )]
        public void RunTests( string file )
        {
            VisCPURun.Run( file, TestDevice );
        }

    }

}
