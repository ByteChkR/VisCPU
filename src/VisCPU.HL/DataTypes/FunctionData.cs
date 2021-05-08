using System;

namespace VisCPU.HL.DataTypes
{

    public class FunctionData : IExternalData
    {
        private readonly Func < string[] > m_FunctionCompiler;

        private readonly string m_Name;
        private string[] m_CompiledOutput;

        public int ParameterCount { get; }

        public string ReturnType { get; }

        public bool Public { get; }

        public bool Static { get; }

        public int UseCount { get; private set; } = 0;

        public ExternalDataType DataType => ExternalDataType.Function;

        #region Public

        public FunctionData(
            string name,
            bool isPublic,
            bool isStatic,
            Func < string[] > funcCompiler,
            int parameterCount,
            string returnType )
        {
            m_Name = name;
            Public = isPublic;
            Static = isStatic;
            m_FunctionCompiler = funcCompiler;
            m_CompiledOutput = null;
            ParameterCount = parameterCount;
            ReturnType = returnType;
        }

        public string[] GetCompiledOutput()
        {
            if ( m_CompiledOutput == null )
            {
                if ( m_FunctionCompiler == null )
                {
                    m_CompiledOutput = new string[0];

                    return m_CompiledOutput;
                }

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

        public void SetUsed()
        {
            UseCount++;
        }

        #endregion
    }

}
