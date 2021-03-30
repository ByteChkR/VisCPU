using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utility.ADL.Configs;

namespace Utility.ADL
{

    public class ADLLogger
    {
        private static readonly Dictionary < IProjectDebugConfig, List < ADLLogger > > LoggerMap =
            new Dictionary < IProjectDebugConfig, List < ADLLogger > >();

        private readonly IProjectDebugConfig ProjectDebugConfig;
        private bool hasProcessedPrefixes;

        private Dictionary < int, string > prefixes = new Dictionary < int, string >();
        private string SubProjectName;

        public virtual string[] ProjectMaskPrefixes { get; } = new string[0];

        internal Dictionary < int, string > Prefixes
        {
            get
            {
                if ( !hasProcessedPrefixes )
                {
                    prefixes = ProcessPrefixes( ProjectMaskPrefixes );
                }

                return prefixes;
            }
        }

        #region Public

        /// <summary>
        ///     Dictionary of Prefixes for the corresponding Masks
        /// </summary>
        public ADLLogger( IProjectDebugConfig projectDebugConfig, string subProjectName = "" )
        {
            ProjectDebugConfig = projectDebugConfig;
            SubProjectName = subProjectName;

            Register( this );
        }

        public static ReadOnlyDictionary < IProjectDebugConfig, List < ADLLogger > > GetReadOnlyLoggerMap()
        {
            return new ReadOnlyDictionary < IProjectDebugConfig, List < ADLLogger > >( LoggerMap );
        }

        public void AddPrefixForMask( BitMask mask, string prefix )
        {
            Debug.AddPrefixForMask( Prefixes, mask, prefix );
        }

        public Dictionary < int, string > GetAllPrefixes()
        {
            return Debug.GetAllPrefixes( Prefixes );
        }

        public string GetMaskPrefix( BitMask mask )
        {
            return Debug.GetMaskPrefix( Prefixes, mask );
        }

        public bool GetPrefixMask( string prefix, out BitMask mask )
        {
            return Debug.GetPrefixMask( Prefixes, prefix, out mask );
        }

        public void Log( int mask, string message, int severity )
        {
            if ( ProjectDebugConfig.GetMinSeverity() < severity )
            {
                return;
            }

            string subp = "";

            if ( !string.IsNullOrEmpty( SubProjectName ) )
            {
                subp = $"[{SubProjectName}]";
            }

            string sev = "";

            if ( Debug.ShowSeverity )
            {
                sev += $"[S:{severity}]";
            }

            Debug.Log( this, mask, $"[{ProjectDebugConfig.GetProjectName()}]{subp}{sev}: {message}" );
        }

        public void RemoveAllPrefixes()
        {
            Debug.RemoveAllPrefixes( Prefixes );
        }

        public void RemovePrefixForMask( BitMask mask )
        {
            Debug.RemovePrefixForMask( Prefixes, mask );
        }

        public void SetAllPrefixes( params string[] prefixNames )
        {
            Debug.SetAllPrefixes( Prefixes, prefixNames );
        }

        public void SetSubProjectName( string subProjectName )
        {
            SubProjectName = subProjectName;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty( SubProjectName ) ? ProjectDebugConfig.GetProjectName() : SubProjectName;
        }

        #endregion

        #region Private

        private static void Register( ADLLogger logger )
        {
            if ( LoggerMap.ContainsKey( logger.ProjectDebugConfig ) )
            {
                LoggerMap[logger.ProjectDebugConfig].Add( logger );
            }
            else
            {
                LoggerMap[logger.ProjectDebugConfig] = new List < ADLLogger > { logger };
            }
        }

        //For completeness sake
        private static void UnRegister( ADLLogger logger )
        {
            if ( LoggerMap.ContainsKey( logger.ProjectDebugConfig ) )
            {
                LoggerMap[logger.ProjectDebugConfig].Remove( logger );
            }
        }

        private Dictionary < int, string > ProcessPrefixes( string[] prefixes )
        {
            if ( prefixes.Length > sizeof( int ) * 8 )
            {
                throw new InvalidOperationException( "Can not add more than " + sizeof( int ) * 8 + " prefixes" );
            }

            Dictionary < int, string > ret = new Dictionary < int, string >();
            int s = 1;

            for ( int i = 0; i < prefixes.Length; i++ )
            {
                ret[s] = prefixes[i];
                s <<= 1;
            }

            hasProcessedPrefixes = true;

            return ret;
        }

        #endregion
    }

    public class ADLLogger < T > : ADLLogger
        where T : struct
    {
        public override string[] ProjectMaskPrefixes
        {
            get
            {
                List < string > names = Enum.GetNames( typeof( T ) ).ToList();

                for ( int i = names.Count - 1; i >= 0; i-- )
                {
                    if ( !IsPowerOfTwo( ( int ) Enum.Parse( typeof( T ), names[i] ) ) )
                    {
                        names.RemoveAt( i );
                    }
                }

                return names.ToArray();
            }
        }

        #region Public

        public ADLLogger( IProjectDebugConfig projectDebugConfig, string subProjectname = "" ) : base(
            projectDebugConfig,
            subProjectname
        )
        {
        }

        public void Log( T mask, string message, int severity )
        {
            Log( Convert.ToInt32( mask ), message, severity );
        }

        #endregion

        #region Protected

        protected bool IsPowerOfTwo( int value )
        {
            return value != 0 && ( value & ( value - 1 ) ) == 0;
        }

        #endregion
    }

}
