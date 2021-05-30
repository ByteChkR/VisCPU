using System;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Atomic
{

    public class UnaryMinusExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlUnaryOp expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlUnaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget targetVal = compilation.Parse(
                                                        expr.Left
                                                       );
            ExpressionTarget target = targetVal.MakeAddress( compilation );

            if ( target.TypeDefinition.Name != HLBaseTypeNames.s_FloatTypeName )
            {
                EventManager < WarningEvent >.SendEvent(
                                                        new UnaryMinusExpressionInvalidEvent(
                                                             "Unary inversion is only possible with signed types and floats, performing this instruction on other types may yield undefined results."
                                                            )
                                                       );
            }

            if ( outputTarget.ResultAddress == target.ResultAddress )
            {
                compilation.EmitterResult.Emit( "INV.F", outputTarget.ResultAddress );

                return outputTarget;
            }
            else
            {
                compilation.EmitterResult.Emit( "INV.F", target.ResultAddress, outputTarget.ResultAddress );
                ExpressionTarget ret = target.CopyIfNotNull( compilation, outputTarget );
                compilation.ReleaseTempVar( target.ResultAddress );
                return ret;
            }
        }

        #endregion

    }

}
