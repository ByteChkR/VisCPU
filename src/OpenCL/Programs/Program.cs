#region Using Directives

using System;
using OpenCL.NET.Interop;
using OpenCL.NET.Interop.Kernels;
using OpenCL.NET.Interop.Programs;
using OpenCL.NET.Kernels;

#endregion

namespace OpenCL.NET.Programs
{

    /// <summary>
    ///     Represents an OpenCL program.
    /// </summary>
    public class Program : HandleBase
    {
        #region Public

        /// <summary>
        ///     Initializes a new <see cref="Program" /> instance.
        /// </summary>
        /// <param name="handle">The handle to the OpenCL program.</param>
        internal Program( IntPtr handle )
            : base( handle, "Program", true )
        {
        }

        /// <summary>
        ///     Creates a kernel with the specified name from the program.
        /// </summary>
        /// <param name="kernelName">The name of the kernel that is defined in the program.</param>
        /// <exception cref="OpenClException">If the kernel could not be created, then an <see cref="OpenClException" /> is thrown.</exception>
        /// <returns>Returns the created kernel.</returns>
        public Kernel CreateKernel( string kernelName )
        {
            // Allocates enough memory for the return value and retrieves it
            Result result;
            IntPtr kernelPointer = KernelsNativeApi.CreateKernel( Handle, kernelName, out result );

            if ( result != Result.Success )
            {
                throw new OpenClException( "The kernel could not be created.", result );
            }

            // Creates a new kernel object from the kernel pointer and returns it
            return new Kernel( kernelPointer );
        }

        /// <summary>
        ///     Disposes of the resources that have been acquired by the program.
        /// </summary>
        /// <param name="disposing">Determines whether managed object or managed and unmanaged resources should be disposed of.</param>
        public override void Dispose()
        {
            // Checks if the program has already been disposed of, if not, then the program is disposed of
            if ( !IsDisposed )
            {
                ProgramsNativeApi.ReleaseProgram( Handle );
            }

            // Makes sure that the base class can execute its dispose logic
            base.Dispose();
        }

        #endregion
    }

}