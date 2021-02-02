using System.Collections.Generic;

using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler
{

    public class OperatorCompilerCollection < T > : HlExpressionCompiler < T >
        where T : HlExpression
    {

        private readonly Dictionary < HlTokenType, HlExpressionCompiler < T > > m_OpCompilers =
            new Dictionary < HlTokenType, HlExpressionCompiler < T > >();

        protected override bool AllImplementations => true;

        #region Public

        public OperatorCompilerCollection( Dictionary < HlTokenType, HlExpressionCompiler < T > > opCompilers )
        {
            m_OpCompilers = opCompilers;
        }

        public override ExpressionTarget ParseExpression( HlCompilation compilation, T expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
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
