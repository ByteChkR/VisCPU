using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings.Events;
using VisCPU.Utility.Settings.Loader;

namespace VisCPU.Utility.Settings
{

    public static class SettingsManager
    {
        private struct SettingsEntry : IEquatable < SettingsEntry >
        {
            public readonly string DefaultFile;
            public object CachedObject;
            public readonly SettingsLoader FileLoader;

            public SettingsEntry( string defaultFile, SettingsLoader loader )
            {
                CachedObject = null;
                DefaultFile = Path.GetFullPath( Path.Combine( UnityIsAPieceOfShitHelper.AppRoot, defaultFile ) );
                Directory.CreateDirectory( Path.GetDirectoryName( DefaultFile ) );
                FileLoader = loader;
            }

            public bool Equals( SettingsEntry other )
            {
                return DefaultFile == other.DefaultFile && Equals( FileLoader, other.FileLoader );
            }

            public override bool Equals( object obj )
            {
                return obj is SettingsEntry other && Equals( other );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( DefaultFile != null ? DefaultFile.GetHashCode() : 0 ) * 397 ) ^
                           ( FileLoader != null ? FileLoader.GetHashCode() : 0 );
                }
            }
        }

        private static readonly Dictionary < Type, SettingsEntry > s_DefaultLoaderMap =
            new Dictionary < Type, SettingsEntry >();

        public static bool EnableIo { get; set; } = true;

        #region Public

        public static bool DefaultExists( Type t )
        {
            return s_DefaultLoaderMap.ContainsKey( t );
        }

        public static bool DefaultExists < T >()
        {
            return DefaultExists( typeof( T ) );
        }

        public static bool DefaultFileExists( Type t )
        {
            return s_DefaultLoaderMap.ContainsKey( t ) && File.Exists( s_DefaultLoaderMap[t].DefaultFile );
        }

        public static bool DefaultFileExists < T >()
        {
            return DefaultFileExists( typeof( T ) );
        }

        public static string GetDefaultFile < T >()
        {
            if ( DefaultExists < T >() )
            {
                return s_DefaultLoaderMap[typeof( T )].DefaultFile;
            }

            return null;
        }

        public static T GetSettings < T >()
        {
            return ( T ) GetSettings( typeof( T ) );
        }

        public static T GetSettings < T >( string file )
        {
            return ( T ) GetSettings( typeof( T ), file );
        }

        public static T GetSettings < T >( SettingsLoader loader, string file )
        {
            return ( T ) GetSettings( loader, typeof( T ), file );
        }

        public static object GetSettings( Type t )
        {
            //Ensure that the static constructor of the type has ran.
            RuntimeHelpers.RunClassConstructor( t.TypeHandle );

            if ( s_DefaultLoaderMap.ContainsKey( t ) )
            {
                if ( s_DefaultLoaderMap[t].CachedObject != null )
                {
                    return s_DefaultLoaderMap[t].CachedObject;
                }

                SettingsEntry e = s_DefaultLoaderMap[t];
                e.CachedObject = GetSettings( s_DefaultLoaderMap[t].FileLoader, t, s_DefaultLoaderMap[t].DefaultFile );
                s_DefaultLoaderMap[t] = e;

                return e.CachedObject;
            }

            EventManager < ErrorEvent >.SendEvent( new SettingsLoaderNotFoundEvent( t ) );

            return null;
        }

        public static object GetSettings( Type t, string file )
        {
            RuntimeHelpers.RunClassConstructor( t.TypeHandle );

            if ( s_DefaultLoaderMap.ContainsKey( t ) )
            {
                return GetSettings( s_DefaultLoaderMap[t].FileLoader, t, file );
            }

            EventManager < ErrorEvent >.SendEvent( new SettingsLoaderNotFoundEvent( t ) );

            return null;
        }

        public static object GetSettings( SettingsLoader loader, Type t, string file )
        {
            if ( EnableIo )
            {
                return loader.LoadSettings( t, file );
            }

            EventManager < ErrorEvent >.SendEvent( new SettingsIoDisabledEvent( "IO Is disabled." ) );

            return null;
        }

        public static void RegisterDefaultLoader < T >(
            SettingsLoader loader,
            SettingsCategory category,
            string defaultFile )
        {
            RegisterDefaultLoader( typeof( T ), loader, category, defaultFile );
        }

        public static void RegisterDefaultLoader < T >(
            SettingsLoader loader,
            SettingsCategory category,
            string defaultFile,
            T defaultValues )
        {
            RegisterDefaultLoader( typeof( T ), loader, category, defaultFile, defaultValues );
        }

        public static void RegisterDefaultLoader(
            Type t,
            SettingsLoader loader,
            SettingsCategory category,
            string defaultFileName )
        {
            s_DefaultLoaderMap[t] = new SettingsEntry(
                Path.Combine( category.GetCategoryDirectory(), defaultFileName ),
                loader
            );
        }

        public static void RegisterDefaultLoader(
            Type t,
            SettingsLoader loader,
            SettingsCategory category,
            string defaultFile,
            object defaultValues )
        {
            s_DefaultLoaderMap[t] = new SettingsEntry(
                Path.Combine( category.GetCategoryDirectory(), defaultFile ),
                loader
            );

            if ( !DefaultFileExists( t ) )
            {
                SaveSettings( loader, defaultValues, s_DefaultLoaderMap[t].DefaultFile );
            }
        }

        public static void SaveSettings( object o )
        {
            if ( s_DefaultLoaderMap.ContainsKey( o.GetType() ) )
            {
                SaveSettings(
                    s_DefaultLoaderMap[o.GetType()].FileLoader,
                    o,
                    s_DefaultLoaderMap[o.GetType()].DefaultFile
                );

                return;
            }

            EventManager < ErrorEvent >.SendEvent( new SettingsLoaderNotFoundEvent( o.GetType() ) );
        }

        public static void SaveSettings( object o, string file )
        {
            if ( s_DefaultLoaderMap.ContainsKey( o.GetType() ) )
            {
                SaveSettings( s_DefaultLoaderMap[o.GetType()].FileLoader, o, file );
            }

            EventManager < ErrorEvent >.SendEvent( new SettingsLoaderNotFoundEvent( o.GetType() ) );
        }

        public static void SaveSettings( SettingsLoader loader, object o, string file )
        {
            if ( EnableIo )
            {
                loader.SaveSettings( o, file );
            }
        }

        #endregion
    }

}
