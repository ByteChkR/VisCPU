using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VisCPU.Utility.ArgumentParser
{

    public static class ArgumentSyntaxParser
    {
        #region Public

        public static IEnumerable < string > GetArgNames( params object[] objs )
        {
            List < string > ret = new List < string >();

            foreach ( object o in objs )
            {
                Type t = o.GetType();

                MemberInfo[] fis = t.GetMembers( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ).
                                     Where(
                                         IsCorrectMemberType
                                     ).
                                     ToArray();

                foreach ( MemberInfo fieldInfo in fis )
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

        public static void Parse( Dictionary < string, string > args, params object[] objs )
        {
            Dictionary < string, string[] > parts = new Dictionary < string, string[] >();

            foreach ( KeyValuePair < string, string > keyValuePair in args )
            {
                parts[keyValuePair.Key] = keyValuePair.Value.Split( ' ' );
            }

            Parse( parts, objs );
        }

        public static void Parse( Dictionary < string, string[] > parts, params object[] objs )
        {
            foreach ( object o in objs )
            {
                Type t = o.GetType();

                MemberInfo[] fis = t.GetMembers( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance ).
                                     Where(
                                         IsCorrectMemberType
                                     ).
                                     ToArray();

                foreach ( MemberInfo fieldInfo in fis )
                {
                    ( Type fieldType, Action < object, object > setDel ) = GetInvocable( fieldInfo );

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

                        if ( parts.ContainsKey( name ) )
                        {
                            if ( fieldType == typeof( string ) )
                            {
                                setDel( o, string.Concat( parts[name].Select( x => x + " " ) ).Trim() );
                            }
                            else if ( fieldType == typeof( bool ) )
                            {
                                setDel( o, parts[name].Length == 0 || bool.Parse( parts[name].First() ) );
                            }
                            else if ( fieldType == typeof( uint ) )
                            {
                                setDel( o, parts[name].First().ParseUInt() );
                            }
                            else if ( fieldType.IsEnum )
                            {
                                setDel(
                                    o,
                                    Enum.ToObject(
                                        fieldType,
                                        parts[name].
                                            Select(
                                                x => ( int ) Enum.Parse(
                                                    fieldType,
                                                    x,
                                                    true
                                                )
                                            ).
                                            Aggregate( ( a, x ) => a | x )
                                    )
                                );
                            }
                            else if ( fieldType == typeof( string[] ) )
                            {
                                setDel( o, parts[name] );
                            }
                            else if ( fieldType == typeof( bool[] ) )
                            {
                                setDel( o, parts[name].Select( bool.Parse ).ToArray() );
                            }
                            else if ( fieldType == typeof( uint[] ) )
                            {
                                setDel( o, parts[name].Select( uint.Parse ).ToArray() );
                            }
                            else if ( fieldType.IsArray && fieldType.GetElementType().IsEnum )
                            {
                                setDel(
                                    o,
                                    Enum.ToObject(
                                        fieldType.GetElementType(),
                                        parts[name].
                                            Select(
                                                x => ( int ) Enum.Parse(
                                                    fieldType.GetElementType(),
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
            List < string > p = new List < string >();
            Dictionary < string, string[] > parts = new Dictionary < string, string[] >();
            string lastArgName = "";

            for ( int i = 0; i < args.Length; i++ )
            {
                if ( args[i].StartsWith( "-" ) )
                {
                    parts.Add( lastArgName, p.ToArray() );
                    p.Clear();
                    lastArgName = args[i].Remove( 0, 1 );
                }
                else
                {
                    p.Add( args[i] );
                }
            }

            parts.Add( lastArgName, p.ToArray() );
            Parse( parts, objs );
        }

        #endregion

        #region Private

        private static (Type, Action < object, object >) GetInvocable( MemberInfo mi )
        {
            if ( mi is PropertyInfo pi )
            {
                return ( pi.PropertyType, pi.SetValue );
            }

            if ( mi is FieldInfo fi )
            {
                return ( fi.FieldType, fi.SetValue );
            }

            return ( typeof( object ), ( o, o1 ) => { } );
        }

        private static bool IsCorrectFieldType( FieldInfo x )
        {
            return x.FieldType == typeof( string ) ||
                   x.FieldType == typeof( string[] ) ||
                   x.FieldType == typeof( bool ) ||
                   x.FieldType == typeof( bool[] ) ||
                   x.FieldType == typeof( uint ) ||
                   x.FieldType == typeof( uint[] ) ||
                   x.FieldType.IsEnum ||
                   x.FieldType.IsArray && x.FieldType.GetElementType().IsEnum;
        }

        private static bool IsCorrectMemberType( MemberInfo x )
        {
            return x is FieldInfo fi ? IsCorrectFieldType( fi ) : x is PropertyInfo pi && IsCorrectPropertyType( pi );
        }

        private static bool IsCorrectPropertyType( PropertyInfo x )
        {
            return x.PropertyType == typeof( string ) ||
                   x.PropertyType == typeof( string[] ) ||
                   x.PropertyType == typeof( bool ) ||
                   x.PropertyType == typeof( bool[] ) ||
                   x.PropertyType == typeof( uint ) ||
                   x.PropertyType == typeof( uint[] ) ||
                   x.PropertyType.IsEnum ||
                   x.PropertyType.IsArray && x.PropertyType.GetElementType().IsEnum && x.CanWrite;
        }

        #endregion
    }

}
