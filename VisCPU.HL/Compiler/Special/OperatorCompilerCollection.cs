using System;
using System.Collections.Generic;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler
{
    public class OperatorCompilerCollection<T> : HLExpressionCompiler<T>
        where T : HLExpression
    {

        protected override bool AllImplementations => true;

        private readonly Dictionary<HLTokenType, HLExpressionCompiler<T>> OpCompilers =
            new Dictionary<HLTokenType, HLExpressionCompiler<T>>();

        public OperatorCompilerCollection(Dictionary<HLTokenType, HLExpressionCompiler<T>> opCompilers)
        {
            OpCompilers = opCompilers;
        }

        public override ExpressionTarget ParseExpression(HLCompilation compilation, T expr)
        {
            return ParseExpression(compilation, expr, new ExpressionTarget());
        }

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, T expr, ExpressionTarget outputTarget)
        {
            if (!OpCompilers.ContainsKey(expr.Type))
            {
                EventManager < ErrorEvent >.SendEvent( new ExpressionCompilerNotFoundEvent( expr ) );

                return new ExpressionTarget();
            }

            return (OpCompilers[expr.Type] as IHLExpressionCompiler).Parse(compilation, expr,outputTarget );
        }

    }
}