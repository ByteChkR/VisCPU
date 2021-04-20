#region Using Directives

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using OpenCL.NET.Contexts;
using OpenCL.NET.Devices;
using OpenCL.NET.Events;
using OpenCL.NET.Interop;
using OpenCL.NET.Interop.CommandQueues;
using OpenCL.NET.Interop.EnqueuedCommands;
using OpenCL.NET.Kernels;
using OpenCL.NET.Memory;

#endregion

namespace OpenCL.NET.CommandQueues
{

    /// <summary>
    ///     Represents an OpenCL command queue.
    /// </summary>
    public class CommandQueue : HandleBase
    {

        #region Public

        /// <summary>
        ///     Initializes a new <see cref="CommandQueue" /> instance.
        /// </summary>
        /// <param name="handle">The handle to the OpenCL command queue.</param>
        internal CommandQueue( IntPtr handle )
            : base( handle, "CommandQueue", true )
        {
        }

        /// <summary>
        ///     Creates a new command queue for the specified context and device.
        /// </summary>
        /// <param name="context">The context for which the command queue is to be created.</param>
        /// <param name="device">The devices for which the command queue is to be created.</param>
        /// <exception cref="OpenClException">
        ///     If the command queue could not be created, then an <see cref="OpenClException" />
        ///     exception is thrown.
        /// </exception>
        /// <returns>Returns the created command queue.</returns>
        public static CommandQueue CreateCommandQueue( Context context, Device device )
        {
            // Creates the new command queue for the specified context and device
            IntPtr commandQueuePointer =
                CommandQueuesNativeApi.CreateCommandQueueWithProperties(
                                                                        context.Handle,
                                                                        device.Handle,
                                                                        IntPtr.Zero,
                                                                        out Result result
                                                                       );

            // Checks if the command queue creation was successful, if not, then an exception is thrown
            if ( result != Result.Success )
            {
                throw new OpenClException( "The command queue could not be created.", result );
            }

            // Creates the new command queue object from the pointer and returns it
            return new CommandQueue( commandQueuePointer );
        }

        /// <summary>
        ///     Disposes of the resources that have been acquired by the command queue.
        /// </summary>
        /// <param name="disposing">Determines whether managed object or managed and unmanaged resources should be disposed of.</param>
        public override void Dispose()
        {
            // Checks if the command queue has already been disposed of, if not, then the command queue is disposed of
            if ( !IsDisposed )
            {
                CommandQueuesNativeApi.ReleaseCommandQueue( Handle );
            }

            // Makes sure that the base class can execute its dispose logic
            base.Dispose();
        }

        /// <summary>
        ///     Enqueues a n-dimensional kernel to the command queue.
        /// </summary>
        /// <param name="kernel">The kernel that is to be enqueued.</param>
        /// <param name="workDimension">The dimensionality of the work.</param>
        /// <param name="workUnitsPerKernel">The number of work units per kernel.</param>
        /// <exception cref="OpenClException">
        ///     If the kernel could not be enqueued, then an <see cref="OpenClException" /> is
        ///     thrown.
        /// </exception>
        public void EnqueueNDRangeKernel( Kernel kernel, int workDimension, int workUnitsPerKernel )
        {
            // Enqueues the kernel
            Result result = EnqueuedCommandsNativeApi.EnqueueNDRangeKernel(
                                                                           Handle,
                                                                           kernel.Handle,
                                                                           ( uint ) workDimension,
                                                                           null,
                                                                           new[] { new IntPtr( workUnitsPerKernel ) },
                                                                           null,
                                                                           0,
                                                                           null,
                                                                           out IntPtr _
                                                                          );

            // Checks if the kernel was enqueued successfully, if not, then an exception is thrown
            if ( result != Result.Success )
            {
                throw new OpenClException( "The kernel could not be enqueued.", result );
            }
        }

