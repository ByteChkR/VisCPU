using System;
using System.Collections.Generic;
using System.Linq;

using OpenCL.NET;
using OpenCL.NET.Kernels;
using OpenCL.NET.Programs;
using OpenCL.Wrapper.TypeEnums;

using Utility.ExtPP.API;

namespace OpenCL.Wrapper
{
    /// <summary>
    ///     A wrapper class for a OpenCL Program.
    /// </summary>
    public class CLProgram
    {

        /// <summary>
        ///     The filepath of the program source
        /// </summary>
        public readonly string FilePath;

        public readonly string Source;

        private CLProgram(string filePath, Dictionary<string, CLKernel> kernels, string source)
        {
            FilePath = filePath;
            ContainedKernels = kernels;
            Source = source;
        }

        private CLProgram(Dictionary<string, CLKernel> kernels, string source)
        {
            FilePath = "";
            ContainedKernels = kernels;
            Source = source;
        }

        /// <summary>
        ///     The kernels that are contained in the Program
        /// </summary>
        public Dictionary<string, CLKernel> ContainedKernels { get; }

        /// <summary>
        ///     The Compiled OpenCL Program
        /// </summary>
        public Program ClProgramHandle { get; set; }

        public void Dispose()
        {
            ClProgramHandle?.Dispose();
            foreach (KeyValuePair<string, CLKernel> containedKernel in ContainedKernels)
            {
                containedKernel.Value.Dispose();
            }
        }

        /// <summary>
        ///     Returns the N of the VectorN types
        /// </summary>
        /// <param name="dtStr">the cl type in use</param>
        /// <returns>the amount of dimensions in the vector type</returns>
        public static int GetVectorNum(string dtStr)
        {
            if (!char.IsNumber(dtStr.Last()))
            {
                return 1;
            }

            if (dtStr.Last() == '2')
            {
                return 2;
            }

            if (dtStr.Last() == '3')
            {
                return 3;
            }

            if (dtStr.Last() == '4')
            {
                return 4;
            }

            if (dtStr.Last() == '8')
            {
                return 8;
            }

            if (dtStr.Last() == '6')
            {
                return 16;
            }

            return 0;
        }

        public static CLProgramBuildResult TryBuildProgram(
            CLAPI instance, string source, string filePath,
            out CLProgram program)
        {
            CLProgramBuildResult result = new CLProgramBuildResult(filePath, source, new List<CLProgramBuildError>());

            string[] kernelNames = FindKernelNames(source);
            if (kernelNames.Length == 0)
            {
                program = new CLProgram(filePath, new Dictionary<string, CLKernel>(), source);
                return result;
            }

            Program prgHandle;

            try
            {
                prgHandle = CLAPI.CreateClProgramFromSource(instance, source);
            }
            catch (Exception e)
            {
                result.BuildErrors.Add(new CLProgramBuildError(ErrorType.ProgramBuild, e));
                program = null;
                return result; //We can not progress
            }


            Dictionary<string, CLKernel> kernels = new Dictionary<string, CLKernel>();
            foreach (string kernelName in kernelNames)
            {
                try
                {
                    Kernel k = CLAPI.CreateKernelFromName(prgHandle, kernelName);
                    int kernelNameIndex = source.IndexOf(" " + kernelName + " (", StringComparison.InvariantCulture);
                    kernelNameIndex = kernelNameIndex == -1
                                          ? source.IndexOf(" " + kernelName + "(", StringComparison.InvariantCulture)
                                          : kernelNameIndex;
                    KernelParameter[] parameter = KernelParameter.CreateKernelParametersFromKernelCode(
                         source,
                         kernelNameIndex,
                         source.Substring(
                                          kernelNameIndex,
                                          source
                                              .Length -
                                          kernelNameIndex
                                         ).IndexOf(
                                                   ')'
                                                  ) +
                         1
                        );
                    if (k == null)
                    {
                        result.BuildErrors.Add(
                                               new CLProgramBuildError(
                                                                       ErrorType.KernelBuild,
                                                                       new OpenClException(
                                                                            $"Header parser completed on {kernelName} but the kernel could not be loaded."
                                                                           )
                                                                      )
                                              );
                        kernels.Add(kernelName, new CLKernel(instance, null, kernelName, parameter));
                    }
                    else
                    {
                        kernels.Add(kernelName, new CLKernel(instance, k, kernelName, parameter));
                    }
                }
                catch (Exception e)
                {
                    result.BuildErrors.Add(new CLProgramBuildError(ErrorType.KernelBuild, e));

                    //We can try other kernels
                }
            }

            program = new CLProgram(filePath, kernels, source);
            program.ClProgramHandle = prgHandle;
            return result;
        }


        public static CLProgramBuildResult TryBuildProgram(CLAPI instance, string filePath, out CLProgram program)
        {
            //string source = TextProcessorAPI.PreprocessSource(IOManager.ReadAllLines(filePath),
            //    Path.GetDirectoryName(filePath), Path.GetExtension(filePath), new Dictionary<string, bool>());
            string source = TextProcessorAPI.PreprocessSource(filePath, new Dictionary<string, bool>());

            //            program = null;

            return TryBuildProgram(instance, source, filePath, out program);
        }

        public static string[] FindFunctions(string source, DataVectorTypes returnType, string[] prefixes)
        {
            return FindFunctions(source, prefixes, KernelParameter.GetDataString(returnType));
        }


        private static string[] FindFunctions(string source, string[] prefixes, string returnType)
        {
            List<string> kernelNames = new List<string>();
            string[] s = source.Split(' ');
            List<string> parts = new List<string>();
            foreach (string part in s)
            {
                parts.AddRange(part.Split('\n'));
            }

            for (int i = 0; i < parts.Count; i++)
            {
                if (prefixes.Contains(parts[i]))
                {
                    if (i < parts.Count - 2 && parts[i + 1] == returnType)
                    {
                        if (parts[i + 2].Contains('('))
                        {
                            kernelNames.Add(
                                            parts[i + 2]. //The Kernel name
                                                Substring(
                                                          0,
                                                          parts[i + 2].IndexOf('(')
                                                         )
                                           );
                        }
                        else
                        {
                            kernelNames.Add(parts[i + 2]);
                        }
                    }
                }
            }

            return kernelNames.ToArray();
        }

        /// <summary>
        ///     Extracts the kernel names from the program source
        /// </summary>
        /// <param name="source">The complete source of the program</param>
        /// <returns>A list of kernel names</returns>
        private static string[] FindKernelNames(string source)
        {
            return FindFunctions(source, new[] { "__kernel", "kernel" }, "void");
        }

    }
}