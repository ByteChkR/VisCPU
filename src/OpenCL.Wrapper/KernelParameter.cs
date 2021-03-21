using System;
using System.Collections.Generic;

using OpenCL.NET;
using OpenCL.NET.DataTypes;
using OpenCL.Wrapper.TypeEnums;

namespace OpenCL.Wrapper
{
    /// <summary>
    ///     A class containing the logic to parse a kernel argument
    /// </summary>
    public class KernelParameter
    {

        /// <summary>
        ///     A List of Data type pairs.
        ///     Item 1: C 99 compilant keyword for the type
        ///     Item 2: The maximum value of the data type
        ///     Item 3: The Enum Representation of the Type
        /// </summary>
        private static readonly List<Tuple<string, float, DataVectorTypes>> DataTypePairs =
            new List<Tuple<string, float, DataVectorTypes>>
            {
                new Tuple<string, float, DataVectorTypes>("float", float.MaxValue, DataVectorTypes.Float1),
                new Tuple<string, float, DataVectorTypes>("float2", float.MaxValue, DataVectorTypes.Float2),
                new Tuple<string, float, DataVectorTypes>("float3", float.MaxValue, DataVectorTypes.Float3),
                new Tuple<string, float, DataVectorTypes>("float4", float.MaxValue, DataVectorTypes.Float4),
                new Tuple<string, float, DataVectorTypes>("float8", float.MaxValue, DataVectorTypes.Float8),
                new Tuple<string, float, DataVectorTypes>("float16", float.MaxValue, DataVectorTypes.Float16),
                new Tuple<string, float, DataVectorTypes>("int", int.MaxValue, DataVectorTypes.Int1),
                new Tuple<string, float, DataVectorTypes>("int2", int.MaxValue, DataVectorTypes.Int2),
                new Tuple<string, float, DataVectorTypes>("int3", int.MaxValue, DataVectorTypes.Int3),
                new Tuple<string, float, DataVectorTypes>("int4", int.MaxValue, DataVectorTypes.Int4),
                new Tuple<string, float, DataVectorTypes>("int8", int.MaxValue, DataVectorTypes.Int8),
                new Tuple<string, float, DataVectorTypes>("int16", int.MaxValue, DataVectorTypes.Int16),
                new Tuple<string, float, DataVectorTypes>("uint", uint.MaxValue, DataVectorTypes.Uint1),
                new Tuple<string, float, DataVectorTypes>("uint2", uint.MaxValue, DataVectorTypes.Uint2),
                new Tuple<string, float, DataVectorTypes>("uint3", uint.MaxValue, DataVectorTypes.Uint3),
                new Tuple<string, float, DataVectorTypes>("uint4", uint.MaxValue, DataVectorTypes.Uint4),
                new Tuple<string, float, DataVectorTypes>("uint8", uint.MaxValue, DataVectorTypes.Uint8),
                new Tuple<string, float, DataVectorTypes>("uint16", uint.MaxValue, DataVectorTypes.Uint16),
                new Tuple<string, float, DataVectorTypes>("char", sbyte.MaxValue, DataVectorTypes.Char1),
                new Tuple<string, float, DataVectorTypes>("char2", sbyte.MaxValue, DataVectorTypes.Char2),
                new Tuple<string, float, DataVectorTypes>("char3", sbyte.MaxValue, DataVectorTypes.Char3),
                new Tuple<string, float, DataVectorTypes>("char4", sbyte.MaxValue, DataVectorTypes.Char4),
                new Tuple<string, float, DataVectorTypes>("char8", sbyte.MaxValue, DataVectorTypes.Char8),
                new Tuple<string, float, DataVectorTypes>("char16", sbyte.MaxValue, DataVectorTypes.Char16),
                new Tuple<string, float, DataVectorTypes>("uchar", byte.MaxValue, DataVectorTypes.Uchar1),
                new Tuple<string, float, DataVectorTypes>("uchar2", byte.MaxValue, DataVectorTypes.Uchar2),
                new Tuple<string, float, DataVectorTypes>("uchar3", byte.MaxValue, DataVectorTypes.Uchar3),
                new Tuple<string, float, DataVectorTypes>("uchar4", byte.MaxValue, DataVectorTypes.Uchar4),
                new Tuple<string, float, DataVectorTypes>("uchar8", byte.MaxValue, DataVectorTypes.Uchar8),
                new Tuple<string, float, DataVectorTypes>("uchar16", byte.MaxValue, DataVectorTypes.Uchar16),
                new Tuple<string, float, DataVectorTypes>("short", short.MaxValue, DataVectorTypes.Short1),
                new Tuple<string, float, DataVectorTypes>("short2", short.MaxValue, DataVectorTypes.Short2),
                new Tuple<string, float, DataVectorTypes>("short3", short.MaxValue, DataVectorTypes.Short3),
                new Tuple<string, float, DataVectorTypes>("short4", short.MaxValue, DataVectorTypes.Short4),
                new Tuple<string, float, DataVectorTypes>("short8", short.MaxValue, DataVectorTypes.Short8),
                new Tuple<string, float, DataVectorTypes>("short16", short.MaxValue, DataVectorTypes.Short16),
                new Tuple<string, float, DataVectorTypes>("ushort", ushort.MaxValue, DataVectorTypes.Ushort1),
                new Tuple<string, float, DataVectorTypes>("ushort2", ushort.MaxValue, DataVectorTypes.Ushort2),
                new Tuple<string, float, DataVectorTypes>("ushort3", ushort.MaxValue, DataVectorTypes.Ushort3),
                new Tuple<string, float, DataVectorTypes>("ushort4", ushort.MaxValue, DataVectorTypes.Ushort4),
                new Tuple<string, float, DataVectorTypes>("ushort8", ushort.MaxValue, DataVectorTypes.Ushort8),
                new Tuple<string, float, DataVectorTypes>(
                                                          "ushort16",
                                                          ushort.MaxValue,
                                                          DataVectorTypes.Ushort16
                                                         ),
                new Tuple<string, float, DataVectorTypes>("long", long.MaxValue, DataVectorTypes.Long1),
                new Tuple<string, float, DataVectorTypes>("long2", long.MaxValue, DataVectorTypes.Long2),
                new Tuple<string, float, DataVectorTypes>("long3", long.MaxValue, DataVectorTypes.Long3),
                new Tuple<string, float, DataVectorTypes>("long4", long.MaxValue, DataVectorTypes.Long4),
                new Tuple<string, float, DataVectorTypes>("long8", long.MaxValue, DataVectorTypes.Long8),
                new Tuple<string, float, DataVectorTypes>("long16", long.MaxValue, DataVectorTypes.Long16),
                new Tuple<string, float, DataVectorTypes>("ulong", ulong.MaxValue, DataVectorTypes.Ulong1),
                new Tuple<string, float, DataVectorTypes>("ulong2", ulong.MaxValue, DataVectorTypes.Ulong2),
                new Tuple<string, float, DataVectorTypes>("ulong3", ulong.MaxValue, DataVectorTypes.Ulong3),
                new Tuple<string, float, DataVectorTypes>("ulong4", ulong.MaxValue, DataVectorTypes.Ulong4),
                new Tuple<string, float, DataVectorTypes>("ulong8", ulong.MaxValue, DataVectorTypes.Ulong8),
                new Tuple<string, float, DataVectorTypes>("ulong16", ulong.MaxValue, DataVectorTypes.Ulong16)
            };