        /// <summary>
        ///     Enqueues a n-dimensional kernel to the command queue, which is executed asynchronously.
        /// </summary>
        /// <param name="kernel">The kernel that is to be enqueued.</param>
        /// <param name="workDimension">The dimensionality of the work.</param>
        /// <param name="workUnitsPerKernel">The number of work units per kernel.</param>
        /// <exception cref="OpenClException">
        ///     If the kernel could not be enqueued, then an <see cref="OpenClException" /> is
        ///     thrown.
        /// </exception>
        public Task EnqueueNDRangeKernelAsync( Kernel kernel, int workDimension, int workUnitsPerKernel )
        {
            // Creates a new task completion source, which is used to signal when the command has completed
            TaskCompletionSource < bool > taskCompletionSource = new TaskCompletionSource < bool >();

            // Enqueues the kernel
            Result result = EnqueuedCommandsNativeApi.EnqueueNDRangeKernel(
                                                                           Handle,
                                                                           kernel.Handle,
                                                                           ( uint ) workDimension,
                                                                           null,
                                                                           new[] { new IntPtr( workUnitsPerKernel ) },
                                                                           null,
                                                                           0,
                                                                           null,
                                                                           out IntPtr waitEventPointer
                                                                          );

            // Checks if the kernel was enqueued successfully, if not, then an exception is thrown
            if ( result != Result.Success )
            {
                throw new OpenClException( "The kernel could not be enqueued.", result );
            }

            // Subscribes to the completed event of the wait event that was returned, when the command finishes, the task completion source is resolved
            AwaitableEvent awaitableEvent = new AwaitableEvent( waitEventPointer );

            awaitableEvent.OnCompleted += ( sender, e ) =>
                                          {
                                              try
                                              {
                                                  if ( awaitableEvent.CommandExecutionStatus ==
                                                       CommandExecutionStatus.Error )
                                                  {
                                                      taskCompletionSource.TrySetException(
                                                           new OpenClException(
                                                                               $"The command completed with the error code {awaitableEvent.CommandExecutionStatusCode}."
                                                                              )
                                                          );
                                                  }
                                                  else
                                                  {
                                                      taskCompletionSource.TrySetResult( true );
                                                  }
                                              }
                                              catch ( Exception exception )
                                              {
                                                  taskCompletionSource.TrySetException( exception );
                                              }
                                              finally
                                              {
                                                  awaitableEvent.Dispose();
                                              }
                                          };

            return taskCompletionSource.Task;
        }

        /// <summary>
        ///     Reads the specified memory object associated with this command queue.
        /// </summary>
        /// <param name="memoryObject">The memory object that is to be read.</param>
        /// <param name="outputSize">The number of array elements that are to be returned.</param>
        /// <typeparam name="T">The type of the array that is to be returned.</typeparam>
        /// <returns>Returns the value of the memory object.</param>
        public T[] EnqueueReadBuffer < T >( MemoryObject memoryObject, int outputSize ) where T : struct
        {
            // Tries to read the memory object
            IntPtr resultValuePointer = IntPtr.Zero;

            try
            {
                // Allocates enough memory for the result value
                int size = Marshal.SizeOf < T >() * outputSize;
                resultValuePointer = Marshal.AllocHGlobal( size );

                // Reads the memory object, by enqueuing the read operation to the command queue
                Result result = EnqueuedCommandsNativeApi.EnqueueReadBuffer(
                                                                            Handle,
                                                                            memoryObject.Handle,
                                                                            1,
                                                                            UIntPtr.Zero,
                                                                            new UIntPtr( ( uint ) size ),
                                                                            resultValuePointer,
                                                                            0,
                                                                            null,
                                                                            out IntPtr _
                                                                           );

                // Checks if the read operation was queued successfuly, if not, an exception is thrown
                if ( result != Result.Success )
                {
                    throw new OpenClException( "The memory object could not be read.", result );
                }

                // Goes through the result and converts the content of the result to an array
                T[] resultValue = new T[outputSize];

                for ( int i = 0; i < outputSize; i++ )
                {
                    resultValue[i] =
                        Marshal.PtrToStructure < T >( IntPtr.Add( resultValuePointer, i * Marshal.SizeOf < T >() ) );
                }

                // Returns the content of the memory object
                return resultValue;
            }
            finally
            {
                // Finally the allocated memory has to be freed
                if ( resultValuePointer != IntPtr.Zero )
                {
                    Marshal.FreeHGlobal( resultValuePointer );
                }
            }
        }

