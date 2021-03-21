using System;
using System.Collections.Generic;

using OpenCL.NET.DataTypes;
using OpenCL.Wrapper.TypeEnums;

namespace OpenCL.Wrapper
{
    /// <summary>
    ///     Used to magically convert VectorN into VectorN of different type.
    /// </summary>
    public static class CLTypeConverter
    {

        /// <summary>
        ///     Dictionary containing the ToConverter
        ///     From Array of objects(need to be implicitly converted into to the specifed base type)
        /// </summary>
        private static readonly Dictionary<DataVectorTypes, ConvertToN> ToConverter =
            new Dictionary<DataVectorTypes, ConvertToN>
            {
                { DataVectorTypes.Uchar2, CreateByte2 },
                { DataVectorTypes.Uchar3, CreateByte3 },
                { DataVectorTypes.Uchar4, CreateByte4 },
                { DataVectorTypes.Uchar8, CreateByte8 },
                { DataVectorTypes.Uchar16, CreateByte16 },
                { DataVectorTypes.Char2, CreateSByte2 },
                { DataVectorTypes.Char3, CreateSByte3 },
                { DataVectorTypes.Char4, CreateSByte4 },
                { DataVectorTypes.Char8, CreateSByte8 },
                { DataVectorTypes.Char16, CreateSByte16 },
                { DataVectorTypes.Ulong2, CreateULong2 },
                { DataVectorTypes.Ulong3, CreateULong3 },
                { DataVectorTypes.Ulong4, CreateULong4 },
                { DataVectorTypes.Ulong8, CreateULong8 },
                { DataVectorTypes.Ulong16, CreateULong16 },
                { DataVectorTypes.Long2, CreateLong2 },
                { DataVectorTypes.Long3, CreateLong3 },
                { DataVectorTypes.Long4, CreateLong4 },
                { DataVectorTypes.Long8, CreateLong8 },
                { DataVectorTypes.Long16, CreateLong16 },
                { DataVectorTypes.Uint2, CreateUInt2 },
                { DataVectorTypes.Uint3, CreateUInt3 },
                { DataVectorTypes.Uint4, CreateUInt4 },
                { DataVectorTypes.Uint8, CreateUInt8 },
                { DataVectorTypes.Uint16, CreateUInt16 },
                { DataVectorTypes.Int2, CreateInt2 },
                { DataVectorTypes.Int3, CreateInt3 },
                { DataVectorTypes.Int4, CreateInt4 },
                { DataVectorTypes.Int8, CreateInt8 },
                { DataVectorTypes.Int16, CreateInt16 },
                { DataVectorTypes.Ushort2, CreateUShort2 },
                { DataVectorTypes.Ushort3, CreateUShort3 },
                { DataVectorTypes.Ushort4, CreateUShort4 },
                { DataVectorTypes.Ushort8, CreateUShort8 },
                { DataVectorTypes.Ushort16, CreateUShort16 },
                { DataVectorTypes.Short2, CreateShort2 },
                { DataVectorTypes.Short3, CreateShort3 },
                { DataVectorTypes.Short4, CreateShort4 },
                { DataVectorTypes.Short8, CreateShort8 },
                { DataVectorTypes.Short16, CreateShort16 },
                { DataVectorTypes.Float2, CreateFloat2 },
                { DataVectorTypes.Float3, CreateFloat3 },
                { DataVectorTypes.Float4, CreateFloat4 },
                { DataVectorTypes.Float8, CreateFloat8 },
                { DataVectorTypes.Float16, CreateFloat16 }
            };

