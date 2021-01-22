using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using VisCPU.HL.Integration;

public class VisExampleScriptTest : MonoBehaviour
{

    [SerializeField]
    private VisCpuInstanceProvider m_Provider = null;

    [SerializeField]
    private Text m_SourceFileText = null;

    private byte[] m_LastCompile = null;

    #region Public

    public void Compile()
    {
        if (m_SourceFileText.text.StartsWith("http://") || m_SourceFileText.text.StartsWith("https://"))
        {
            string srcPath = Path.Combine(Application.temporaryCachePath, Path.GetFileName(m_SourceFileText.text));

            Debug.Log("Downloading File: " + m_SourceFileText.text);
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(m_SourceFileText.text, srcPath);
            }

            m_SourceFileText.text = srcPath;
        }
        string file = CompilerHelper.Compile(
            m_SourceFileText.text,
            m_Provider.BuildOutput,
            m_Provider.TempBuildOutput,
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
        StartCoroutine(m_Provider.RunAsync(m_LastCompile).GetEnumerator());
    }

    #endregion
}
