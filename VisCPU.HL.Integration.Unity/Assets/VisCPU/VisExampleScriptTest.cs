using System.IO;
using UnityEngine;
using VisCPU.HL.Integration;

public class VisExampleScriptTest : MonoBehaviour
{
    [SerializeField]
    private VisCPUInstanceProvider m_Provider = null;

    [SerializeField]
    private string m_SourceFile = "./Assets/VisCPU/Examples/HelloWorld.vhl";
    private byte[] m_LastCompile = null;

    #region Public

    public void Compile()
    {
        string file = CompilerHelper.Compile(
            m_SourceFile,
            m_Provider.BuildOutput,
            m_Provider.TempBuildOutput,
            true,
            new[] { "HL-expr", "bin" } );

        m_LastCompile = File.ReadAllBytes( file );
    }

    public void Run()
    {
        if ( m_LastCompile == null )
        {
            Debug.LogError( "Script not compiled. Can not run uncompiled script." );
        }
        StartCoroutine( m_Provider.RunAsync( m_LastCompile ).GetEnumerator() );
    }

    #endregion
}