        /// <summary>
        ///     A dictionary containing the Base types for the different CL typee
        /// </summary>
        private static readonly Dictionary<DataVectorTypes, Type> BaseTypes =
            new Dictionary<DataVectorTypes, Type>
            {
                { DataVectorTypes.Uchar2, typeof(byte) },
                { DataVectorTypes.Uchar3, typeof(byte) },
                { DataVectorTypes.Uchar4, typeof(byte) },
                { DataVectorTypes.Uchar8, typeof(byte) },
                { DataVectorTypes.Uchar16, typeof(byte) },
                { DataVectorTypes.Char2, typeof(sbyte) },
                { DataVectorTypes.Char3, typeof(sbyte) },
                { DataVectorTypes.Char4, typeof(sbyte) },
                { DataVectorTypes.Char8, typeof(sbyte) },
                { DataVectorTypes.Char16, typeof(sbyte) },
                { DataVectorTypes.Ulong2, typeof(ulong) },
                { DataVectorTypes.Ulong3, typeof(ulong) },
                { DataVectorTypes.Ulong4, typeof(ulong) },
                { DataVectorTypes.Ulong8, typeof(ulong) },
                { DataVectorTypes.Ulong16, typeof(ulong) },
                { DataVectorTypes.Long2, typeof(long) },
                { DataVectorTypes.Long3, typeof(long) },
                { DataVectorTypes.Long4, typeof(long) },
                { DataVectorTypes.Long8, typeof(long) },
                { DataVectorTypes.Long16, typeof(long) },
                { DataVectorTypes.Uint2, typeof(uint) },
                { DataVectorTypes.Uint3, typeof(uint) },
                { DataVectorTypes.Uint4, typeof(uint) },
                { DataVectorTypes.Uint8, typeof(uint) },
                { DataVectorTypes.Uint16, typeof(uint) },
                { DataVectorTypes.Int2, typeof(int) },
                { DataVectorTypes.Int3, typeof(int) },
                { DataVectorTypes.Int4, typeof(int) },
                { DataVectorTypes.Int8, typeof(int) },
                { DataVectorTypes.Int16, typeof(int) },
                { DataVectorTypes.Ushort2, typeof(ushort) },
                { DataVectorTypes.Ushort3, typeof(ushort) },
                { DataVectorTypes.Ushort4, typeof(short) },
                { DataVectorTypes.Ushort8, typeof(ushort) },
                { DataVectorTypes.Ushort16, typeof(ushort) },
                { DataVectorTypes.Short2, typeof(short) },
                { DataVectorTypes.Short3, typeof(short) },
                { DataVectorTypes.Short4, typeof(short) },
                { DataVectorTypes.Short8, typeof(short) },
                { DataVectorTypes.Short16, typeof(short) },
                { DataVectorTypes.Float2, typeof(float) },
                { DataVectorTypes.Float3, typeof(float) },
                { DataVectorTypes.Float4, typeof(float) },
                { DataVectorTypes.Float8, typeof(float) },
                { DataVectorTypes.Float16, typeof(float) }
            };

        /// <summary>
        ///     Dictionary containing the FromConverter
        ///     From the CL Type to an Array of objects
        /// </summary>
        private static readonly Dictionary<DataVectorTypes, ConvertFromN> FromConverter =
            new Dictionary<DataVectorTypes, ConvertFromN>
            {
                { DataVectorTypes.Uchar2, FromByte2 },
                { DataVectorTypes.Uchar3, FromByte3 },
                { DataVectorTypes.Uchar4, FromByte4 },
                { DataVectorTypes.Uchar8, FromByte8 },
                { DataVectorTypes.Uchar16, FromByte16 },
                { DataVectorTypes.Char2, FromSByte2 },
                { DataVectorTypes.Char3, FromSByte3 },
                { DataVectorTypes.Char4, FromSByte4 },
                { DataVectorTypes.Char8, FromSByte8 },
                { DataVectorTypes.Char16, FromSByte16 },
                { DataVectorTypes.Ulong2, FromULong2 },
                { DataVectorTypes.Ulong3, FromULong3 },
                { DataVectorTypes.Ulong4, FromULong4 },
                { DataVectorTypes.Ulong8, FromULong8 },
                { DataVectorTypes.Ulong16, FromULong16 },
                { DataVectorTypes.Long2, FromLong2 },
                { DataVectorTypes.Long3, FromLong3 },
                { DataVectorTypes.Long4, FromLong4 },
                { DataVectorTypes.Long8, FromLong8 },
                { DataVectorTypes.Long16, FromLong16 },
                { DataVectorTypes.Uint2, FromUInt2 },
                { DataVectorTypes.Uint3, FromUInt3 },
                { DataVectorTypes.Uint4, FromUInt4 },
                { DataVectorTypes.Uint8, FromUInt8 },
                { DataVectorTypes.Uint16, FromUInt16 },
                { DataVectorTypes.Int2, FromInt2 },
                { DataVectorTypes.Int3, FromInt3 },
                { DataVectorTypes.Int4, FromInt4 },
                { DataVectorTypes.Int8, FromInt8 },
                { DataVectorTypes.Int16, FromInt16 },
                { DataVectorTypes.Ushort2, FromUShort2 },
                { DataVectorTypes.Ushort3, FromUShort3 },
                { DataVectorTypes.Ushort4, FromUShort4 },
                { DataVectorTypes.Ushort8, FromUShort8 },
                { DataVectorTypes.Ushort16, FromUShort16 },
                { DataVectorTypes.Short2, Fromshort2 },
                { DataVectorTypes.Short3, Fromshort3 },
                { DataVectorTypes.Short4, Fromshort4 },
                { DataVectorTypes.Short8, Fromshort8 },
                { DataVectorTypes.Short16, Fromshort16 },
                { DataVectorTypes.Float2, FromFloat2 },
                { DataVectorTypes.Float3, FromFloat3 },
                { DataVectorTypes.Float4, FromFloat4 },
                { DataVectorTypes.Float8, FromFloat8 },
                { DataVectorTypes.Float16, FromFloat16 }
            };

