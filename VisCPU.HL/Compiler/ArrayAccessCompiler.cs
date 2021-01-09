using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Compiler
{

    public class ArrayAccessCompiler : HLExpressionCompiler < HLArrayAccessorOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLArrayAccessorOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget tempPtrVar = compilation.Parse( expr.Left );
            ExpressionTarget tempPtr = new ExpressionTarget(compilation.GetTempVar(), true, tempPtrVar.TypeDefinition, true );

            ExpressionTarget pn = compilation.Parse(
                                                    expr.ParameterList[0],
                                                    new ExpressionTarget(
                                                                         compilation.GetTempVar(),
                                                                         true,
                                                                         compilation.TypeSystem.GetType( "var" )
                                                                        )
                                                   );

            string tmpSName = compilation.GetTempVar();
            compilation.ProgramCode.Add( $"LOAD {tmpSName} {tempPtr.TypeDefinition.GetSize()}" );
            compilation.ProgramCode.Add( $"MUL {pn.ResultAddress} {tmpSName}" );
            compilation.ReleaseTempVar( tmpSName );

            if ( tempPtrVar.IsPointer && !( tempPtrVar.TypeDefinition is ArrayTypeDefintion ) )
            {
                compilation.ProgramCode.Add( $"LOAD {tempPtr.ResultAddress} {tempPtrVar.ResultAddress}" );
            }
            else
            {
                compilation.ProgramCode.Add( $"COPY {tempPtrVar.ResultAddress} {tempPtr.ResultAddress}" );
            }

            compilation.ProgramCode.Add( $"ADD {tempPtr.ResultAddress} {pn.ResultAddress} ; Apply offset" );

            if ( outputTarget.ResultAddress != null )
            {
                compilation.ProgramCode.Add(
                                            $"DREF {tempPtr.ResultAddress} {outputTarget.ResultAddress} ; Dereference Array Pointer"
                                           );

                compilation.ReleaseTempVar(tempPtr.ResultAddress);
                compilation.ReleaseTempVar(tempPtrVar.ResultAddress);
                compilation.ReleaseTempVar(pn.ResultAddress);

                return outputTarget;
            }
            
            compilation.ReleaseTempVar(tempPtrVar.ResultAddress);
            compilation.ReleaseTempVar(pn.ResultAddress);
            return tempPtr;
        }

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLArrayAccessorOp expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        #endregion

    }

}
