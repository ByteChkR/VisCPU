using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings.Events;

namespace VisCPU.Utility.Settings
{

    public static class SettingsCategories
    {
        private static string DefaultConfigDir => UnityIsAPieceOfShitHelper.AppRoot;

        private static readonly List < (string rootDir, SettingsCategory category) > s_RootCategories =
            new List < (string, SettingsCategory) >();

        #region Public

        public static SettingsCategory Add( string rootDir, string name )
        {
            if ( s_RootCategories.Any( x => x.category.CategoryName == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new SettingsRootCategoryExistsEvent( name ) );

                return null;
            }

            Directory.CreateDirectory( rootDir );
            SettingsCategory ret = new SettingsCategory( name );
            s_RootCategories.Add( ( rootDir, ret ) );

            return ret;
        }

        public static SettingsCategory Get( string fullName, bool create )
        {
            string[] parts = fullName.Split( '.' );
            SettingsCategory current = null;

            foreach ( (string rootDir, SettingsCategory category) entry in s_RootCategories )
            {
                if ( entry.category.CategoryName == parts[0] )
                {
                    current = entry.category;

                    break;
                }
            }

            if ( current == null )
            {
                if ( !create )
                {
                    return null;
                }

                current = Add( Path.Combine( DefaultConfigDir, "configs" ), parts[0] );
            }

            for ( int i = 1; i < parts.Length; i++ )
            {
                SettingsCategory next = current.GetCategory( parts[i] );

                if ( next == null )
                {
                    if ( !create )
                    {
                        return null;
                    }

                    next = current.AddCategory( parts[i] );
                }

                current = next;
            }

            return current;
        }

        public static string GetRootDir( SettingsCategory category )
        {
            SettingsCategory root = category.Root;

            foreach ( (string rootDir, SettingsCategory category) entry in s_RootCategories )
            {
                if ( entry.category == root )
                {
                    return entry.rootDir;
                }
            }

            return "";
        }
        #endregion
    }

}
