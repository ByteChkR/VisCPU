using System;

namespace VisCPU.HL
{
    public class FunctionData : IExternalData
    {

        private readonly Func<string[]> functionCompiler;
        private string[] compiledOutput;
        public bool HasReturnValue;
        public int ParameterCount;
        public bool Public;

        public FunctionData(
            string name, bool isPublic, Func<string[]> funcCompiler, int parameterCount, bool hasReturnValue)
        {
            this.name = name;
            Public = isPublic;
            functionCompiler = funcCompiler;
            compiledOutput = null;
            ParameterCount = parameterCount;
            HasReturnValue = hasReturnValue;
        }

        public ExternalDataType DataType => ExternalDataType.FUNCTION;

        public string GetName()
        {
            return name;
        }

        public string GetFinalName()
        {
            return name;
        }

        private string name;

        public string[] GetCompiledOutput()
        {
            if (compiledOutput == null)
            {
                compiledOutput = functionCompiler();
            }

            return compiledOutput;
        }

    }
}