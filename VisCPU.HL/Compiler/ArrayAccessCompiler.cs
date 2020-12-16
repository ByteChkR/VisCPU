using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler
{

    public class ArrayAccessCompiler : HLExpressionCompiler < HLArrayAccessorOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLArrayAccessorOp expr,
            ExpressionTarget outputTarget )
        {
            string tmpPtrName = compilation.GetTempVar();
            ExpressionTarget tempPtr = new ExpressionTarget( tmpPtrName, true, true );
            ExpressionTarget tempPtrVar = compilation.Parse( expr.Left );

            string pnName = compilation.GetTempVar();
            ExpressionTarget pn = compilation.Parse( expr.ParameterList[0], new ExpressionTarget( pnName, true ) );

            if ( tempPtrVar.IsArray )
            {
                compilation.ProgramCode.Add( $"LOAD {tempPtr.ResultAddress} {tempPtrVar.ResultAddress}" );
            }
            else
            {
                compilation.ProgramCode.Add( $"COPY {tempPtrVar.ResultAddress} {tempPtr.ResultAddress}" );
            }

            compilation.ProgramCode.Add( $"ADD {tempPtr.ResultAddress} {pn.ResultAddress} ; Apply offset" );

            if ( outputTarget.ResultAddress != null )
            {
                compilation.ProgramCode.Add(
                                            $"DREF {tempPtr.ResultAddress} {outputTarget.ResultAddress} ; Dereference Array Pointer"
                                           );

                compilation.ReleaseTempVar( tmpPtrName );

                return outputTarget;
            }

            return tempPtr;
        }

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLArrayAccessorOp expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        #endregion

    }

}
