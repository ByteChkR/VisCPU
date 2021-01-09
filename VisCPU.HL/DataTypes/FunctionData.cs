using System;

namespace VisCPU.HL.DataTypes
{

    public class FunctionData : IExternalData
    {

        //public bool HasReturnValue;
        public int ParameterCount;
        public bool Public;
        private readonly Func < string[] > functionCompiler;

        private readonly string name;
        private string[] compiledOutput;

        public ExternalDataType DataType => ExternalDataType.FUNCTION;

        #region Public

        public FunctionData(
            string name,
            bool isPublic,
            Func < string[] > funcCompiler,
            int parameterCount,
            bool hasReturnValue )
        {
            this.name = name;
            Public = isPublic;
            functionCompiler = funcCompiler;
            compiledOutput = null;
            ParameterCount = parameterCount;

            //HasReturnValue = hasReturnValue;
        }

        public string[] GetCompiledOutput()
        {
            if ( compiledOutput == null )
            {
                compiledOutput = functionCompiler();
            }

            return compiledOutput;
        }

        public string GetFinalName()
        {
            return name;
        }

        public string GetName()
        {
            return name;
        }

        #endregion

    }

}