        /// <summary>
        ///     The name of the argument
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The Data type
        /// </summary>
        public DataVectorTypes DataType { get; set; }

        /// <summary>
        ///     Is the Argument an Array?
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        ///     The Argument id of the Parameter
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     The scope of the argument
        /// </summary>
        public MemoryScope MemScope { get; set; }

        /// <summary>
        ///     A list of Types(in the same order as the DataType enum
        /// </summary>
        internal static Type[] Converters =>
            new[]
            {
                typeof(object),
                typeof(float),
                typeof(float2),
                typeof(float3),
                typeof(float4),
                typeof(float8),
                typeof(float16),
                typeof(int),
                typeof(int2),
                typeof(int3),
                typeof(int4),
                typeof(int8),
                typeof(int16),
                typeof(uint),
                typeof(uint2),
                typeof(uint3),
                typeof(uint4),
                typeof(uint8),
                typeof(uint16),
                typeof(sbyte),
                typeof(char2),
                typeof(char3),
                typeof(char4),
                typeof(char8),
                typeof(char16),
                typeof(byte),
                typeof(uchar2),
                typeof(uchar3),
                typeof(uchar4),
                typeof(uchar8),
                typeof(uchar16),
                typeof(short),
                typeof(short2),
                typeof(short3),
                typeof(short4),
                typeof(short8),
                typeof(short16),
                typeof(ushort),
                typeof(ushort2),
                typeof(ushort3),
                typeof(ushort4),
                typeof(ushort8),
                typeof(ushort16),
                typeof(long),
                typeof(long2),
                typeof(long3),
                typeof(long4),
                typeof(long8),
                typeof(long16),
                typeof(ulong),
                typeof(ulong2),
                typeof(ulong3),
                typeof(ulong4),
                typeof(ulong8),
                typeof(ulong16)
            };

