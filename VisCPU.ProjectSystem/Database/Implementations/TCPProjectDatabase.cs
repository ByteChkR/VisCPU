using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using VisCPU.ProjectSystem.Data;

namespace VisCPU.ProjectSystem.Database.Implementations
{

    public class TCPProjectDatabase : ProjectDatabase
    {
        #region Public

        public TCPProjectDatabase( string moduleRoot ) : base( moduleRoot )
        {
        }

        public override void AddPackage( ProjectConfig target, string moduleDataPath )
        {
            TcpClient client = new TcpClient( ModuleRoot.Host, ModuleRoot.Port );

            byte[] mod = Encoding.UTF8.GetBytes( JsonConvert.SerializeObject( target ) );
            client.GetStream().Write( BitConverter.GetBytes( mod.Length ), 0, sizeof( int ) );
            client.GetStream().Write( mod, 0, mod.Length );
            FileInfo info = new FileInfo( moduleDataPath );
            client.GetStream().Write( BitConverter.GetBytes( ( int ) info.Length ), 0, sizeof( int ) );
            Stream fs = info.OpenRead();
            fs.CopyTo( client.GetStream() );
            fs.Close();
            byte[] response = new byte[sizeof( int )];
            client.GetStream().Read( response, 0, response.Length );
            int responseLen = BitConverter.ToInt32( response, 0 );
            response = new byte[responseLen];
            client.GetStream().Read( response, 0, response.Length );
            client.Close();
            Log( "Response: {0}", Encoding.UTF8.GetString( response ) );
        }

        public override void Get( ProjectConfig target, string targetDir )
        {
        }

        public override string GetModulePackagePath( ProjectPackage package )
        {
            return null;
        }

        public override ProjectPackage GetPackage( string name )
        {
            return null;
        }

        public override IEnumerable < ProjectPackage > GetPackages()
        {
            yield break;
        }

        public override string GetTargetDataPath( ProjectConfig target )
        {
            return null;
        }

        public override string GetTargetDataUri( ProjectConfig target )
        {
            return null;
        }

        public override string GetTargetInfoUri( ProjectPackage package, string moduleVersion )
        {
            return null;
        }

        public override bool HasPackage( string name )
        {
            return false;
        }

        public override void Restore( ProjectConfig target, string rootDir )
        {
        }

        #endregion
    }

}
