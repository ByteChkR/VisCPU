using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Modules.UploadService
{

    public class TCPModuleManagerServer : VisBase
    {

        public readonly string TempStagingDirectory;
        public readonly ModuleManager Manager;

        private bool stopServer = false;
        private TcpListener Listener;

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public TCPModuleManagerServer( ModuleManager manager, int port, string tempStagingDirectory )
        {
            Manager = manager;
            Listener = TcpListener.Create( port );
            TempStagingDirectory = tempStagingDirectory;
            Directory.CreateDirectory( TempStagingDirectory );
        }

        public string GetTempFile()
        {
            return Path.Combine( TempStagingDirectory, Path.GetRandomFileName() );
        }

        public void ServerLoop()
        {
            stopServer = false;
            Listener.Start();

            while ( !stopServer )
            {
                Task < TcpClient > clientTask = Listener.AcceptTcpClientAsync();

                while ( !clientTask.IsCompleted )
                {
                    if ( stopServer )
                    {
                        break;
                    }

                    Thread.Sleep( 500 );
                }

                if ( stopServer )
                {
                    break;
                }

                if ( clientTask.IsFaulted || clientTask.IsCanceled )
                {
                    continue;
                }

                TcpClient client = clientTask.Result;

                string tempFile = GetTempFile();

                try
                {
                    byte[] modLen = new byte[sizeof( int )];
                    client.GetStream().Read( modLen, 0, modLen.Length );
                    int moduleTargetLength = BitConverter.ToInt32( modLen, 0 );
                    byte[] mod = new byte[moduleTargetLength];
                    client.GetStream().Read( mod, 0, mod.Length );

                    ProjectInfo target = ModuleManager.LoadModuleTarget( Encoding.UTF8.GetString( mod ) );

                    client.GetStream().Read( modLen, 0, modLen.Length );
                    int dataLength = BitConverter.ToInt32( modLen, 0 );

                    Stream s = File.Create( tempFile );

                    for ( int i = 0; i < dataLength; i++ )
                    {
                        s.WriteByte( ( byte ) client.GetStream().ReadByte() );
                    }

                    s.Close();

                    Manager.AddPackage( target, tempFile );

                    File.Delete( tempFile );

                    byte[] response = Encoding.UTF8.GetBytes( "Success." );
                    client.GetStream().Write( BitConverter.GetBytes( response.Length ), 0, sizeof( int ) );
                    client.GetStream().Write( response, 0, response.Length );
                }
                catch ( Exception e )
                {
                    if ( File.Exists( tempFile ) )
                    {
                        File.Delete( tempFile );
                    }

                    byte[] response = Encoding.UTF8.GetBytes( e.Message );
                    client.GetStream().Write( BitConverter.GetBytes( response.Length ), 0, sizeof( int ) );
                    client.GetStream().Write( response, 0, response.Length );
                }

                client.Close();
            }

            Directory.Delete( TempStagingDirectory, true );
        }

        public void Stop()
        {
            stopServer = true;
        }

        #endregion

    }

}
