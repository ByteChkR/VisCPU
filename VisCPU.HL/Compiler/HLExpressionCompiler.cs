using System;

using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler
{

    public abstract class HLExpressionCompiler < T > : VisBase, IHLExpressionCompiler
        where T : HLExpression
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Compiler;

        protected virtual bool NeedsOutput { get; }

        protected virtual bool AllImplementations { get; }

        #region Public

        public virtual ExpressionTarget ParseExpression(
            HLCompilation compilation,
            T expr,
            ExpressionTarget outputTarget )
        {
            throw new NotImplementedException();
        }

        public virtual ExpressionTarget ParseExpression( HLCompilation compilation, T expr )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        private ExpressionTarget InnerParseExpression(
            HLCompilation compilation,
            T expr,
            ExpressionTarget outputTarget )
        {
            if ( NeedsOutput )
            {
                ExpressionTarget target = outputTarget;

                if ( outputTarget.ResultAddress == null || !outputTarget.IsAddress )
                {
                    target = new ExpressionTarget(
                                                  compilation.GetTempVar(0),
                                                  true,
                                                  compilation.TypeSystem.GetType( "var" )
                                                 );
                }

                return ParseExpression( compilation, expr, target );
            }

            if ( AllImplementations && outputTarget.ResultAddress != null )
            {
                return ParseExpression( compilation, expr, outputTarget );
            }

            return ParseExpression( compilation, expr );
        }

        ExpressionTarget IHLExpressionCompiler.Parse(
            HLCompilation compilation,
            HLExpression expr,
            ExpressionTarget outputTarget )
        {
            return InnerParseExpression( compilation, ( T ) expr, outputTarget );
        }

        #endregion

    }

}
