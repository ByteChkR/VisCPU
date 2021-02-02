﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Atomic
{

    public class DecrementExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlUnaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            string instrKey =
                target.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName
                    ? "DEC.F"
                    : "DEC";

            compilation.EmitterResult.Emit(
                instrKey,
                target.ResultAddress
            );

            return target;
        }

        #endregion
    }

}