        /// <summary>
        ///     Reads the specified memory object associated with this command queue asynchronously.
        /// </summary>
        /// <param name="memoryObject">The memory object that is to be read.</param>
        /// <param name="outputSize">The number of array elements that are to be returned.</param>
        /// <typeparam name="T">The type of the array that is to be returned.</typeparam>
        /// <returns>Returns the value of the memory object.</param>
        public Task < T[] > EnqueueReadBufferAsync < T >( MemoryObject memoryObject, int outputSize ) where T : struct
        {
            // Creates a new task completion source, which is used to signal when the command has completed
            TaskCompletionSource < T[] > taskCompletionSource = new TaskCompletionSource < T[] >();

            // Allocates enough memory for the result value
            IntPtr resultValuePointer = IntPtr.Zero;
            int size = Marshal.SizeOf < T >() * outputSize;
            resultValuePointer = Marshal.AllocHGlobal( size );

            // Reads the memory object, by enqueuing the read operation to the command queue
            Result result = EnqueuedCommandsNativeApi.EnqueueReadBuffer(
                                                                        Handle,
                                                                        memoryObject.Handle,
                                                                        1,
                                                                        UIntPtr.Zero,
                                                                        new UIntPtr( ( uint ) size ),
                                                                        resultValuePointer,
                                                                        0,
                                                                        null,
                                                                        out IntPtr waitEventPointer
                                                                       );

            // Checks if the read operation was queued successfuly, if not, an exception is thrown
            if ( result != Result.Success )
            {
                throw new OpenClException( "The memory object could not be read.", result );
            }

            // Subscribes to the completed event of the wait event that was returned, when the command finishes, the task completion source is resolved
            AwaitableEvent awaitableEvent = new AwaitableEvent( waitEventPointer );

            awaitableEvent.OnCompleted += ( sender, e ) =>
                                          {
                                              try
                                              {
                                                  // Checks if the command was executed successfully, if not, then an exception is thrown
                                                  if ( awaitableEvent.CommandExecutionStatus ==
                                                       CommandExecutionStatus.Error )
                                                  {
                                                      taskCompletionSource.TrySetException(
                                                           new OpenClException(
                                                                               $"The command completed with the error code {awaitableEvent.CommandExecutionStatusCode}."
                                                                              )
                                                          );

                                                      return;
                                                  }

                                                  // Goes through the result and converts the content of the result to an array
                                                  T[] resultValue = new T[outputSize];

                                                  for ( int i = 0; i < outputSize; i++ )
                                                  {
                                                      resultValue[i] =
                                                          Marshal.PtrToStructure < T >(
                                                               IntPtr.Add(
                                                                          resultValuePointer,
                                                                          i * Marshal.SizeOf < T >()
                                                                         )
                                                              );
                                                  }

                                                  // Sets the result
                                                  taskCompletionSource.TrySetResult( resultValue );
                                              }
                                              catch ( Exception exception )
                                              {
                                                  taskCompletionSource.TrySetException( exception );
                                              }
                                              finally
                                              {
                                                  // Finally the allocated memory has to be freed and the allocated resources are disposed of
                                                  if ( resultValuePointer != IntPtr.Zero )
                                                  {
                                                      Marshal.FreeHGlobal( resultValuePointer );
                                                  }

                                                  awaitableEvent.Dispose();
                                              }
                                          };

            // Returns the task completion source, which resolves when the command has finished
            return taskCompletionSource.Task;
        }

        /// <summary>
        ///     Reads the specified memory object associated with this command queue.
        /// </summary>
        /// <param name="memoryObject">The memory object that is to be read.</param>
        /// <param name="outputSize">The number of array elements that are to be returned.</param>
        /// <typeparam name="T">The type of the array that is to be returned.</typeparam>
        /// <returns>Returns the value of the memory object.</param>
        public void EnqueueWriteBuffer < T >( MemoryObject memoryObject, T[] value ) where T : struct
        {
            // Tries to read the memory object
            GCHandle h = GCHandle.Alloc( value, GCHandleType.Pinned );
            IntPtr resultValuePointer = h.AddrOfPinnedObject();

            try
            {
                // Allocates enough memory for the result value
                int size = Marshal.SizeOf < T >() * value.Length;

                // Reads the memory object, by enqueuing the read operation to the command queue
                Result result = EnqueuedCommandsNativeApi.EnqueueWriteBuffer(
                                                                             Handle,
                                                                             memoryObject.Handle,
                                                                             1,
                                                                             UIntPtr.Zero,
                                                                             new UIntPtr( ( uint ) size ),
                                                                             resultValuePointer,
                                                                             0,
                                                                             null,
                                                                             out IntPtr _
                                                                            );

                // Checks if the read operation was queued successfuly, if not, an exception is thrown
                if ( result != Result.Success )
                {
                    throw new OpenClException( "The memory object could not be written to.", result );
                }
            }
            finally
            {
                // Finally the allocated memory has to be freed
                if ( resultValuePointer != IntPtr.Zero )
                {
                    h.Free();
                }
            }
        }

        public void Flush()
        {
            CommandQueuesNativeApi.Flush( Handle );
        }

        #endregion

    }

}
