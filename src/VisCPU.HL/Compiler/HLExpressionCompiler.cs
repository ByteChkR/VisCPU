using System;

using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler
{

    public abstract class HlExpressionCompiler < T > : VisBase, IHlExpressionCompiler
        where T : HlExpression
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HlCompiler;

        protected virtual bool NeedsOutput { get; }

        protected virtual bool AllImplementations { get; }

        #region Public

        public virtual ExpressionTarget ParseExpression(
            HlCompilation compilation,
            T expr,
            ExpressionTarget outputTarget )
        {
            throw new NotImplementedException();
        }

        public virtual ExpressionTarget ParseExpression( HlCompilation compilation, T expr )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private

        private ExpressionTarget InnerParseExpression(
            HlCompilation compilation,
            T expr,
            ExpressionTarget outputTarget )
        {
            if ( NeedsOutput )
            {
                ExpressionTarget target = outputTarget;
                
                if ( outputTarget.ResultAddress == null || !outputTarget.IsAddress )
                {
                    target = new ExpressionTarget(
                                                  SettingsManager.GetSettings < HlCompilerSettings >().OmitTempVarInit
                                                      ? compilation.GetTempVar()
                                                      : compilation.GetTempVar( 0 ),
                                                  true,
                                                  compilation.TypeSystem.GetType(
                                                       compilation.Root,
                                                       HLBaseTypeNames.s_UintTypeName
                                                      )
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

        ExpressionTarget IHlExpressionCompiler.Parse(
            HlCompilation compilation,
            HlExpression expr,
            ExpressionTarget outputTarget )
        {
            return InnerParseExpression( compilation, ( T ) expr, outputTarget );
        }

        #endregion

    }

}
