using VisCPU.HL.DataTypes;
using VisCPU.HL.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Variables
{

    public class VarExpressionCompiler : HlExpressionCompiler < HlVarOperand >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlVarOperand expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlVarOperand expr,
            ExpressionTarget outputTarget )
        {
            if ( compilation.ConstValTypes.Contains( expr.Value.ToString() ) )
            {
                return new ExpressionTarget(
                                            expr.Value.ToString(),
                                            true,
                                            compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName )
                                           ).
                    CopyIfNotNull( compilation, outputTarget );
            }

            string varAddr;

            if ( compilation.ContainsVariable( expr.Value.ToString() ) )
            {
                VariableData v = compilation.GetVariable( expr.Value.ToString() );
                varAddr = v.GetFinalName();

                return new ExpressionTarget(
                                            varAddr,
                                            true,
                                            v.TypeDefinition,
                                            !v.TypeDefinition.IsValueType ||
                                            v.IsPointer
                                           ).CopyIfNotNull( compilation, outputTarget );
            }

            if ( compilation.FunctionMap.Contains( expr.Value.ToString() ) )
            {
                varAddr = expr.Value.ToString();

                return new ExpressionTarget( varAddr, true, null );
            }

            if ( compilation.TypeSystem.HasType( expr.Value.ToString() ) )
            {
                HlTypeDefinition tdef = compilation.TypeSystem.GetType( expr.Value.ToString() );

                return new ExpressionTarget( "%%TYPE%%", false, tdef, false );
            }

            EventManager < ErrorEvent >.SendEvent( new HlVariableNotFoundEvent( expr.Value.ToString(), false ) );

            return new ExpressionTarget();
        }

        #endregion

    }

}
