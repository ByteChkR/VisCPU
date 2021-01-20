﻿using NUnit.Framework;
using VisCPU.Tests.Utils;

namespace VisCPU.Tests
{

    [TestFixture]
    [SingleThreaded]
    public class BranchCompileTest : VisCpuCompileTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Initialize();
            TestDevice.OnFail += ( name, reason ) => Assert.Fail( $"Test '{name}' failed with Reason '{reason}'" );
        }

        [Test]
        [Order( 1 )]
        [TestCaseSource( nameof( GetTestExpressionInstructions ), new object[] { "tests/branches" } )]
        public void CompileHl( string file )
        {
            Vhl2Vasm( file );
        }

        [Test]
        [Order( 2 )]
        [TestCaseSource( nameof( GetTestAssemblyInstructions ), new object[] { "tests/branches" } )]
        public void CompileVasm( string file )
        {
            Vasm2Vbin( file );
        }

        [Test]
        [Order( 3 )]
        [TestCaseSource( nameof( GetTestBinaryInstructions ), new object[] { "tests/branches" } )]
        public void RunTests( string file )
        {
            VisCpuRun.Run( file, TestDevice );
        }
    }

}
