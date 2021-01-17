using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using VisCPU.ProjectSystem.Data;
using VisCPU.ProjectSystem.Database;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.ProjectSystem.UploadService
{

    public class TCPProjectDatabaseServer : VisBase
    {

        private readonly string m_TempStagingDirectory;
        private readonly ProjectDatabase m_Manager;

        private bool m_StopServer;
        private readonly TcpListener m_Listener;

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public TCPProjectDatabaseServer( ProjectDatabase manager, int port, string tempStagingDirectory )
        {
            m_Manager = manager;
            m_Listener = TcpListener.Create( port );
            m_TempStagingDirectory = tempStagingDirectory;
            Directory.CreateDirectory( m_TempStagingDirectory );
        }

        public string GetTempFile()
        {
            return Path.Combine( m_TempStagingDirectory, Path.GetRandomFileName() );
        }

        public void ServerLoop()
        {
            m_StopServer = false;
            m_Listener.Start();

            while ( !m_StopServer )
            {
                Task < TcpClient > clientTask = m_Listener.AcceptTcpClientAsync();

                while ( !clientTask.IsCompleted )
                {
                    if ( m_StopServer )
                    {
                        break;
                    }

                    Thread.Sleep( 500 );
                }

                if ( m_StopServer )
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

                    ProjectConfig target = ProjectConfig.Deserialize( Encoding.UTF8.GetString( mod ) );

                    client.GetStream().Read( modLen, 0, modLen.Length );
                    int dataLength = BitConverter.ToInt32( modLen, 0 );

                    Stream s = File.Create( tempFile );

                    for ( int i = 0; i < dataLength; i++ )
                    {
                        s.WriteByte( ( byte ) client.GetStream().ReadByte() );
                    }

                    s.Close();

                    m_Manager.AddPackage( target, tempFile );

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

            Directory.Delete( m_TempStagingDirectory, true );
        }

        public void Stop()
        {
            m_StopServer = true;
        }

        #endregion

    }

}
