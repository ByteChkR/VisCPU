﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Newtonsoft.Json;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers.Events;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Modules.ModuleManagers
{

    public class LocalModuleManager : ModuleManager
    {

        private readonly List < ModulePackage > m_PackageList;

        #region Public

        public LocalModuleManager( string moduleRoot ) : base(
                                                              Parse( moduleRoot )
                                                             )
        {
            Directory.CreateDirectory( ModuleRoot.OriginalString );

            string modListPath = Path.Combine(
                                              ModuleRoot.OriginalString,
                                              s_ModuleList
                                             );

            if ( !File.Exists( modListPath ) )
            {
                File.WriteAllText( modListPath, JsonConvert.SerializeObject( new List < ModulePackage >() ) );
            }

            m_PackageList =
                JsonConvert.DeserializeObject < List < ModulePackage > >(
                                                                         File.ReadAllText( modListPath )
                                                                        );

            m_PackageList.ForEach( x => x.Manager = this );
        }

        public override void AddPackage( ProjectConfig target, string moduleDataPath )
        {
            ModulePackage package;

            if ( !HasPackage( target.ProjectName ) )
            {
                package = new ModulePackage( this, target.ProjectName, new string[0] );
                m_PackageList.Add( package );
            }
            else
            {
                package = GetPackage( target.ProjectName );
            }

            if ( package.HasTarget( target.ProjectVersion ) )
            {
                EventManager < WarningEvent >.SendEvent(
                                                        new ModuleVersionAlreadyExistsEvent(
                                                             target.ProjectName,
                                                             target.ProjectVersion
                                                            )
                                                       );

                string dataDir = GetTargetDataPath( target );

                if ( Directory.Exists( dataDir ) )
                {
                    Directory.Delete( dataDir, true );
                }
            }

            package.ModuleVersions.Add( target.ProjectVersion );
            string data = GetTargetDataPath( target );
            Directory.CreateDirectory( data );
            File.Copy( moduleDataPath, GetTargetDataUri( target ) );
            ProjectConfig.Save( GetTargetInfoUri( package, target.ProjectVersion ), target );
            SavePackageList( m_PackageList );
        }

        public override void Get( ProjectConfig target, string targetDir )
        {
            if ( Directory.Exists( targetDir ) )
            {
                Directory.Delete( targetDir, true );
            }

            string dataPath = GetTargetDataUri( target );
            ZipFile.ExtractToDirectory( dataPath, targetDir );

            foreach ( ProjectDependency targetDependency in target.Dependencies )
            {
                Get(
                    ModuleResolver.Resolve( this, targetDependency ),
                    Path.Combine( targetDir, targetDependency.ProjectName )
                   );
            }
        }

        public override string GetModulePackagePath( ModulePackage package )
        {
            return Path.Combine( ModuleRoot.OriginalString, s_ModulePath, package.ModuleName );
        }

        public override ModulePackage GetPackage( string name )
        {
            return m_PackageList.First( x => x.ModuleName == name );
        }

        public override IEnumerable < ModulePackage > GetPackages()
        {
            return m_PackageList;
        }

        public override string GetTargetDataPath( ProjectConfig target )
        {
            return Path.Combine( ModuleRoot.OriginalString, s_ModulePath, target.ProjectName, target.ProjectVersion );
        }

        public override string GetTargetDataUri( ProjectConfig target )
        {
            return Path.Combine( GetTargetDataPath( target ), s_ModuleData );
        }

        public override string GetTargetInfoUri( ModulePackage package, string moduleVersion )
        {
            return Path.Combine( GetModulePackagePath( package ), moduleVersion, s_ModuleTarget );
        }

        public override bool HasPackage( string name )
        {
            return m_PackageList.Any( x => x.ModuleName == name );
        }

        public override void RemovePackage( string moduleName )
        {
            ModulePackage p = GetPackage( moduleName );
            Directory.Delete( GetModulePackagePath( p ), true );
            m_PackageList.Remove( p );
            SavePackageList( m_PackageList );
        }

        public override void RemoveTarget( string moduleName, string moduleVersion )
        {
            ModulePackage p = GetPackage( moduleName );
            ProjectConfig t = p.GetInstallTarget( moduleVersion );
            string dataPath = GetTargetDataPath( t );

            if ( Directory.Exists( dataPath ) )
            {
                Directory.Delete( dataPath, true );
            }

            p.ModuleVersions.RemoveAll( x => x == moduleVersion );
            SavePackageList( m_PackageList );
        }

        public override void Restore( ProjectConfig target, string rootDir )
        {
            foreach ( ProjectDependency targetDependency in target.Dependencies )
            {
                Get(
                    ModuleResolver.Resolve( this, targetDependency ),
                    Path.Combine( rootDir, targetDependency.ProjectName )
                   );
            }
        }

        #endregion

        #region Private

        private static string Parse( string moduleRoot )
        {
            if ( Uri.TryCreate( moduleRoot, UriKind.Absolute, out Uri u ) )
            {
                return moduleRoot;
            }

            return Path.Combine(
                                AppDomain.CurrentDomain.BaseDirectory,
                                moduleRoot
                               );
        }

        #endregion

    }

}
