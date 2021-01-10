using VisCPU.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Variables
{

    public class VariableDefinitionExpressionCompiler : HLExpressionCompiler < HLVarDefOperand >
    {

        public readonly HLTypeSystem TypeSystem;

        #region Public

        public VariableDefinitionExpressionCompiler( HLTypeSystem typeSystem )
        {
            TypeSystem = typeSystem;
        }

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLVarDefOperand expr )
        {
            if ( expr.value.TypeName.ToString() == HLCompilation.CONST_VAL_TYPE )
            {
                string asmVarName = expr.value.Name.ToString();

                if ( compilation.ConstValTypes.ContainsKey( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateConstVarDefinitionEvent( asmVarName ) );
                }

                compilation.ConstValTypes.Add( asmVarName, null );

                return new ExpressionTarget( asmVarName, true, compilation.TypeSystem.GetType( "var" ) );
            }

            if ( expr.value.TypeName.ToString() == HLCompilation.VAL_TYPE )
            {
                string asmVarName = expr.value.Name.ToString();

                if ( compilation.ContainsLocalVariable( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateVarDefinitionEvent( asmVarName ) );
                }

                HLTypeDefinition vdef = TypeSystem.GetType( HLCompilation.VAL_TYPE );
                uint arrSize = expr.value.Size?.ToString().ParseUInt() ?? 1;

                if ( arrSize != 1 )
                {
                    vdef = new ArrayTypeDefintion( vdef, arrSize );
                }

                compilation.CreateVariable(
                                           asmVarName,
                                           arrSize,
                                           vdef
                                          );

                return new ExpressionTarget(
                                            compilation.GetFinalName( asmVarName ),
                                            true,
                                            vdef
                                           );
            }

            HLTypeDefinition type = TypeSystem.GetType( expr.value.TypeName.ToString() );
            uint size = expr.value.Size?.ToString().ParseUInt() ?? 1;
            bool isArray = expr.value.Size != null;
            compilation.CreateVariable( expr.value.Name.ToString(), type.GetSize() * size, type );

            return new ExpressionTarget( compilation.GetFinalName( expr.value.Name.ToString() ), true, type, isArray );
        }

        #endregion

    }

}
