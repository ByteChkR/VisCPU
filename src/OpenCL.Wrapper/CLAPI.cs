using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

using OpenCL.NET;
using OpenCL.NET.CommandQueues;
using OpenCL.NET.Contexts;
using OpenCL.NET.DataTypes;
using OpenCL.NET.Devices;
using OpenCL.NET.Kernels;
using OpenCL.NET.Memory;
using OpenCL.NET.Platforms;
using OpenCL.NET.Programs;

using Utility.ADL;

namespace OpenCL.Wrapper
{
    /// <summary>
    ///     A wrapper class that is handling all the CL operations.
    /// </summary>
    public class CLAPI : ALoggable<LogType>, IDisposable
    {

        /// <summary>
        ///     A Delegate to create random numbers for every data type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns> a random value of type T</returns>
        public delegate T RandomFunc<out T>() where T : struct;

        private const string COPY_KERNEL_SOURCE = @"
__kernel void copy(__global uchar* destination, __global uchar* source)
{
	int idx = get_global_id(0);	
	destination[idx] = source[idx];
}";


        private static readonly Dictionary<CLAPI, CLKernel> CopyKernels = new Dictionary<CLAPI, CLKernel>();

        /// <summary>
        ///     Field that holds the instance of the CL wrapper
        /// </summary>
        private static CLAPI Instance;

        /// <summary>
        ///     The Command queue that the wrapper is using
        /// </summary>
        private CommandQueue commandQueue;

        /// <summary>
        ///     The CL Context that the wrapper is using
        /// </summary>
        private Context context;

        public bool IsDisposed;

        /// <summary>
        ///     Private constructor
        /// </summary>
        private CLAPI() : base(OpenCLDebugConfig.Settings, "API")
        {
            InitializeOpenCl();
        }

        /// <summary>
        ///     Helpful property for initializing the singleton
        /// </summary>
        public static CLAPI MainThread => Instance ?? (Instance = new CLAPI());

        public void Dispose()
        {
            IsDisposed = true;
            context.Dispose();
            context = null;
            commandQueue.Dispose();
            commandQueue = null;
        }

        public static void DisposeInstance()
        {
            Instance.Dispose();
            Instance = null;
        }

        /// <summary>
        ///     Returns the Command queue(dont use, its just for debugging if something is wrong)
        /// </summary>
        /// <returns>The Internal Command queue</returns>
        internal static CommandQueue GetQueue(CLAPI instance)
        {
            return instance.commandQueue;
        }

        public static CLAPI GetInstance()
        {
            return new CLAPI();
        }

        /// <summary>
        ///     Reinitializes the CL API
        ///     Used by The unit tests when requireing actual CL api calls(e.g. not in NO_CL mode)
        /// </summary>
        public static void Reinitialize()
        {
            Instance = new CLAPI();
        }


        /// <summary>
        ///     Initializes the OpenCL API
        /// </summary>
        private void InitializeOpenCl()
        {
            IEnumerable<Platform> platforms = Platform.GetPlatforms();
            List<Device> devs = new List<Device>();
            for (int i = 0; i < platforms.Count(); i++)
            {
                IEnumerable<Device> ds = platforms.ElementAt(i).GetDevices(DeviceType.Default);
                for (int j = 0; j < ds.Count(); j++)
                {
                    Logger.Log(LogType.Log, "Adding Device: " + ds.ElementAt(j).Name + "@" + ds.ElementAt(j).Vendor, 1);
                    devs.Add(ds.ElementAt(j));
                }
            }

            Device chosenDevice = null;
            bool found = false;
            for (int i = 0; i < devs.Count; i++)
            {
                bool available = devs[i].IsAvailable;
                if (available && !found)
                {
                    Logger.Log(LogType.Log, "Choosing Device: " + devs[i].Name + "@" + devs[i].Vendor, 1);
                    chosenDevice = devs[i];
                    found = true;
                }
            }

            if (chosenDevice == null)
            {
                throw new OpenClException("Could not Get Device. Total Devices: " + devs.Count);
            }

            try
            {
                context = Context.CreateContext(chosenDevice);
                commandQueue = CommandQueue.CreateCommandQueue(context, chosenDevice);
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, e.ToString(), 1);
                throw new OpenClException(
                                          "Could not initialize OpenCL with Device: " +
                                          chosenDevice.Name +
                                          "@" +
                                          chosenDevice.Vendor +
                                          "\n\t" +
                                          e.Message,
                                          e
                                         );
            }
        }


