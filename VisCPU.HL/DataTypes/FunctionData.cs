using System;

namespace VisCPU.HL.DataTypes
{

    public class FunctionData : IExternalData
    {

        public int ParameterCount { get; }

        public bool Public { get; }

        private readonly Func < string[] > m_FunctionCompiler;

        private readonly string m_Name;
        private string[] m_CompiledOutput;

        public ExternalDataType DataType => ExternalDataType.Function;

        #region Public

        public FunctionData(
            string name,
            bool isPublic,
            Func < string[] > funcCompiler,
            int parameterCount,
            bool hasReturnValue )
        {
            m_Name = name;
            Public = isPublic;
            m_FunctionCompiler = funcCompiler;
            m_CompiledOutput = null;
            ParameterCount = parameterCount;
        }

        public string[] GetCompiledOutput()
        {
            if ( m_CompiledOutput == null )
            {
                m_CompiledOutput = m_FunctionCompiler();
            }

            return m_CompiledOutput;
        }

        public string GetFinalName()
        {
            return m_Name;
        }

        public string GetName()
        {
            return m_Name;
        }

        #endregion

    }

}
