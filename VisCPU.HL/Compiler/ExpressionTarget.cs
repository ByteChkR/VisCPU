﻿using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Compiler
{

    public readonly struct ExpressionTarget
    {

        public readonly string ResultAddress;
        public readonly bool IsAddress;
        public readonly bool IsPointer;
        public readonly HLTypeDefinition TypeDefinition;

        public ExpressionTarget( string resultAddress, bool isAddress, HLTypeDefinition def, bool isPointer = false )
        {
            ResultAddress = resultAddress;
            IsAddress = isAddress;
            IsPointer = isPointer;
            TypeDefinition = def;
        }

        public ExpressionTarget Cast( HLTypeDefinition newType )
        {
            return new ExpressionTarget( ResultAddress, IsAddress, newType, IsPointer );
        }

        public ExpressionTarget MakeAddress( HLCompilation c )
        {
            if ( IsAddress )
            {
                return this;
            }

            ExpressionTarget tmpVal = new ExpressionTarget( c.GetTempVar(), true, TypeDefinition, IsPointer );
            c.ProgramCode.Add( $"LOAD {tmpVal.ResultAddress} {ResultAddress}" );

            return tmpVal;
        }

        public ExpressionTarget LoadIfNotNull( HLCompilation compilation, ExpressionTarget target )
        {
            if ( target.ResultAddress == null )
            {
                return this;
            }

            compilation.ProgramCode.Add( $"LOAD {target.ResultAddress} {ResultAddress}" );

            return target;
        }

        public ExpressionTarget CopyIfNotNull( HLCompilation compilation, ExpressionTarget target )
        {
            if ( target.ResultAddress == null )
            {
                return this;
            }

            compilation.ProgramCode.Add( $"COPY {ResultAddress} {target.ResultAddress}" );

            return target;
        }

    }

}
