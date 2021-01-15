using System;
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

        private readonly string m_RepoName;
        private readonly string m_LocalTempCache;
        private List < ModulePackage > m_PList;

        private List < ModulePackage > PackageList => m_PList ?? ( m_PList = GetPackageList() );

        #region Public

        public HttpModuleManager( string repoName, string moduleRoot ) : base( moduleRoot )
        {
            m_RepoName = repoName;
            m_LocalTempCache = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "module", m_RepoName );
            Directory.CreateDirectory( m_LocalTempCache );
        }

        public override void Get( ProjectConfig target, string targetDir )
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
            return Path.Combine( m_LocalTempCache, s_ModulePath, package.ModuleName );
        }

        public override ModulePackage GetPackage( string name )
        {
            return PackageList.First( x => x.ModuleName == name );
        }

        public override IEnumerable < ModulePackage > GetPackages()
        {
            return PackageList;
        }

        public override string GetTargetDataPath( ProjectConfig target )
        {
            return GetTargetDataPath( m_LocalTempCache, target.ProjectName, target.ProjectVersion );
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
            return PackageList.Any( x => x.ModuleName == name );
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

        private void FetchData( ProjectConfig target )
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
            string localIndex = GetPackageListPath( m_LocalTempCache );

            if ( File.Exists( localIndex ) )
            {
                File.Delete( localIndex );
            }

            using ( WebClient wc = new WebClient() )
            {
                Log( "Downloading Index: {0}", repoIndex );
                wc.DownloadFile( new Uri( repoIndex ), localIndex );
            }

            List < ModulePackage > packages = LoadPackageList( m_LocalTempCache );
            SaveInfo( packages );

            return packages;
        }

        private string GetRemoteModulePackagePath( ModulePackage package )
        {
            return Path.Combine( ModuleRoot.OriginalString, s_ModulePath, package.ModuleName );
        }

        private string GetRemoteModulePackagePath( ProjectConfig package )
        {
            return Path.Combine( ModuleRoot.OriginalString, s_ModulePath, package.ProjectName );
        }

        private string GetRemoteTargetDataPath( ProjectConfig target )
        {
            return Path.Combine( ModuleRoot.OriginalString, s_ModulePath, target.ProjectName, target.ProjectVersion );
        }

        private string GetRemoteTargetDataUri( ProjectConfig target )
        {
            return Path.Combine( GetRemoteTargetDataPath( target ), s_ModuleData );
        }

        private string GetRemoteTargetInfoUri( ModulePackage package, string moduleVersion )
        {
            return Path.Combine( GetRemoteModulePackagePath( package ), moduleVersion, s_ModuleTarget );
        }

        private string GetRemoteTargetInfoUri( ProjectConfig target )
        {
            return Path.Combine( GetRemoteModulePackagePath( target ), target.ProjectVersion, s_ModuleTarget );
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
                                                   m_LocalTempCache,
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
