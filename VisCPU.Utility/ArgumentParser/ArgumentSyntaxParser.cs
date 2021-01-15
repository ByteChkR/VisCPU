using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VisCPU.Utility.ArgumentParser
{

    public class ArgumentSyntaxParser
    {

        #region Public

        public static IEnumerable < string > GetArgNames( params object[] objs )
        {
            List < string > ret = new List < string >();

            foreach ( object o in objs )
            {
                Type t = o.GetType();

                FieldInfo[] fis = t.GetFields( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ).
                                    Where(
                                          x => x.FieldType == typeof( string ) ||
                                               x.FieldType == typeof( string[] ) ||
                                               x.FieldType == typeof( bool ) ||
                                               x.FieldType == typeof( bool[] ) ||
                                               x.FieldType == typeof( uint ) ||
                                               x.FieldType == typeof( uint[] ) ||
                                               x.FieldType.IsEnum ||
                                               x.FieldType.IsArray && x.FieldType.GetElementType().IsEnum
                                         ).
                                    ToArray();

                foreach ( FieldInfo fieldInfo in fis )
                {
                    foreach ( ArgumentAttribute attr in fieldInfo.GetCustomAttributes < ArgumentAttribute >() )
                    {
                        string name;

                        if ( attr.Name == null )
                        {
                            name = fieldInfo.Name;
                        }
                        else
                        {
                            name = attr.Name;
                        }

                        ret.Add( name );
                    }
                }
            }

            ret.Sort();

            return ret.Distinct();
        }

        public static void Parse(Dictionary <string, string> args, params object[] objs)
        {
            Dictionary<string, string[]> parts = new Dictionary<string, string[]>();

            foreach ( KeyValuePair < string, string > keyValuePair in args )
            {
                parts[keyValuePair.Key] = keyValuePair.Value.Split( ' ' );
            }

            Parse( parts, objs );
        }
        public static void Parse(Dictionary<string, string[]> parts, params object[] objs)
            {
            

            foreach (object o in objs)
            {
                Type t = o.GetType();

                FieldInfo[] fis = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                                    Where(
                                          x => x.FieldType == typeof(string) ||
                                               x.FieldType == typeof(string[]) ||
                                               x.FieldType == typeof(bool) ||
                                               x.FieldType == typeof(bool[]) ||
                                               x.FieldType == typeof(uint) ||
                                               x.FieldType == typeof(uint[]) ||
                                               x.FieldType.IsEnum ||
                                               x.FieldType.IsArray && x.FieldType.GetElementType().IsEnum
                                         ).
                                    ToArray();

                foreach (FieldInfo fieldInfo in fis)
                {
                    foreach (ArgumentAttribute attr in fieldInfo.GetCustomAttributes<ArgumentAttribute>())
                    {
                        string name;

                        if (attr.Name == null)
                        {
                            name = fieldInfo.Name;
                        }
                        else
                        {
                            name = attr.Name;
                        }

                        if (parts.ContainsKey(name))
                        {
                            if (fieldInfo.FieldType == typeof(string))
                            {
                                fieldInfo.SetValue(o, string.Concat(parts[name].Select(x => x + " ")).Trim());
                            }
                            else if (fieldInfo.FieldType == typeof(bool))
                            {
                                fieldInfo.SetValue(o, parts[name].Length == 0 || bool.Parse(parts[name].First()));
                            }
                            else if (fieldInfo.FieldType == typeof(uint))
                            {
                                fieldInfo.SetValue(o, uint.Parse(parts[name].First()));
                            }
                            else if (fieldInfo.FieldType.IsEnum)
                            {
                                fieldInfo.SetValue(
                                                   o,
                                                   Enum.ToObject(
                                                                 fieldInfo.FieldType,
                                                                 parts[name].
                                                                     Select(
                                                                            x => (int)Enum.Parse(
                                                                                 fieldInfo.FieldType,
                                                                                 x,
                                                                                 true
                                                                                )
                                                                           ).
                                                                     Aggregate((a, x) => a |= x)
                                                                )
                                                  );
                            }
                            else if (fieldInfo.FieldType == typeof(string[]))
                            {
                                fieldInfo.SetValue(o, parts[name]);
                            }
                            else if (fieldInfo.FieldType == typeof(bool[]))
                            {
                                fieldInfo.SetValue(o, parts[name].Select(bool.Parse).ToArray());
                            }
                            else if (fieldInfo.FieldType == typeof(uint[]))
                            {
                                fieldInfo.SetValue(o, parts[name].Select(uint.Parse).ToArray());
                            }
                            else if (fieldInfo.FieldType.IsArray && fieldInfo.FieldType.GetElementType().IsEnum)
                            {
                                fieldInfo.SetValue(
                                                   o,
                                                   Enum.ToObject(
                                                                 fieldInfo.FieldType.GetElementType(),
                                                                 parts[name].
                                                                     Select(
                                                                            x => (int)Enum.Parse(
                                                                                 fieldInfo.FieldType.GetElementType(),
                                                                                 x,
                                                                                 true
                                                                                )
                                                                           ).
                                                                     ToArray()
                                                                )
                                                  );
                            }
                        }
                    }
                }
            }
        }


        public static void Parse( string[] args, params object[] objs )
        {

            List<string> p = new List<string>();
            Dictionary < string, string[] > parts = new Dictionary < string, string[] >();
            string lastArgName = "";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    parts.Add(lastArgName, p.ToArray());
                    p.Clear();
                    lastArgName = args[i].Remove(0, 1);
                }
                else
                {
                    p.Add(args[i]);
                }
            }

            parts.Add(lastArgName, p.ToArray());
            Parse( parts, objs );
        }

        #endregion

    }

}