        /// <summary>
        ///     Casts the supplied value to the specified type
        /// </summary>
        /// <param name="instance">CLAPI Instance for the current thread</param>
        /// <param name="value">the value casted to the required type for the parameter</param>
        /// <returns></returns>
        public object CastToType(CLAPI instance, object value)
        {
            if (IsArray)
            {
                throw new OpenClException("Can not Change types on an array.");
            }


            return CastToType(Converters[(int) DataType], value);
        }

        /// <summary>
        ///     Returns the Data type enum from the C# type
        /// </summary>
        /// <param name="t">The type</param>
        /// <returns>The Data type or UNKNOWN if not found</returns>
        public static DataVectorTypes GetEnumFromType(Type t)
        {
            for (int i = 0; i < Converters.Length; i++)
            {
                if (Converters[i] == t)
                {
                    return (DataVectorTypes) i;
                }
            }

            return DataVectorTypes.Unknown;
        }

        /// <summary>
        ///     Casts the supplied value to type t
        /// </summary>
        /// <param name="t">the target type</param>
        /// <param name="value">the value to be casted</param>
        /// <returns>The casted value</returns>
        public static object CastToType(Type t, object value)
        {
            if (value is decimal)
            {
                return Convert.ChangeType(value, t);
            }

            return CLTypeConverter.Convert(t, value);
        }


        /// <summary>
        ///     Parses the kernel parameters from the kernel signature
        /// </summary>
        /// <param name="code">The full program code</param>
        /// <param name="startIndex">The index where the kernel name starts</param>
        /// <param name="endIndex">the index after the bracket of the signature closed</param>
        /// <returns>A parsed list of Kernel parameters</returns>
        public static KernelParameter[] CreateKernelParametersFromKernelCode(string code, int startIndex, int endIndex)
        {
            string kernelHeader = code.Substring(startIndex, endIndex);
            int start = kernelHeader.IndexOf('('), end = kernelHeader.LastIndexOf(')');
            string parameters = kernelHeader.Substring(start + 1, end - start - 1);
            string[] pars = parameters.Split(',');
            KernelParameter[] ret = new KernelParameter[pars.Length];
            for (int i = 0; i < pars.Length; i++)
            {
                string[] parametr = pars[i].Trim().Split(' ');

                ret[i] = new KernelParameter
                         {
                             Name = parametr[parametr.Length - 1].Replace('*', ' ').Trim(),
                             DataType = GetDataType(parametr[parametr.Length - 2].Replace('*', ' ').Trim()),
                             MemScope = GetMemoryScope(parametr.Length == 3 ? parametr[0] : ""),
                             IsArray = parametr[parametr.Length - 2].IndexOf("*", StringComparison.InvariantCulture) !=
                                       -1 ||
                                       parametr[parametr.Length - 1].IndexOf("*", StringComparison.InvariantCulture) !=
                                       -1,
                             Id = i
                         };
            }

            return ret;
        }

        /// <summary>
        ///     returns the Correct DataType string for the equivalent in the CL Library
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The keyword for OpenCL as string</returns>
        public static string GetDataString(DataVectorTypes type)
        {
            foreach (Tuple<string, float, DataVectorTypes> dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item3 == type)
                {
                    return dataTypePair.Item1;
                }
            }

            return "UNKNOWN";
        }


        /// <summary>
        ///     returns the Correct DataType max value for the equivalent in the CL Library
        /// </summary>
        /// <param name="genType">the cl type that is used</param>
        /// <returns>max value of the data type</returns>
        public static float GetDataMaxSize(string genType)
        {
            foreach (Tuple<string, float, DataVectorTypes> dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item1 == genType)
                {
                    return dataTypePair.Item2;
                }
            }

            return 0;
        }

        /// <summary>
        ///     returns the Correct DataType enum for the equivalent in OpenCL C99
        /// </summary>
        /// <param name="str">String Representation of the CL Type</param>
        /// <returns>The data type</returns>
        public static DataVectorTypes GetDataType(string str)
        {
            foreach (Tuple<string, float, DataVectorTypes> dataTypePair in DataTypePairs)
            {
                if (dataTypePair.Item1 == str)
                {
                    return dataTypePair.Item3;
                }
            }

            return DataVectorTypes.Unknown;
        }

        /// <summary>
        ///     Returns the memory scope that is associated with the modifier
        /// </summary>
        /// <param name="modifier">The modifier to be tested</param>
        /// <returns>the MemoryScope</returns>
        private static MemoryScope GetMemoryScope(string modifier)
        {
            switch (modifier)
            {
                case "__global":
                    return MemoryScope.Global;
                case "global":
                    return MemoryScope.Global;
                default:
                    return MemoryScope.None;
            }
        }

    }
}