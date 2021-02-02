using System;
using System.Collections.Generic;
using System.Linq;
using VisCPU.ProjectSystem.Data;
using VisCPU.ProjectSystem.Database;
using VisCPU.ProjectSystem.Database.Implementations;
using VisCPU.Utility.Settings;

namespace VisCPU.ProjectSystem.Resolvers
{

    public static class ProjectResolver
    {
        private static Dictionary < string, ProjectDatabase > s_Managers;

        public static ProjectResolverSettings ResolverSettings { get; set; }

        #region Public

        public static void AddManager( string name, string url )
        {
            if ( Uri.TryCreate( url, UriKind.Absolute, out Uri u ) )
            {
                if ( u.Scheme == "http" || u.Scheme == "https" )
                {
                    s_Managers[name] = new HttpProjectDatabase( name, u.OriginalString );
                }
                else if ( u.Scheme == "dev" )
                {
                    s_Managers[name] = new TcpProjectDatabase( u.OriginalString );
                }
                else
                {
                    s_Managers[name] = new LocalProjectDatabase( url );
                }
            }
            else
            {
                s_Managers[name] = new LocalProjectDatabase( url );
            }
        }

        public static ProjectDatabase GetManager( string name )
        {
            return s_Managers[name];
        }

        public static IEnumerable < KeyValuePair < string, ProjectDatabase > > GetManagers()
        {
            return s_Managers;
        }

        public static void Initialize()
        {
            if ( ResolverSettings == null && s_Managers == null )
            {
                SettingsCategories.Get( "sdk.module", true );
                ResolverSettings = SettingsManager.GetSettings < ProjectResolverSettings >();
                s_Managers = new Dictionary < string, ProjectDatabase >();

                foreach ( KeyValuePair < string, string > origin in ResolverSettings.ModuleOrigins )
                {
                    AddManager( origin.Key, origin.Value );
                }
            }
        }

        public static ProjectConfig Resolve( string name )
        {
            return Resolve( name, "ANY" );
        }

        public static ProjectConfig Resolve( string name, string version )
        {
            return s_Managers.First( x => x.Value.HasPackage( name ) ).
                              Value.
                              GetPackage( name ).
                              GetInstallTarget( version == "ANY" ? null : version );
        }

        public static ProjectConfig Resolve( string repository, ProjectDependency dependency )
        {
            return s_Managers[repository].
                   GetPackage( dependency.ProjectName ).
                   GetInstallTarget( dependency.ProjectVersion == "ANY" ? null : dependency.ProjectVersion );
        }

        public static ProjectConfig Resolve( ProjectDatabase manager, ProjectDependency dependency )
        {
            return manager.GetPackage( dependency.ProjectName ).
                           GetInstallTarget( dependency.ProjectVersion == "ANY" ? null : dependency.ProjectVersion );
        }

        #endregion

        #region Private

        static ProjectResolver()
        {
            Initialize();
        }

        #endregion
    }

}