        /// <summary>
        ///     Creates a CL Kernel from name
        /// </summary>
        /// <param name="program">The program that contains the kernel</param>
        /// <param name="name">The name of the kernel</param>
        /// <returns>The Compiled and Linked Kernel</returns>
        internal static Kernel CreateKernelFromName(Program program, string name)
        {
            if (program == null)
            {
                return null;
            }

            return program.CreateKernel(name);
        }

        /// <summary>
        ///     Creates an array with random values
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="size">the size of the array</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        /// <returns>An array filled with random values of type T</returns>
        public static T[] CreateRandom<T>(int size, byte[] channelEnableState, RandomFunc<T> rnd, bool uniform)
            where T : struct
        {
            T[] buffer = new T[size];
            WriteRandom(buffer, channelEnableState, rnd, uniform);
            return buffer;
        }

        /// <summary>
        ///     Creates an array with random values
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="size">the size of the array</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <returns>An array filled with random values of type T</returns>
        public static T[] CreateRandom<T>(int size, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            return CreateRandom(size, channelEnableState, rnd, true);
        }


        public static MemoryBuffer Copy<T>(CLAPI instance, MemoryBuffer input) where T : struct
        {
            CLKernel k = null;
            if (CopyKernels.ContainsKey(instance))
            {
                k = CopyKernels[instance];
            }
            else
            {
                string clt = KernelParameter.GetDataString(KernelParameter.GetEnumFromType(typeof(T)));
                string src = COPY_KERNEL_SOURCE.Replace("__TYPE__", clt);
                CLProgram.TryBuildProgram(instance, src, "internal_copy_kernel.cl", out CLProgram prog);
                k = prog.ContainedKernels["copy"];
                CopyKernels[instance] = k;
            }

            MemoryBuffer mb = CreateEmpty<T>(
                                             instance,
                                             (int) input.Size,
                                             input.Flags,
                                             "CopyOf:" + input,
                                             true
                                            );
            k.SetBuffer(0, mb);
            k.SetBuffer(1, input);
            Run(instance, k, (int) mb.Size);
            return mb;
        }

