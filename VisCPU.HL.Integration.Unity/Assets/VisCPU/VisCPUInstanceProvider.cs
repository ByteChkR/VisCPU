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

public class VisCPUInstanceProvider : MonoBehaviour
{
    [SerializeField]
    private LoggerSystems m_LogOutput = LoggerSystems.Default;

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

    private CPU m_CpuInstance = null;

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

        //Path Setup
        ImporterCache = Path.GetFullPath( ImporterCache );
        TempBuildOutput = Path.GetFullPath( TempBuildOutput );
        ConfigPath = Path.GetFullPath( ConfigPath );
        BuildOutput = Path.GetFullPath( BuildOutput );

        AImporter.CacheRoot = ImporterCache;
        SettingsCategories.SetDefaultConfigDir( ConfigPath );

        CPUSettings.FallbackSet = new DefaultSet();

        Memory mem = new Memory();

        ConsoleOutInterface cout = new ConsoleOutInterface();
        cout.ConsoleClear = () => m_ConsoleOutput.text = "";
        cout.WriteConsoleNum = chr => m_ConsoleOutput.text = m_ConsoleOutput.text + chr;
        cout.WriteConsoleChar = chr => m_ConsoleOutput.text = m_ConsoleOutput.text + chr;

        CPU cpu = new CPUInstanceBuilder().WithPeripherals( mem, cout ).
                                           WithExposedAPI( UnityVisAPI.UAPI_DestroyByName, "UNITY_DestroyByName", 2 ).
                                           WithExposedAPI(
                                               UnityVisAPI.UAPI_DestroyByHandle,
                                               "UNITY_DestroyByHandle",
                                               1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_CreateHandle, "UNITY_CreateHandle", 2 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_SetPosition, "UNITY_SetPosition", 4 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_AddPosition, "UNITY_AddPosition", 4 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_SetPositionX, "UNITY_SetPositionX", 2 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_SetPositionY, "UNITY_SetPositionY", 2 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_SetPositionZ, "UNITY_SetPositionZ", 2 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_GetPositionX, "UNITY_GetPositionX", 1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_GetPositionY, "UNITY_GetPositionY", 1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_GetPositionZ, "UNITY_GetPositionZ", 1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_AddPositionX, "UNITY_AddPositionX", 1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_AddPositionY, "UNITY_AddPositionY", 1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_AddPositionZ, "UNITY_AddPositionZ", 1 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_Log, "UNITY_Log", 2 ).
                                           WithExposedAPI( UnityVisAPI.UAPI_LogError, "UNITY_LogError", 2 ).
                                           Build();

        m_CpuInstance = cpu;
    }

    #endregion

    #region Public

    public void Run( byte[] data )
    {
        m_CpuInstance.LoadBinary( data.ToUInt() );
        m_CpuInstance.Run();
        m_CpuInstance.UnSet( CPU.Flags.Halt );
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

        while ( !m_CpuInstance.HasSet( CPU.Flags.Halt ) )
        {
            m_CpuInstance.Cycle();

            if ( m_CpuInstance.HasSet( CPU.Flags.Break ) )
            {
                m_CpuInstance.UnSet( CPU.Flags.Break );
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

        m_CpuInstance.UnSet( CPU.Flags.Halt );
    }

    #endregion
}
