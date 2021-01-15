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

        private static Dictionary < string, ModuleManager > Managers;

        #region Public

        public static void AddManager( string name, string url )
        {
            if ( Uri.TryCreate( url, UriKind.Absolute, out Uri u ) )
            {
                if ( u.Scheme == "http" || u.Scheme == "https" )
                {
                    Managers[name] = new HttpModuleManager( name, u.OriginalString );
                }
                else if ( u.Scheme == "dev" )
                {
                    Managers[name] = new TCPUploadModuleManager( u.OriginalString );
                }
                else
                {
                    Managers[name] = new LocalModuleManager( url );
                }
            }
            else
            {
                Managers[name] = new LocalModuleManager( url );
            }
        }

        public static ModuleManager GetManager( string name )
        {
            return Managers[name];
        }

        public static IEnumerable < KeyValuePair < string, ModuleManager > > GetManagers()
        {
            return Managers;
        }

        public static void Initialize()
        {
            if ( ResolverSettings == null && Managers == null )
            {
                Directory.CreateDirectory( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config/module" ) );
                ResolverSettings = ModuleResolverSettings.Create();
                Managers = new Dictionary < string, ModuleManager >();

                foreach ( KeyValuePair < string, string > origin in ResolverSettings.ModuleOrigins )
                {
                    AddManager( origin.Key, origin.Value );
                }
            }
        }

        public static ProjectConfig Resolve( string name, string version = "ANY" )
        {
            return Managers.First( x => x.Value.HasPackage( name ) ).
                            Value.
                            GetPackage( name ).
                            GetInstallTarget( version == "ANY" ? null : version );
        }

        public static ProjectConfig Resolve( string repository, ProjectDependency dependency )
        {
            return Managers[repository].
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
