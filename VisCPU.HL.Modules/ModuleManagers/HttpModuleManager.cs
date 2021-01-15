﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.Resolvers;

namespace VisCPU.HL.Modules.ModuleManagers
{

    public class HttpModuleManager : ModuleManager
    {

        private readonly string RepoName;
        private readonly string LocalTempCache;
        private List < ModulePackage > pList;

        private List < ModulePackage > PackageList => pList ?? ( pList = GetPackageList() );

        #region Public

        public HttpModuleManager( string repoName, string moduleRoot ) : base( moduleRoot )
        {
            RepoName = repoName;
            LocalTempCache = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "module", RepoName );
            Directory.CreateDirectory( LocalTempCache );
        }

        public override void Get( ProjectInfo target, string targetDir )
        {
            if ( Directory.Exists( targetDir ) )
            {
                Directory.Delete( targetDir, true );
            }

            string dataPath = GetTargetDataUri( target );

            if ( !File.Exists( dataPath ) )
            {
                FetchData( target );
            }

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
            return Path.Combine( LocalTempCache, MODULE_PATH, package.ModuleName );
        }

        public override ModulePackage GetPackage( string name )
        {
            return PackageList.First( x => x.ModuleName == name );
        }

        public override IEnumerable < ModulePackage > GetPackages()
        {
            return PackageList;
        }

        public override string GetTargetDataPath( ProjectInfo target )
        {
            return GetTargetDataPath( LocalTempCache, target.ProjectName, target.ProjectVersion );
        }

        public override string GetTargetDataUri( ProjectInfo target )
        {
            return Path.Combine( GetTargetDataPath( target ), MODULE_DATA );
        }

        public override string GetTargetInfoUri( ModulePackage package, string moduleVersion )
        {
            return Path.Combine( GetModulePackagePath( package ), moduleVersion, MODULE_TARGET );
        }

        public override bool HasPackage( string name )
        {
            return PackageList.Any( x => x.ModuleName == name );
        }

        public override void Restore( ProjectInfo target, string rootDir )
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

        private void FetchData( ProjectInfo target )
        {
            string dataUri = GetRemoteTargetDataUri( target );
            string dataLocal = GetTargetDataUri( target );

            if ( File.Exists( dataLocal ) )
            {
                return;
            }

            Directory.CreateDirectory( GetTargetDataPath( target ) );

            using ( WebClient wc = new WebClient() )
            {
                Log( "Downloading Module: {0}", dataUri );
                wc.DownloadFile( new Uri( dataUri ), dataLocal );
            }
        }

        private List < ModulePackage > GetPackageList()
        {
            string repoIndex = GetPackageListPath();
            string localIndex = GetPackageListPath( LocalTempCache );

            if ( File.Exists( localIndex ) )
            {
                File.Delete( localIndex );
            }

            using ( WebClient wc = new WebClient() )
            {
                Log( "Downloading Index: {0}", repoIndex );
                wc.DownloadFile( new Uri( repoIndex ), localIndex );
            }

            List < ModulePackage > packages = LoadPackageList( LocalTempCache );
            SaveInfo( packages );

            return packages;
        }

        private string GetRemoteModulePackagePath( ModulePackage package )
        {
            return Path.Combine( ModuleRoot.OriginalString, MODULE_PATH, package.ModuleName );
        }

        private string GetRemoteModulePackagePath( ProjectInfo package )
        {
            return Path.Combine( ModuleRoot.OriginalString, MODULE_PATH, package.ProjectName );
        }

        private string GetRemoteTargetDataPath( ProjectInfo target )
        {
            return Path.Combine( ModuleRoot.OriginalString, MODULE_PATH, target.ProjectName, target.ProjectVersion );
        }

        private string GetRemoteTargetDataUri( ProjectInfo target )
        {
            return Path.Combine( GetRemoteTargetDataPath( target ), MODULE_DATA );
        }

        private string GetRemoteTargetInfoUri( ModulePackage package, string moduleVersion )
        {
            return Path.Combine( GetRemoteModulePackagePath( package ), moduleVersion, MODULE_TARGET );
        }

        private string GetRemoteTargetInfoUri( ProjectInfo target )
        {
            return Path.Combine( GetRemoteModulePackagePath( target ), target.ProjectVersion, MODULE_TARGET );
        }

        private void SaveInfo( List < ModulePackage > packages )
        {
            foreach ( ModulePackage modulePackage in packages )
            {
                foreach ( string modulePackageModuleVersion in modulePackage.ModuleVersions )
                {
                    string infoLocal = GetTargetInfoUri( modulePackage, modulePackageModuleVersion );
                    string infoRemote = GetRemoteTargetInfoUri( modulePackage, modulePackageModuleVersion );

                    if ( File.Exists( infoLocal ) )
                    {
                        continue;
                    }

                    string dir = GetTargetDataPath(
                                                   LocalTempCache,
                                                   modulePackage.ModuleName,
                                                   modulePackageModuleVersion
                                                  );

                    Directory.CreateDirectory(
                                              dir
                                             );

                    using ( WebClient wc = new WebClient() )
                    {
                        Log( "Downloading Info: {0}", infoRemote );
                        wc.DownloadFile( new Uri( infoRemote ), infoLocal );
                    }
                }
            }
        }

        #endregion

    }

}