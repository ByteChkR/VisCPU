using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.HL.Modules.Resolvers
{

    public static class ModuleResolver
    {

        public static ModuleResolverSettings ResolverSettings;

        private static Dictionary < string, ModuleManager > s_Managers;

        #region Public

        public static void AddManager( string name, string url )
        {
            if ( Uri.TryCreate( url, UriKind.Absolute, out Uri u ) )
            {
                if ( u.Scheme == "http" || u.Scheme == "https" )
                {
                    s_Managers[name] = new HttpModuleManager( name, u.OriginalString );
                }
                else if ( u.Scheme == "dev" )
                {
                    s_Managers[name] = new TCPUploadModuleManager( u.OriginalString );
                }
                else
                {
                    s_Managers[name] = new LocalModuleManager( url );
                }
            }
            else
            {
                s_Managers[name] = new LocalModuleManager( url );
            }
        }

        public static ModuleManager GetManager( string name )
        {
            return s_Managers[name];
        }

        public static IEnumerable < KeyValuePair < string, ModuleManager > > GetManagers()
        {
            return s_Managers;
        }

        public static void Initialize()
        {
            if ( ResolverSettings == null && s_Managers == null )
            {
                Directory.CreateDirectory( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config/module" ) );
                ResolverSettings = ModuleResolverSettings.Create();
                s_Managers = new Dictionary < string, ModuleManager >();

                foreach ( KeyValuePair < string, string > origin in ResolverSettings.ModuleOrigins )
                {
                    AddManager( origin.Key, origin.Value );
                }
            }
        }

        public static ProjectConfig Resolve( string name, string version = "ANY" )
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

        public static ProjectConfig Resolve( ModuleManager manager, ProjectDependency dependency )
        {
            return manager.GetPackage( dependency.ProjectName ).
                           GetInstallTarget( dependency.ProjectVersion == "ANY" ? null : dependency.ProjectVersion );
        }

        #endregion

        #region Private

        static ModuleResolver()
        {
            Initialize();
        }

        #endregion

    }

}
