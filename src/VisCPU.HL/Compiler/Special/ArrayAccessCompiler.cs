using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.IO.Settings;

namespace VisCPU.HL.Compiler.Special
{

    public class ArrayAccessCompiler : HlExpressionCompiler < HlArrayAccessorOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlArrayAccessorOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget tempPtrVar = compilation.Parse( expr.Left );

            ExpressionTarget tempPtr = new ExpressionTarget(
                                                            SettingsManager.GetSettings<HlCompilerSettings>().OmitTempVarInit
                                                                ? compilation.GetTempVar()
                                                                : compilation.GetTempVar(0),
                                                            true,
                                                            tempPtrVar.TypeDefinition,
                                                            true
                                                           );

            ExpressionTarget pnVal = compilation.Parse(
                                                       expr.ParameterList[0]
                                                      );
            ExpressionTarget pn = pnVal.MakeAddress( compilation );

            if ( tempPtrVar.TypeDefinition is ArrayTypeDefintion adef )
            {
                string tmpSName = compilation.GetTempVar( adef.ElementType.GetSize() );
                compilation.EmitterResult.Emit( $"MUL", pn.ResultAddress, tmpSName );
                compilation.ReleaseTempVar( tmpSName );
                compilation.EmitterResult.Emit( $"LOAD", tempPtr.ResultAddress, tempPtrVar.ResultAddress );
            }
            else
            {
                string tmpSName = compilation.GetTempVar( tempPtrVar.TypeDefinition.GetSize() );
                compilation.EmitterResult.Emit( $"MUL", pn.ResultAddress, tmpSName );
                compilation.ReleaseTempVar( tmpSName );
                compilation.EmitterResult.Emit( $"COPY", tempPtrVar.ResultAddress, tempPtr.ResultAddress );
            }

            compilation.EmitterResult.Emit( $"ADD", tempPtr.ResultAddress, pn.ResultAddress );

            if ( outputTarget.ResultAddress != null )
            {
                compilation.EmitterResult.Emit(
                                               $"DREF",
                                               tempPtr.ResultAddress,
                                               outputTarget.ResultAddress
                                              );

                compilation.ReleaseTempVar( tempPtr.ResultAddress );
                compilation.ReleaseTempVar( tempPtrVar.ResultAddress );
                compilation.ReleaseTempVar(pn.ResultAddress);
                compilation.ReleaseTempVar(pnVal.ResultAddress);

                return outputTarget;
            }

            compilation.ReleaseTempVar( tempPtrVar.ResultAddress );
            compilation.ReleaseTempVar( pn.ResultAddress );
            compilation.ReleaseTempVar(pnVal.ResultAddress);

            return tempPtr;
        }

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlArrayAccessorOp expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        #endregion

    }

}