        /// <summary>
        ///     Converts the Range of a number to a different one
        /// </summary>
        /// <param name="value">The Value to be changed</param>
        /// <param name="oldMax">The old maximum range</param>
        /// <param name="max">the new maximum range</param>
        /// <returns></returns>
        private static float ConvertRange(float value, float oldMax, float max)
        {
            float d = value / oldMax;
            return d * max;
        }

        /// <summary>
        ///     Converts the Specified value(that also needs to be a CL struct e.g. uchar4 to float4 is valid) into another CL
        ///     struct
        /// </summary>
        /// <param name="newType">The new type of the Value</param>
        /// <param name="value">The value</param>
        /// <returns>The value as the new Type</returns>
        public static object Convert(Type newType, object value)
        {
            DataVectorTypes olddt = KernelParameter.GetEnumFromType(value.GetType());
            DataVectorTypes dt = KernelParameter.GetEnumFromType(newType);

            string oldName = KernelParameter.GetDataString(olddt);
            string newName = KernelParameter.GetDataString(dt);

            float oldMax = KernelParameter.GetDataMaxSize(oldName);
            float newMax = KernelParameter.GetDataMaxSize(newName);
            int w = CLProgram.GetVectorNum(oldName);
            if (w == 1)
            {
                return System.Convert.ChangeType(
                                                 ConvertRange(
                                                              (float) System.Convert.ChangeType(value, typeof(float)),
                                                              oldMax,
                                                              newMax
                                                             ),
                                                 newType
                                                );
            }

            object[] objs = FromConverter[olddt](value);


            for (int i = 0; i < objs.Length; i++)
            {
                objs[i] = System.Convert.ChangeType(
                                                    ConvertRange(
                                                                 (float) System.Convert.ChangeType(
                                                                      objs[i],
                                                                      typeof(float)
                                                                     ),
                                                                 oldMax,
                                                                 newMax
                                                                ),
                                                    BaseTypes[dt]
                                                   );
            }

            return ToConverter[dt](objs);
        }

        /// <summary>
        ///     Delegate used to Create the ToConverter
        /// </summary>
        /// <param name="args">The Numers</param>
        /// <returns>a CL type</returns>
        private delegate object ConvertToN(object[] args);

        /// <summary>
        ///     Delegate used to Create the FromConverter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>An array of numbers resembling the components of the CL Type</returns>
        private delegate object[] ConvertFromN(object value);


        #region Float

