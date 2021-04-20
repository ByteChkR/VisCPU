using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Namespaces
{

    public class HlNamespace : IEnumerable < HlNamespace >
    {

        public const string NS_SEPARATOR = "::";

        private readonly string m_Name;

        private readonly HlNamespace m_Parent;
        private readonly List < HlNamespace > m_Children;
        private readonly List < HlNamespace > m_ImportedNamespaces;

        public HlNamespace Parent => m_Parent;

        public HlNamespace Root => m_Parent?.Root ?? this;

        public string Name => m_Name;

        public string FullName => m_Parent != null ? m_Parent.FullName + NS_SEPARATOR + m_Name : m_Name;

        #region Public

        public static HlNamespace Parse( HlNamespace current, string name )
        {
            if ( current.RelativeExists( name ) )
            {
                return current.GetRecursive( name );
            }

            if ( current.Root.RelativeExists( name ) )
            {
                return current.Root.GetRecursive( name );
            }

            return null;
        }

        public HlNamespace Add( string name )
        {
            if ( HasChild( name ) )
            {
                return Get( name );
            }

            HlNamespace ns = new HlNamespace( this, name );
            m_Children.Add( ns );

            return ns;
        }

        public HlNamespace AddRecursive( string[] parts )
        {
            return InternalAddRecursive( parts, 0 );
        }

        public HlNamespace Get( string name )
        {
            return m_Children.FirstOrDefault( x => x.m_Name == name );
        }

        public IEnumerator < HlNamespace > GetEnumerator()
        {
            return m_Children.GetEnumerator();
        }

        public HlNamespace GetImported( string name )
        {
            return m_ImportedNamespaces.FirstOrDefault( x => x.Name == name );
        }

        public HlNamespace GetRecursive( string name )
        {
            string[] parts = name.Split( new[] { NS_SEPARATOR }, StringSplitOptions.None );

            return InternalGetRecursive( parts, 0 );
        }

        public bool HasChild( string name )
        {
            return m_Children.Any( x => x.m_Name == name );
        }

        public bool HasImported( string name )
        {
            return m_ImportedNamespaces.Any( x => x.Name == name );
        }

        public void Import( HlNamespace ns )
        {
            if ( m_ImportedNamespaces.Contains( ns ) )
            {
                return;
            }

            m_ImportedNamespaces.Add( ns );

            foreach ( HlNamespace hlNamespace in m_Children )
            {
                hlNamespace.Import( ns );
            }
        }

        public bool IsChild( HlNamespace other )
        {
            return HasChild( other.Name ) || m_Children.Any( x => x.IsChild( other ) );
        }

        public bool IsVisibleTo( HlNamespace other )
        {
            return FullName == other.FullName || other.HasImported( Name ) || other.IsChild( this );
        }

        public bool RelativeExists( string name )
        {
            string[] parts = name.Split( new[] { NS_SEPARATOR }, StringSplitOptions.None );

            return InternalExists( parts, 0 );
        }

        #endregion

        #region Protected

        protected HlNamespace( string name ) : this( null, name )
        {
        }

        #endregion

        #region Private

        private HlNamespace( HlNamespace parent, string name )
        {
            m_Name = name;
            m_Parent = parent;
            m_Children = new List < HlNamespace >();
            m_ImportedNamespaces = parent?.m_ImportedNamespaces.ToList() ?? new List < HlNamespace >();

            if ( parent != null )
            {
                m_ImportedNamespaces.Add( parent );
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_Children ).GetEnumerator();
        }

        private HlNamespace InternalAddRecursive( string[] parts, int currentIndex )
        {
            if ( currentIndex == parts.Length )
            {
                return this;
            }

            if ( !HasChild( parts[currentIndex] ) )
            {
                return Add( parts[currentIndex] ).InternalAddRecursive( parts, currentIndex + 1 );
            }

            return Get( parts[currentIndex] ).InternalAddRecursive( parts, currentIndex );
        }

        private bool InternalExists( string[] parts, int currentIndex )
        {
            if ( HasChild( parts[currentIndex] ) )
            {
                return Get( parts[currentIndex] ).InternalExists( parts, currentIndex + 1 );
            }

            return false;
        }

        private HlNamespace InternalGetRecursive( string[] parts, int currentIndex )
        {
            if ( HasChild( parts[currentIndex] ) )
            {
                return Get( parts[currentIndex] ).InternalGetRecursive( parts, currentIndex + 1 );
            }

            return null;
        }

        #endregion

    }

}
