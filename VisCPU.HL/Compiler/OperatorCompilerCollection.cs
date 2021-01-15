using System.Collections.Generic;

using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler
{

    public class OperatorCompilerCollection < T > : HLExpressionCompiler < T >
        where T : HLExpression
    {

        private readonly Dictionary < HLTokenType, HLExpressionCompiler < T > > m_OpCompilers =
            new Dictionary < HLTokenType, HLExpressionCompiler < T > >();

        protected override bool AllImplementations => true;

        #region Public

        public OperatorCompilerCollection( Dictionary < HLTokenType, HLExpressionCompiler < T > > opCompilers )
        {
            m_OpCompilers = opCompilers;
        }

        public override ExpressionTarget ParseExpression( HLCompilation compilation, T expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            T expr,
            ExpressionTarget outputTarget )
        {
            if ( !m_OpCompilers.ContainsKey( expr.Type ) )
            {
                EventManager < ErrorEvent >.SendEvent( new ExpressionCompilerNotFoundEvent( expr ) );

                return new ExpressionTarget();
            }

            return ( m_OpCompilers[expr.Type] as IHlExpressionCompiler ).Parse( compilation, expr, outputTarget );
        }

        #endregion

    }

}
