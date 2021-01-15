using System;
using System.Collections.Generic;
using System.IO;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.HL.Modules.Resolvers
{

    public static class ModuleResolver
    {

        public static ModuleResolverSettings ResolverSettings;

        private static Dictionary < string, ModuleManager > Managers;

        #region Public

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
                    if ( Uri.TryCreate( origin.Value, UriKind.Absolute, out Uri u ) )
                    {
                        if ( u.Scheme == "http" || u.Scheme == "https" )
                        {
                            Managers.Add( origin.Key, new HttpModuleManager( origin.Key, u.OriginalString ) );
                        }
                        else if ( u.Scheme == "dev" )
                        {
                            Managers.Add( origin.Key, new TCPUploadModuleManager( u.OriginalString ) );
                        }
                        else
                        {
                            Managers.Add( origin.Key, new LocalModuleManager( origin.Value ) );
                        }
                    }
                    else
                    {
                        Managers.Add( origin.Key, new LocalModuleManager( origin.Value ) );
                    }
                }
            }
        }

        public static ProjectInfo Resolve( string repository, ProjectDependency dependency )
        {
            return Managers[repository].
                   GetPackage( dependency.ProjectName ).
                   GetInstallTarget( dependency.ProjectVersion == "ANY" ? null : dependency.ProjectVersion );
        }

        public static ProjectInfo Resolve( ModuleManager manager, ProjectDependency dependency )
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

        //public static ModuleManager Manager;

    }

}
