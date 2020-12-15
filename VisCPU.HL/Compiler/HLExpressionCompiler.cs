using System;

using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.Compiler
{
    public abstract class HLExpressionCompiler<T> : VisBase, IHLExpressionCompiler
        where T : HLExpression
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Compiler;

        protected virtual bool NeedsOutput { get; }
        protected virtual bool AllImplementations { get; }

        ExpressionTarget IHLExpressionCompiler.Parse(
            HLCompilation compilation, HLExpression expr, ExpressionTarget outputTarget)
        {
            return InnerParseExpression(compilation, (T) expr, outputTarget);
        }

        private ExpressionTarget InnerParseExpression(HLCompilation compilation, T expr, ExpressionTarget outputTarget)
        {
            if (NeedsOutput)
            {
                ExpressionTarget target = outputTarget;
                if (outputTarget.ResultAddress == null || !outputTarget.IsAddress)target=new ExpressionTarget(compilation.GetTempVar(), true);
                return ParseExpression(compilation, expr, target);
            }

            if (AllImplementations && outputTarget.ResultAddress != null)
                return ParseExpression(compilation, expr, outputTarget);
            return ParseExpression(compilation, expr);
        }

        public virtual ExpressionTarget ParseExpression(
            HLCompilation compilation, T expr, ExpressionTarget outputTarget)
        {
            throw new NotImplementedException();
        }

        public virtual ExpressionTarget ParseExpression(
            HLCompilation compilation, T expr)
        {
            throw new NotImplementedException();
        }

    }
}