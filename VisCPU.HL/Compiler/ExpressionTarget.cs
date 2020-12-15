namespace VisCPU.HL.Compiler
{
    public readonly struct ExpressionTarget
    {

        public readonly string ResultAddress;
        public readonly bool IsAddress;
        public readonly bool IsArray;

        public ExpressionTarget(string resultAddress, bool isAddress, bool isArray = false)
        {
            ResultAddress = resultAddress;
            IsAddress = isAddress;
            IsArray = isArray;
        }

        public ExpressionTarget MakeAddress(HLCompilation c)
        {
            if (IsAddress)
            {
                return this;
            }

            ExpressionTarget tmpVal = new ExpressionTarget(c.GetTempVar(), true, IsArray);
            c.ProgramCode.Add($"LOAD {tmpVal.ResultAddress} {ResultAddress}");
            return tmpVal;
        }

        public ExpressionTarget LoadIfNotNull(HLCompilation compilation, ExpressionTarget target)
        {
            if (target.ResultAddress == null) return this;
            compilation.ProgramCode.Add($"LOAD {target.ResultAddress} {ResultAddress}");
            return target;
        }
        public ExpressionTarget CopyIfNotNull(HLCompilation compilation, ExpressionTarget target)
        {
            if (target.ResultAddress == null) return this;
            compilation.ProgramCode.Add($"COPY {ResultAddress} {target.ResultAddress}");
            return target;
        }
    }
}