        /// <summary>
        ///     Writes random values to an array
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buffer">Array containing the values to overwrite</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        public static void WriteRandom<T>(
            T[] buffer, byte[] channelEnableState, RandomFunc<T> rnd,
            bool uniform) where T : struct
        {
            T val = rnd.Invoke();
            for (int i = 0; i < buffer.Length; i++)
            {
                int channel = i % channelEnableState.Length;
                if (channel == 0 || !uniform)
                {
                    val = rnd.Invoke();
                }

                if (channelEnableState[channel] == 1)
                {
                    buffer[i] = val;
                }
            }
        }

        /// <summary>
        ///     Writes random values to an array
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="buffer">Array containing the values to overwrite</param>
        /// <param name="channelEnableState">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        public static void WriteRandom<T>(T[] buffer, byte[] channelEnableState, RandomFunc<T> rnd) where T : struct
        {
            WriteRandom(buffer, channelEnableState, rnd, true);
        }

        /// <summary>
        ///     Writes random values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="enabledChannels">the channels that are enables(aka. get written with bytes)</param>
        /// <param name="uniform">Should every channel receive the same value on the same pixel?</param>
        public static void WriteRandom<T>(
            CLAPI instance, MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels,
            bool uniform)
            where T : struct
        {
            MemoryBuffer buffer = buf;

            T[] data = instance.commandQueue.EnqueueReadBuffer<T>(buffer, (int) buffer.Size);


            WriteRandom(data, enabledChannels, rnd, uniform);

            instance.commandQueue.EnqueueWriteBuffer(buffer, data);
        }

        /// <summary>
        ///     Writes random values to a Memory Buffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="rnd">the RandomFunc delegate providing the random numbers.</param>
        /// <param name="enabledChannels">the channels that are enables(aka. get written with bytes)</param>
        public static void WriteRandom<T>(CLAPI instance, MemoryBuffer buf, RandomFunc<T> rnd, byte[] enabledChannels)
            where T : struct
        {
            WriteRandom(instance, buf, rnd, enabledChannels, true);
        }


        /// <summary>
        ///     Writes values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="values">The values to be written to the buffer</param>
        public static void WriteToBuffer<T>(CLAPI instance, MemoryBuffer buf, T[] values) where T : struct
        {
            instance.commandQueue.EnqueueWriteBuffer(buf, values);
        }

        /// <summary>
        ///     Writes values to a MemoryBuffer
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="buf">MemoryBuffer containing the values to overwrite</param>
        /// <param name="size">The count of structs to be read from the buffer</param>
        /// <returns>The content of the buffer</returns>
        public static T[] ReadBuffer<T>(CLAPI instance, MemoryBuffer buf, int size) where T : struct
        {
            return instance.commandQueue.EnqueueReadBuffer<T>(buf, size);
        }

        /// <summary>
        ///     Runs a kernel with a valid FL kernel signature
        /// </summary>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="kernel">The CLKernel to be executed</param>
        /// <param name="image">The image buffer that serves as input</param>
        /// <param name="dimensions">The dimensions of the input buffer</param>
        /// <param name="genTypeMaxVal">The max valuee of the generic type that is used.(byte = 255)</param>
        /// <param name="enabledChannels">The enabled channels for the kernel</param>
        /// <param name="channelCount">The amount of active channels.</param>
        public static void Run(
            CLAPI instance, CLKernel kernel, MemoryBuffer image, int3 dimensions,
            float genTypeMaxVal,
            MemoryBuffer enabledChannels,
            int channelCount)
        {
            kernel.Run(instance.commandQueue, image, dimensions, genTypeMaxVal, enabledChannels, channelCount);
        }

        public static void Run(CLAPI instance, CLKernel kernel, int groupSize)
        {
            kernel.Run(instance.commandQueue, 1, groupSize);
        }

        public static void Flush(CLAPI instance)
        {
            instance.commandQueue.Flush();
        }

        #region Instance Functions

        internal static Program CreateClProgramFromSource(CLAPI instance, string source)
        {
            try
            {
                return instance.context.CreateAndBuildProgramFromString(source);
            }
            catch (Exception e)
            {
                throw new OpenClException("Could not compile file", e);
            }
        }

        internal static Program CreateClProgramFromSource(CLAPI instance, string[] source)
        {
            return instance.context.CreateAndBuildProgramFromString(source);
        }

        /// <summary>
        ///     Creates an empty buffer of type T with the specified size and MemoryFlags
        /// </summary>
        /// <typeparam name="T">The type of the struct</typeparam>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="size">The size of the buffer(Total size in bytes: size*sizeof(T)</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateEmpty<T>(
            CLAPI instance, int size, MemoryFlag flags, object handleIdentifier, bool optimize = false)
            where T : struct
        {
            if (!optimize)
            {
                return CreateBuffer(instance, new byte[size], flags, handleIdentifier);
            }

            return CreateEmptyOptimized<T>(instance, size, flags, handleIdentifier);
        }

        /// <summary>
        ///     Creates a Buffer with the specified content and Memory Flags
        /// </summary>
        /// <typeparam name="T">Type of the struct</typeparam>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="data">The array of T</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateBuffer<T>(CLAPI instance, T[] data, MemoryFlag flags, object handleIdentifier)
            where T : struct
        {
            object[] arr = Array.ConvertAll(data, x => (object) x);
            return CreateBuffer(instance, arr, typeof(T), flags, handleIdentifier);
        }

        //Optimization
        private static MemoryBuffer CreateEmptyOptimized<T>(
            CLAPI instance, int size, MemoryFlag flags,
            object handleIdentifier) where T : struct
        {
            //return CreateBuffer(instance, new byte[size], flags, handleIdentifier);
            int bufByteSize = Marshal.SizeOf<T>() * size;
            return instance.context.CreateBuffer(flags | MemoryFlag.AllocateHostPointer, bufByteSize, handleIdentifier);
        }

        public static MemoryBuffer CreateBuffer(
            CLAPI instance, object[] data, Type t, MemoryFlag flags,
            object handleIdentifier)
        {
            MemoryBuffer mb =
                instance.context.CreateBuffer(
                                              flags | MemoryFlag.CopyHostPointer,
                                              t,
                                              data,
                                              handleIdentifier
                                             );

            return mb;
        }

        /// <summary>
        ///     Creates a buffer with the content of an image and the specified Memory Flags
        /// </summary>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="bmp">The image that holds the data</param>
        /// <param name="flags">The memory flags for the buffer creation</param>
        /// <returns></returns>
        public static MemoryBuffer CreateFromImage(
            CLAPI instance, Bitmap bmp, MemoryFlag flags,
            object handleIdentifier)
        {
            BitmapData data = bmp.LockBits(
                                           new Rectangle(0, 0, bmp.Width, bmp.Height),
                                           ImageLockMode.ReadOnly,
                                           PixelFormat.Format32bppArgb
                                          );
            byte[] buffer = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(data);

            ARGBtoBGRA(buffer);

            MemoryBuffer mb = CreateBuffer(instance, buffer, flags, handleIdentifier);
            return mb;
        }

        private static void ARGBtoBGRA(Span<byte> bytes)
        {
            byte[] ret = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length / 4; i++)
            {
                byte a = bytes[i];
                byte r = bytes[i + 1];
                byte g = bytes[i + 2];
                byte b = bytes[i + 3];
                ret[i] = b;
                ret[i + 1] = g;
                ret[i + 2] = r;
                ret[i + 3] = a;
            }
        }

        private static void BGRAtoARGB(Span<byte> bytes)
        {
            byte[] ret = new byte[bytes.Length];
            for (int i = 0; i < bytes.Length / 4; i++)
            {
                byte b = bytes[i];
                byte g = bytes[i + 1];
                byte r = bytes[i + 2];
                byte a = bytes[i + 3];
                ret[i] = a;
                ret[i + 1] = r;
                ret[i + 2] = g;
                ret[i + 3] = b;
            }
        }

        public static void UpdateBitmap(CLAPI instance, Bitmap target, MemoryBuffer buffer)
        {
            byte[] bs = ReadBuffer<byte>(instance, buffer, (int) buffer.Size);
            UpdateBitmap(instance, target, bs);
        }

        public static byte[] GetBitmapData(Bitmap bmp)
        {
            BitmapData data = bmp.LockBits(
                                           new Rectangle(0, 0, bmp.Width, bmp.Height),
                                           ImageLockMode.ReadOnly,
                                           PixelFormat.Format32bppArgb
                                          );
            byte[] buffer = new byte[bmp.Width * bmp.Height * 4];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            bmp.UnlockBits(data);
            return buffer;
        }

        public static void UpdateBitmap(CLAPI instance, Bitmap target, Span<byte> bytes)
        {
            BGRAtoARGB(bytes);
            BitmapData data = target.LockBits(
                                              new Rectangle(0, 0, target.Width, target.Height),
                                              ImageLockMode.WriteOnly,
                                              PixelFormat.Format32bppArgb
                                             );

            Marshal.Copy(bytes.ToArray(), 0, data.Scan0, bytes.Length);
            target.UnlockBits(data);
        }

        public static void UpdateBitmap(CLAPI instance, Bitmap target, byte[] bytes)
        {
            BGRAtoARGB(bytes);
            BitmapData data = target.LockBits(
                                              new Rectangle(0, 0, target.Width, target.Height),
                                              ImageLockMode.WriteOnly,
                                              PixelFormat.Format32bppArgb
                                             );
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            target.UnlockBits(data);
        }

        #endregion

    }
}