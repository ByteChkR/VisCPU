using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace VisCPU.Utility.Extensions
{

    internal class ExtensionLoadResult < T > : IEnumerable < T >
    {

        private readonly string[] m_Files;

        #region Public

        public ExtensionLoadResult( string directory, bool recursive )
        {
            m_Files = Directory.GetFiles(
                                         Path.GetFullPath( directory ),
                                         "*.dll",
                                         recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                                        );
        }

        public ExtensionLoadResult( string[] files )
        {
            m_Files = files;
        }

        #endregion

        #region Private

        private bool CanInstanciate( Type t )
        {
            return typeof( T ).IsAssignableFrom( t ) && !t.IsAbstract && t.GetConstructor( new Type[0] ) != null;
        }

        IEnumerator < T > IEnumerable < T >.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator < T > GetEnumerator()
        {
            int i = 0;

            while ( i < m_Files.Length )
            {
                if ( TryLoadAssembly( m_Files[i], out Assembly asm ) )
                {
                    IEnumerable < Type > types = GetExtensionTypes( asm );

                    foreach ( Type extensionType in types )
                    {
                        yield return ( T ) Activator.CreateInstance( extensionType );
                    }
                }

                i++;
            }
        }

        private IEnumerable < Type > GetExtensionTypes( Assembly asm )
        {
            return asm.GetTypes().Where( CanInstanciate );
        }

        private bool TryLoadAssembly( string file, out Assembly asm )
        {
            try
            {
                asm = Assembly.LoadFrom( file );

                return true;
            }
            catch ( Exception )
            {
                asm = null;

                return false;
            }
        }

        #endregion

    }

}
