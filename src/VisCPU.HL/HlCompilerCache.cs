using System.Collections.Generic;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.HL
{

    public static class HlCompilerCache
    {
        private static readonly Dictionary < string, HlCompilation > m_Compilations =
            new Dictionary < string, HlCompilation >();

        #region Public

        public static void Add( string src, HlCompilation comp )
        {
            if ( SettingsManager.GetSettings < HlCompilerSettings >().EnableCompilationCaching )
            {
                m_Compilations[src] = comp;
            }
            else
            {
                comp.Unload();
            }
        }

        public static HlCompilation Get( string src )
        {
            return m_Compilations[src];
        }

        public static bool HasCached( string src )
        {
            return m_Compilations.ContainsKey( src );
        }

        #endregion
    }

}
