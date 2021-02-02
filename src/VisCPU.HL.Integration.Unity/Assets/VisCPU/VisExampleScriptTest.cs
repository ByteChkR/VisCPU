using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using VisCPU.HL.Integration;
using VisCPU.Utility;

public class VisExampleScriptTest : MonoBehaviour
{
    private static readonly string s_DefaultScript =
        @"https://raw.githubusercontent.com/ByteChkR/VisCPU/master/VisCPU.HL.Integration.Unity/Examples/MovePanel.vhl";

    [SerializeField]
    private VisCpuInstanceProvider m_Provider;

    [SerializeField]
    private bool m_Async = true;

    [SerializeField]
    private Text m_SourceFileText;

    private byte[] m_LastCompile;

    private void Start()
    {
        m_SourceFileText.text = s_DefaultScript;
    }

    #region Public

    public void Compile()
    {
        Debug.Log( "Resolving File: " + m_SourceFileText.text );
        string target = m_SourceFileText.text;
        if (m_SourceFileText.text.StartsWith("http://") || m_SourceFileText.text.StartsWith("https://"))
        {
            string srcPath = Path.Combine(Application.temporaryCachePath, Path.GetFileName(m_SourceFileText.text));

            Debug.Log("Downloading File: " + m_SourceFileText.text);
            Debug.Log("Target: " + srcPath);
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(m_SourceFileText.text, srcPath);
            }

            target = srcPath;

        }

        m_Provider.ClearUpdateAddrList();

        string file = CompilerHelper.Compile(
            target,
            Path.Combine(AppRootHelper.AppRoot, "build"),
            Path.Combine(AppRootHelper.AppRoot, "temp"),
            true,
            new[] { "HL-expr", "bin" });

        m_LastCompile = File.ReadAllBytes(file);
    }

    public void Run()
    {
        if (m_LastCompile == null)
        {
            Debug.LogError("Script not compiled. Can not run uncompiled script.");
        }

        if ( m_Async )
        {
            StartCoroutine( m_Provider.RunAsync( m_LastCompile ).GetEnumerator() );
        }
        else
        {
            m_Provider.Run( m_LastCompile );
        }
    }

    #endregion
}
