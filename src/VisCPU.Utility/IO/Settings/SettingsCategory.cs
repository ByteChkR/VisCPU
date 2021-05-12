﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisCPU.Utility.IO.Settings
{

    public class SettingsCategory : IEnumerable < SettingsCategory >
    {

        private readonly List < SettingsCategory > m_SubCategories = new List < SettingsCategory >();

        public SettingsCategory Parent { get; }

        public SettingsCategory Root => Parent?.Root ?? this;

        public string CategoryName { get; }

        public string FullCategoryName => ( Parent != null ? Parent.FullCategoryName + "." : "" ) + CategoryName;

        #region Public

        internal SettingsCategory( string name ) : this( null, name )
        {
        }

        internal SettingsCategory( SettingsCategory parent, string name )
        {
            CategoryName = name;
            Parent = parent;
        }

        public SettingsCategory AddCategory( string name )
        {
            if ( m_SubCategories.Any( x => x.CategoryName == name ) )
            {
                return GetCategory( name );
            }

            SettingsCategory ret = new SettingsCategory( this, name );
            Directory.CreateDirectory( ret.GetCategoryDirectory() );
            m_SubCategories.Add( ret );

            return ret;
        }

        public SettingsCategory GetCategory( string name )
        {
            return m_SubCategories.FirstOrDefault( x => x.CategoryName == name );
        }

        public string GetCategoryDirectory()
        {
            return GetCategoryDirectory( null );
        }

        public string GetCategoryDirectory( string root )
        {
            List < string > parts = new List < string > { root ?? SettingsCategories.GetRootDir( this ) };
            parts.AddRange( FullCategoryName.Split( '.' ) );

            return Path.Combine( parts.ToArray() );
        }

        public IEnumerator < SettingsCategory > GetEnumerator()
        {
            return m_SubCategories.GetEnumerator();
        }

        #endregion

        #region Private

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_SubCategories ).GetEnumerator();
        }

        #endregion

    }

}
