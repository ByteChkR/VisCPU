using System;
using System.Collections.Generic;
using System.IO;

using VisCPU.Utility.Events;

namespace VisCPU.Utility.Settings
{

    public static class Settings
    {

        private struct SettingsEntry
        {

            public readonly string DefaultFile;
            public object CachedObject;
            public readonly SettingsLoader FileLoader;

            public SettingsEntry(string defaultFile, SettingsLoader loader)
            {
                CachedObject = null;
                DefaultFile = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,defaultFile));
                Directory.CreateDirectory( Path.GetDirectoryName( DefaultFile ) );
                FileLoader = loader;
            }
        }

        public static bool DefaultExists(Type t) => defaultLoaderMap.ContainsKey(t);
        public static bool DefaultFileExists(Type t) => defaultLoaderMap.ContainsKey(t) && File.Exists(defaultLoaderMap[t].DefaultFile);

        public static bool DefaultExists<T>() => DefaultExists(typeof(T));
        public static bool DefaultFileExists<T>() => DefaultFileExists(typeof(T));

        private static readonly Dictionary<Type, SettingsEntry> defaultLoaderMap =
            new Dictionary<Type, SettingsEntry>();

        public static void RegisterDefaultLoader<T>(SettingsLoader loader, string defaultFile) =>
            RegisterDefaultLoader(typeof(T), loader, defaultFile); 
        
        public static void RegisterDefaultLoader<T>(SettingsLoader loader, string defaultFile, T defaultValues) =>
            RegisterDefaultLoader(typeof(T), loader, defaultFile, defaultValues);

        public static void RegisterDefaultLoader(Type t, SettingsLoader loader, string defaultFile)
        {
            defaultLoaderMap[t] = new SettingsEntry(defaultFile, loader);
        }

        public static void RegisterDefaultLoader(Type t, SettingsLoader loader, string defaultFile, object defaultValues)
        {
            defaultLoaderMap[t] = new SettingsEntry(defaultFile, loader);
            SaveSettings(loader, defaultValues, defaultFile);
        }

        public static T GetSettings<T>() => (T)GetSettings(typeof(T));

        public static T GetSettings<T>(string file) => (T)GetSettings(typeof(T), file);

        public static T GetSettings<T>(SettingsLoader loader, string file) => (T)GetSettings(loader, typeof(T), file);

        public static object GetSettings(Type t)
        {
            if (defaultLoaderMap.ContainsKey(t))
            {
                if ( defaultLoaderMap[t].CachedObject != null )
                    return defaultLoaderMap[t].CachedObject;

                SettingsEntry e = defaultLoaderMap[t];
                e.CachedObject= GetSettings(defaultLoaderMap[t].FileLoader, t, defaultLoaderMap[t].DefaultFile);
                defaultLoaderMap[t] = e;
                return e.CachedObject;
            }

            EventManager<ErrorEvent>.SendEvent(new SettingsLoaderNotFoundEvent(t));

            return null;
        }

        public static object GetSettings(Type t, string file)
        {
            if (defaultLoaderMap.ContainsKey(t))
            {
                return GetSettings(defaultLoaderMap[t].FileLoader, t, file);
            }

            EventManager<ErrorEvent>.SendEvent(new SettingsLoaderNotFoundEvent(t));

            return null;
        }

        public static object GetSettings(SettingsLoader loader, Type t, string file)
        {
            return loader.LoadSettings(t, file);
        }

        public static void SaveSettings( object o )
        {
            if (defaultLoaderMap.ContainsKey(o.GetType()))
            {
                SaveSettings(defaultLoaderMap[o.GetType()].FileLoader, o, defaultLoaderMap[o.GetType()].DefaultFile);

                return;
            }

            EventManager<ErrorEvent>.SendEvent(new SettingsLoaderNotFoundEvent(o.GetType(), true));
        }

        public static void SaveSettings( object o, string file )
        {
            if (defaultLoaderMap.ContainsKey(o.GetType()))
            {
                SaveSettings(defaultLoaderMap[o.GetType()].FileLoader, o, file);
            }

            EventManager<ErrorEvent>.SendEvent(new SettingsLoaderNotFoundEvent(o.GetType(), true));
        }
        
        public static void SaveSettings( SettingsLoader loader, object o, string file )
        {
            loader.SaveSettings(o, file);
        }

    }

}