        /// <summary>
        ///     FromFloat 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromFloat16(object value)
        {
            float16 val = (float16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromFloat 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromFloat8(object value)
        {
            float8 val = (float8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromFloat 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromFloat4(object value)
        {
            float4 val = (float4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromFloat 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromFloat3(object value)
        {
            float3 val = (float3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromFloat 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromFloat2(object value)
        {
            float2 val = (float2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }


        /// <summary>
        ///     ToFloat 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateFloat16(params object[] args)
        {
            return new float16(
                               (float) args[0],
                               (float) args[1],
                               (float) args[2],
                               (float) args[3],
                               (float) args[4],
                               (float) args[5],
                               (float) args[6],
                               (float) args[7],
                               (float) args[8],
                               (float) args[9],
                               (float) args[10],
                               (float) args[11],
                               (float) args[12],
                               (float) args[13],
                               (float) args[14],
                               (float) args[15]
                              );
        }


        /// <summary>
        ///     ToFloat 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateFloat8(params object[] args)
        {
            return new float8(
                              (float) args[0],
                              (float) args[1],
                              (float) args[2],
                              (float) args[3],
                              (float) args[4],
                              (float) args[5],
                              (float) args[6],
                              (float) args[7]
                             );
        }

        /// <summary>
        ///     ToFloat 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateFloat4(params object[] args)
        {
            return new float4((float) args[0], (float) args[1], (float) args[2], (float) args[3]);
        }

        /// <summary>
        ///     ToFloat 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateFloat3(params object[] args)
        {
            return new float3((float) args[0], (float) args[1], (float) args[2]);
        }

        /// <summary>
        ///     ToFloat 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateFloat2(params object[] args)
        {
            return new float2((float) args[0], (float) args[1]);
        }

        #endregion

        #region Byte

        /// <summary>
        ///     FromByte 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromByte16(object value)
        {
            uchar16 val = (uchar16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromByte 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromByte8(object value)
        {
            uchar8 val = (uchar8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromByte 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromByte4(object value)
        {
            uchar4 val = (uchar4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromByte 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromByte3(object value)
        {
            uchar3 val = (uchar3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromByte 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromByte2(object value)
        {
            uchar2 val = (uchar2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToByte 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateByte16(params object[] args)
        {
            return new uchar16(
                               (byte) args[0],
                               (byte) args[1],
                               (byte) args[2],
                               (byte) args[3],
                               (byte) args[4],
                               (byte) args[5],
                               (byte) args[6],
                               (byte) args[7],
                               (byte) args[8],
                               (byte) args[9],
                               (byte) args[10],
                               (byte) args[11],
                               (byte) args[12],
                               (byte) args[13],
                               (byte) args[14],
                               (byte) args[15]
                              );
        }

        /// <summary>
        ///     ToByte 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateByte8(params object[] args)
        {
            return new uchar8(
                              (byte) args[0],
                              (byte) args[1],
                              (byte) args[2],
                              (byte) args[3],
                              (byte) args[4],
                              (byte) args[5],
                              (byte) args[6],
                              (byte) args[7]
                             );
        }

        /// <summary>
        ///     ToByte 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateByte4(params object[] args)
        {
            return new uchar4((byte) args[0], (byte) args[1], (byte) args[2], (byte) args[3]);
        }

        /// <summary>
        ///     ToByte 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateByte3(params object[] args)
        {
            return new uchar3((byte) args[0], (byte) args[1], (byte) args[2]);
        }

        /// <summary>
        ///     ToByte 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateByte2(params object[] args)
        {
            return new uchar2((byte) args[0], (byte) args[1]);
        }

        #endregion

        #region SByte

        /// <summary>
        ///     FromSByte 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromSByte16(object value)
        {
            char16 val = (char16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromSByte 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromSByte8(object value)
        {
            char8 val = (char8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromSByte 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromSByte4(object value)
        {
            char4 val = (char4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromSByte 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromSByte3(object value)
        {
            char3 val = (char3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromSByte 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromSByte2(object value)
        {
            char2 val = (char2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }


        /// <summary>
        ///     ToSbyte 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateSByte16(params object[] args)
        {
            return new char16(
                              (sbyte) args[0],
                              (sbyte) args[1],
                              (sbyte) args[2],
                              (sbyte) args[3],
                              (sbyte) args[4],
                              (sbyte) args[5],
                              (sbyte) args[6],
                              (sbyte) args[7],
                              (sbyte) args[8],
                              (sbyte) args[9],
                              (sbyte) args[10],
                              (sbyte) args[11],
                              (sbyte) args[12],
                              (sbyte) args[13],
                              (sbyte) args[14],
                              (sbyte) args[15]
                             );
        }

        /// <summary>
        ///     ToSbyte 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateSByte8(params object[] args)
        {
            return new char8(
                             (sbyte) args[0],
                             (sbyte) args[1],
                             (sbyte) args[2],
                             (sbyte) args[3],
                             (sbyte) args[4],
                             (sbyte) args[5],
                             (sbyte) args[6],
                             (sbyte) args[7]
                            );
        }

        /// <summary>
        ///     ToSbyte 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateSByte4(params object[] args)
        {
            return new char4((sbyte) args[0], (sbyte) args[1], (sbyte) args[2], (sbyte) args[3]);
        }

        /// <summary>
        ///     ToSbyte 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateSByte3(params object[] args)
        {
            return new char3((sbyte) args[0], (sbyte) args[1], (sbyte) args[2]);
        }

        /// <summary>
        ///     ToSbyte 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateSByte2(params object[] args)
        {
            return new char2((sbyte) args[0], (sbyte) args[1]);
        }

        #endregion

        #region Long

        /// <summary>
        ///     FromLong 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromLong16(object value)
        {
            long16 val = (long16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromLong 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromLong8(object value)
        {
            long8 val = (long8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromLong 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromLong4(object value)
        {
            long4 val = (long4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromLong 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromLong3(object value)
        {
            long3 val = (long3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromLong 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromLong2(object value)
        {
            long2 val = (long2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToLong 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateLong16(params object[] args)
        {
            return new long16(
                              (long) args[0],
                              (long) args[1],
                              (long) args[2],
                              (long) args[3],
                              (long) args[4],
                              (long) args[5],
                              (long) args[6],
                              (long) args[7],
                              (long) args[8],
                              (long) args[9],
                              (long) args[10],
                              (long) args[11],
                              (long) args[12],
                              (long) args[13],
                              (long) args[14],
                              (long) args[15]
                             );
        }

        /// <summary>
        ///     ToLong 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateLong8(params object[] args)
        {
            return new long8(
                             (long) args[0],
                             (long) args[1],
                             (long) args[2],
                             (long) args[3],
                             (long) args[4],
                             (long) args[5],
                             (long) args[6],
                             (long) args[7]
                            );
        }

        /// <summary>
        ///     ToLong 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateLong4(params object[] args)
        {
            return new long4((long) args[0], (long) args[1], (long) args[2], (long) args[3]);
        }

        /// <summary>
        ///     ToLong 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateLong3(params object[] args)
        {
            return new long3((long) args[0], (long) args[1], (long) args[2]);
        }

        /// <summary>
        ///     ToLong 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateLong2(params object[] args)
        {
            return new long2((long) args[0], (long) args[1]);
        }

        #endregion

        #region ULong

        /// <summary>
        ///     FromULong 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromULong16(object value)
        {
            ulong16 val = (ulong16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromULong 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromULong8(object value)
        {
            ulong8 val = (ulong8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromULong 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromULong4(object value)
        {
            ulong4 val = (ulong4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromULong 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromULong3(object value)
        {
            ulong3 val = (ulong3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromULong 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromULong2(object value)
        {
            ulong2 val = (ulong2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToULong 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateULong16(params object[] args)
        {
            return new ulong16(
                               (ulong) args[0],
                               (ulong) args[1],
                               (ulong) args[2],
                               (ulong) args[3],
                               (ulong) args[4],
                               (ulong) args[5],
                               (ulong) args[6],
                               (ulong) args[7],
                               (ulong) args[8],
                               (ulong) args[9],
                               (ulong) args[10],
                               (ulong) args[11],
                               (ulong) args[12],
                               (ulong) args[13],
                               (ulong) args[14],
                               (ulong) args[15]
                              );
        }

        /// <summary>
        ///     ToULong 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateULong8(params object[] args)
        {
            return new ulong8(
                              (ulong) args[0],
                              (ulong) args[1],
                              (ulong) args[2],
                              (ulong) args[3],
                              (ulong) args[4],
                              (ulong) args[5],
                              (ulong) args[6],
                              (ulong) args[7]
                             );
        }

        /// <summary>
        ///     ToULong 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateULong4(params object[] args)
        {
            return new ulong4((ulong) args[0], (ulong) args[1], (ulong) args[2], (ulong) args[3]);
        }

        /// <summary>
        ///     ToULong 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateULong3(params object[] args)
        {
            return new ulong3((ulong) args[0], (ulong) args[1], (ulong) args[2]);
        }

        /// <summary>
        ///     ToULong 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateULong2(params object[] args)
        {
            return new ulong2((ulong) args[0], (ulong) args[1]);
        }

        #endregion

        #region Int

        /// <summary>
        ///     FromInt 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromInt16(object value)
        {
            int16 val = (int16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromInt 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromInt8(object value)
        {
            int8 val = (int8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromInt 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromInt4(object value)
        {
            int4 val = (int4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromInt 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromInt3(object value)
        {
            int3 val = (int3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromInt 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromInt2(object value)
        {
            int2 val = (int2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToInt 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateInt16(params object[] args)
        {
            return new int16(
                             (int) args[0],
                             (int) args[1],
                             (int) args[2],
                             (int) args[3],
                             (int) args[4],
                             (int) args[5],
                             (int) args[6],
                             (int) args[7],
                             (int) args[8],
                             (int) args[9],
                             (int) args[10],
                             (int) args[11],
                             (int) args[12],
                             (int) args[13],
                             (int) args[14],
                             (int) args[15]
                            );
        }

        /// <summary>
        ///     ToInt 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateInt8(params object[] args)
        {
            return new int8(
                            (int) args[0],
                            (int) args[1],
                            (int) args[2],
                            (int) args[3],
                            (int) args[4],
                            (int) args[5],
                            (int) args[6],
                            (int) args[7]
                           );
        }

        /// <summary>
        ///     ToInt 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateInt4(params object[] args)
        {
            return new int4((int) args[0], (int) args[1], (int) args[2], (int) args[3]);
        }

        /// <summary>
        ///     ToInt 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateInt3(params object[] args)
        {
            return new int3((int) args[0], (int) args[1], (int) args[2]);
        }

        /// <summary>
        ///     ToInt 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateInt2(params object[] args)
        {
            return new int2((int) args[0], (int) args[1]);
        }

        #endregion

        #region UInt

        /// <summary>
        ///     FromUInt 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUInt16(object value)
        {
            uint16 val = (uint16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromUInt 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUInt8(object value)
        {
            uint8 val = (uint8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromUInt 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUInt4(object value)
        {
            uint4 val = (uint4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromUInt 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUInt3(object value)
        {
            uint3 val = (uint3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromUInt 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUInt2(object value)
        {
            uint2 val = (uint2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToUInt 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUInt16(params object[] args)
        {
            return new uint16(
                              (uint) args[0],
                              (uint) args[1],
                              (uint) args[2],
                              (uint) args[3],
                              (uint) args[4],
                              (uint) args[5],
                              (uint) args[6],
                              (uint) args[7],
                              (uint) args[8],
                              (uint) args[9],
                              (uint) args[10],
                              (uint) args[11],
                              (uint) args[12],
                              (uint) args[13],
                              (uint) args[14],
                              (uint) args[15]
                             );
        }

        /// <summary>
        ///     ToUInt 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUInt8(params object[] args)
        {
            return new uint8(
                             (uint) args[0],
                             (uint) args[1],
                             (uint) args[2],
                             (uint) args[3],
                             (uint) args[4],
                             (uint) args[5],
                             (uint) args[6],
                             (uint) args[7]
                            );
        }

        /// <summary>
        ///     ToUInt 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUInt4(params object[] args)
        {
            return new uint4((uint) args[0], (uint) args[1], (uint) args[2], (uint) args[3]);
        }

        /// <summary>
        ///     ToUInt 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUInt3(params object[] args)
        {
            return new uint3((uint) args[0], (uint) args[1], (uint) args[2]);
        }

        /// <summary>
        ///     ToUInt 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUInt2(params object[] args)
        {
            return new uint2((uint) args[0], (uint) args[1]);
        }

        #endregion

        #region Short

        /// <summary>
        ///     FromShort 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] Fromshort16(object value)
        {
            short16 val = (short16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] Fromshort8(object value)
        {
            short8 val = (short8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] Fromshort4(object value)
        {
            short4 val = (short4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] Fromshort3(object value)
        {
            short3 val = (short3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] Fromshort2(object value)
        {
            short2 val = (short2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToShort 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateShort16(params object[] args)
        {
            return new short16(
                               (short) args[0],
                               (short) args[1],
                               (short) args[2],
                               (short) args[3],
                               (short) args[4],
                               (short) args[5],
                               (short) args[6],
                               (short) args[7],
                               (short) args[8],
                               (short) args[9],
                               (short) args[10],
                               (short) args[11],
                               (short) args[12],
                               (short) args[13],
                               (short) args[14],
                               (short) args[15]
                              );
        }

        /// <summary>
        ///     ToShort 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateShort8(params object[] args)
        {
            return new short8(
                              (short) args[0],
                              (short) args[1],
                              (short) args[2],
                              (short) args[3],
                              (short) args[4],
                              (short) args[5],
                              (short) args[6],
                              (short) args[7]
                             );
        }

        /// <summary>
        ///     ToShort 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateShort4(params object[] args)
        {
            return new short4((short) args[0], (short) args[1], (short) args[2], (short) args[3]);
        }

        /// <summary>
        ///     ToShort 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateShort3(params object[] args)
        {
            return new short3((short) args[0], (short) args[1], (short) args[2]);
        }

        /// <summary>
        ///     ToShort 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateShort2(params object[] args)
        {
            return new short2((short) args[0], (short) args[1]);
        }

        #endregion

        #region UShort

        /// <summary>
        ///     FromShort 16 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUShort16(object value)
        {
            ushort16 val = (ushort16) value;
            int num = 16;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 8 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUShort8(object value)
        {
            ushort8 val = (ushort8) value;
            int num = 8;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 4 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUShort4(object value)
        {
            ushort4 val = (ushort4) value;
            int num = 4;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 3 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUShort3(object value)
        {
            ushort3 val = (ushort3) value;
            int num = 3;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     FromShort 2 Converter
        /// </summary>
        /// <param name="value">The Value to be converted</param>
        /// <returns>The components as array</returns>
        private static object[] FromUShort2(object value)
        {
            ushort2 val = (ushort2) value;
            int num = 2;
            object[] ret = new object[num];
            for (int i = 0; i < num; i++)
            {
                ret[i] = val[i];
            }

            return ret;
        }

        /// <summary>
        ///     ToUShort 16 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUShort16(params object[] args)
        {
            return new ushort16(
                                (ushort) args[0],
                                (ushort) args[1],
                                (ushort) args[2],
                                (ushort) args[3],
                                (ushort) args[4],
                                (ushort) args[5],
                                (ushort) args[6],
                                (ushort) args[7],
                                (ushort) args[8],
                                (ushort) args[9],
                                (ushort) args[10],
                                (ushort) args[11],
                                (ushort) args[12],
                                (ushort) args[13],
                                (ushort) args[14],
                                (ushort) args[15]
                               );
        }

        /// <summary>
        ///     ToUShort 8 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUShort8(params object[] args)
        {
            return new ushort8(
                               (ushort) args[0],
                               (ushort) args[1],
                               (ushort) args[2],
                               (ushort) args[3],
                               (ushort) args[4],
                               (ushort) args[5],
                               (ushort) args[6],
                               (ushort) args[7]
                              );
        }

        /// <summary>
        ///     ToUShort 4 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUShort4(params object[] args)
        {
            return new ushort4((ushort) args[0], (ushort) args[1], (ushort) args[2], (ushort) args[3]);
        }

        /// <summary>
        ///     ToUShort 3 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUShort3(params object[] args)
        {
            return new ushort3((ushort) args[0], (ushort) args[1], (ushort) args[2]);
        }

        /// <summary>
        ///     ToUShort 2 Converter
        /// </summary>
        /// <param name="args">The Numbers to be converted</param>
        /// <returns>The CL Type</returns>
        private static object CreateUShort2(params object[] args)
        {
            return new ushort2((ushort) args[0], (ushort) args[1]);
        }

        #endregion

    }
}