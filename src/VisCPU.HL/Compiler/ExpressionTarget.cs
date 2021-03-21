using System;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler
{

    public readonly struct ExpressionTarget : IEquatable < ExpressionTarget >
    {

        public readonly string ResultAddress;
        public readonly bool IsAddress;
        public readonly bool IsPointer;
        public readonly HlTypeDefinition TypeDefinition;

        public ExpressionTarget( string resultAddress, bool isAddress, HlTypeDefinition def )
            : this(
                   resultAddress,
                   isAddress,
                   def,
                   false
                  )
        {
            //ResultAddress = resultAddress;
            //IsAddress = isAddress;
            //TypeDefinition = def;
        }

        public ExpressionTarget( string resultAddress, bool isAddress, HlTypeDefinition def, bool isPointer )
        {
            ResultAddress = resultAddress;
            IsAddress = isAddress;
            IsPointer = isPointer;
            TypeDefinition = def;
        }

        public uint StaticParse()
        {
            if ( TypeDefinition.Name == HLBaseTypeNames.s_UintTypeName && ResultAddress.TryParseUInt( out uint hret ) )
            {
                return hret;
            }

            //if (TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName && float.TryParse(ResultAddress, out float fret))
            //{
            //    return fret;
            //}

            if ( ResultAddress.StartsWith( "'" ) &&
                 ResultAddress.EndsWith( "'" ) &&
                 char.TryParse( ResultAddress.Remove( ResultAddress.Length - 1, 1 ).Remove( 0, 1 ), out char cret ) )

            {
                return cret;
            }

            EventManager < ErrorEvent >.SendEvent( new StaticParseFailedEvent( ResultAddress ) );

            return 0;
        }

        public ExpressionTarget Cast( HlTypeDefinition newType )
        {
            return new ExpressionTarget( ResultAddress, IsAddress, newType );
        }

        public ExpressionTarget Reinterpret( bool isAddress, bool isPointer )
        {
            return new ExpressionTarget( ResultAddress, isAddress, TypeDefinition );
        }

        public ExpressionTarget Dereference( HlCompilation c )
        {
            if ( !IsPointer )
            {
                return this;
            }

            ExpressionTarget tmpVal = new ExpressionTarget(
                                                           c.GetTempVarDref( ResultAddress ),
                                                           true,
                                                           TypeDefinition
                                                          );

            return tmpVal;
        }

        public ExpressionTarget MakeAddress( HlCompilation c )
        {
            if ( IsAddress )
            {
                return this;
            }

            ExpressionTarget tmpVal = new ExpressionTarget(
                                                           c.GetTempVarLoad( ResultAddress ),
                                                           true,
                                                           TypeDefinition,
                                                           IsPointer
                                                          );

            return tmpVal;
        }
        public ExpressionTarget LoadIfNotNull(HlCompilation compilation, string value)
        {
            if (value == null) return this;
            compilation.EmitterResult.Emit($"LOAD", ResultAddress, value);

            return this;
        }
        public ExpressionTarget LoadIfNotNull( HlCompilation compilation, ExpressionTarget target )
        {
            if ( target.ResultAddress == null || target.ResultAddress == ResultAddress )
            {
                return this;
            }

            compilation.EmitterResult.Emit( $"LOAD", target.ResultAddress, ResultAddress );

            return target;
        }

        public ExpressionTarget CopyIfNotNull(
            HlCompilation compilation,
            ExpressionTarget target )
        {
            if ( target.ResultAddress == null || target.ResultAddress == ResultAddress )
            {
                return this;
            }

            if ( !IsAddress )
            {
                compilation.EmitterResult.Emit( $"LOAD", target.ResultAddress, ResultAddress );
            }
            else
            {
                compilation.EmitterResult.Emit( $"COPY", ResultAddress, target.ResultAddress );
            }

            compilation.ReleaseTempVar( ResultAddress );

            return target;
        }

        public bool Equals( ExpressionTarget other )
        {
            return ResultAddress == other.ResultAddress &&
                   IsAddress == other.IsAddress &&
                   IsPointer == other.IsPointer &&
                   Equals( TypeDefinition, other.TypeDefinition );
        }

        public override bool Equals( object obj )
        {
            return obj is ExpressionTarget other && Equals( other );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ResultAddress != null ? ResultAddress.GetHashCode() : 0;
                hashCode = ( hashCode * 397 ) ^ IsAddress.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ IsPointer.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ ( TypeDefinition != null ? TypeDefinition.GetHashCode() : 0 );

                return hashCode;
            }
        }

    }

}
