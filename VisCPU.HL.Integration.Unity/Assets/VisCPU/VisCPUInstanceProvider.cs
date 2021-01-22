using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VisCPU;
using VisCPU.HL.Importer;
using VisCPU.Instructions;
using VisCPU.Peripherals.Console;
using VisCPU.Peripherals.Memory;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;
using VisCPU.HL.Integration;
using Logger = VisCPU.Utility.Logging.Logger;

public class VisCpuInstanceProvider : MonoBehaviour
{
    [SerializeField]
    private LoggerSystems m_LogOutput = LoggerSystems.Default;

    [SerializeField]
    private bool m_UsePersistentPath=false;
    [SerializeField]
    private bool m_UseInGameConsole = true;
    [SerializeField]
    private bool m_PrintStackTrace = false;

    [SerializeField]
    private Text m_ConsoleOutput = null;

    [SerializeField]
    public string TempBuildOutput = null;

    [SerializeField]
    public string ConfigPath = null;

    [SerializeField]
    public string ImporterCache = null;

    [SerializeField]
    public string BuildOutput = null;

    private Cpu m_CpuInstance = null;

    #region Unity Event Functions

    private void OnDestroy()
    {
        m_CpuInstance.MemoryBus.Shutdown();
    }

    private void Start()
    {


        Logger.s_Settings.SetLogLevel( m_LogOutput );
        EventManager.RegisterDefaultHandlers();

        Logger.OnLogReceive += ( systems, s ) => Debug.Log( $"[{systems}] {s}" );

        if ( m_UseInGameConsole )
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        if ( m_UsePersistentPath )
        {
            string persistentPath = Application.persistentDataPath;
            //Path Setup
            ImporterCache = Path.GetFullPath(Path.Combine(persistentPath,ImporterCache));
            TempBuildOutput = Path.GetFullPath(Path.Combine(persistentPath,TempBuildOutput));
            ConfigPath = Path.GetFullPath(Path.Combine(persistentPath,ConfigPath));
            BuildOutput = Path.GetFullPath(Path.Combine(persistentPath, BuildOutput));
        }
        else
        {
            //Path Setup
            ImporterCache = Path.GetFullPath(ImporterCache);
            TempBuildOutput = Path.GetFullPath(TempBuildOutput);
            ConfigPath = Path.GetFullPath(ConfigPath);
            BuildOutput = Path.GetFullPath(BuildOutput);
        }

        AImporter.CacheRoot = ImporterCache;
        SettingsCategories.SetDefaultConfigDir( ConfigPath );

        CpuSettings.FallbackSet = new DefaultSet();

        Memory mem = new Memory();

        Cpu cpu = new CpuInstanceBuilder().WithPeripherals( mem ).
                                           WithExposedApi( UnityVisApi.UAPI_DestroyByName, "UNITY_DestroyByName", 2 ).
                                           WithExposedApi(
                                               UnityVisApi.UAPI_DestroyByHandle,
                                               "UNITY_DestroyByHandle",
                                               1 ).
                                           WithExposedApi( UnityVisApi.UAPI_CreateHandle, "UNITY_CreateHandle", 2 ).
                                           WithExposedApi( UnityVisApi.UAPI_SetPosition, "UNITY_SetPosition", 4 ).
                                           WithExposedApi( UnityVisApi.UAPI_AddPosition, "UNITY_AddPosition", 4 ).
                                           WithExposedApi( UnityVisApi.UAPI_SetPositionX, "UNITY_SetPositionX", 2 ).
                                           WithExposedApi( UnityVisApi.UAPI_SetPositionY, "UNITY_SetPositionY", 2 ).
                                           WithExposedApi( UnityVisApi.UAPI_SetPositionZ, "UNITY_SetPositionZ", 2 ).
                                           WithExposedApi( UnityVisApi.UAPI_GetPositionX, "UNITY_GetPositionX", 1 ).
                                           WithExposedApi( UnityVisApi.UAPI_GetPositionY, "UNITY_GetPositionY", 1 ).
                                           WithExposedApi( UnityVisApi.UAPI_GetPositionZ, "UNITY_GetPositionZ", 1 ).
                                           WithExposedApi( UnityVisApi.UAPI_AddPositionX, "UNITY_AddPositionX", 1 ).
                                           WithExposedApi( UnityVisApi.UAPI_AddPositionY, "UNITY_AddPositionY", 1 ).
                                           WithExposedApi( UnityVisApi.UAPI_AddPositionZ, "UNITY_AddPositionZ", 1 ).
                                           WithExposedApi( UnityVisApi.UAPI_Log, "UNITY_Log", 2 ).
                                           WithExposedApi( UnityVisApi.UAPI_LogError, "UNITY_LogError", 2 ).
                                           Build();

        m_CpuInstance = cpu;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        m_ConsoleOutput.text += $"\n[{type}]{condition}";

        if ( m_PrintStackTrace )
        {
            m_ConsoleOutput.text += $"\n{stackTrace}";
        }
    }

    #endregion

    #region Public

    public void Run( byte[] data )
    {
        m_CpuInstance.LoadBinary( data.ToUInt() );
        m_CpuInstance.Run();
        m_CpuInstance.UnSet( Cpu.Flags.Halt );
    }

    public void Run( string file )
    {
        Run( File.ReadAllBytes( file ) );
    }

    public IEnumerable RunAsync( string file )
    {
        return RunAsync( File.ReadAllBytes( file ) );
    }

    public IEnumerable RunAsync( byte[] data )
    {
        uint itCount = 0;
        m_CpuInstance.LoadBinary( data.ToUInt() );

        while ( !m_CpuInstance.HasSet(Cpu.Flags.Halt ) )
        {
            m_CpuInstance.Cycle();

            if ( m_CpuInstance.HasSet(Cpu.Flags.Break ) )
            {
                m_CpuInstance.UnSet(Cpu.Flags.Break );
            }

            if ( itCount > 1000 )
            {
                itCount = 0;

                yield return new WaitForEndOfFrame();
            }
            else
            {
                itCount++;
            }
        }

        m_CpuInstance.UnSet(Cpu.Flags.Halt );
    }

    #endregion
}
