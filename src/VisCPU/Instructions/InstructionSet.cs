using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Instructions
{

    public abstract class InstructionSet
    {

        public readonly struct InstructionData : IEquatable < InstructionData >
        {

            public readonly Instruction Instruction;
            public readonly byte OpCode;

            public InstructionData( byte opCode, Instruction instr )
            {
                OpCode = opCode;
                Instruction = instr;
            }

            public bool Equals( InstructionData other )
            {
                return Equals( Instruction, other.Instruction ) && OpCode == other.OpCode;
            }

            public override bool Equals( object obj )
            {
                return obj is InstructionData other && Equals( other );
            }

            public override int GetHashCode()
            {
                return ( ( Instruction != null ? Instruction.GetHashCode() : 0 ) * 397 ) ^ OpCode.GetHashCode();
            }

        }

        private readonly Instruction[] m_Instructions;

        public abstract string SetKey { get; }

        #region Public

        public Instruction GetInstruction( string key, int arguments )
        {
            Instruction ret = m_Instructions.FirstOrDefault( x => x.Key == key && x.ArgumentCount == arguments );

            if ( ret == null )
            {
                EventManager < ErrorEvent >.SendEvent( new InstructionNotFoundEvent( key, arguments ) );
            }

            return ret;
        }

        public uint GetInstruction( Instruction instr )
        {
            int idx = m_Instructions.ToList().IndexOf( instr );

            if ( idx == -1 )
            {
                EventManager < ErrorEvent >.SendEvent( new InstructionNotFoundEvent( instr ) );
            }

            return ( uint ) idx; //lazy bastard
        }

        public Instruction GetInstruction( uint opCode )
        {
            Instruction i = m_Instructions[opCode];

            if ( i == null )
            {
                EventManager < ErrorEvent >.SendEvent( new InstructionNotFoundEvent( ( byte ) opCode ) );
            }

            return i;
        }

        public Instruction[] GetInstructions()
        {
            return m_Instructions.ToArray();
        }

        public Instruction[] GetInstructions( string key )
        {
            return m_Instructions.Where( x => x.Key == key ).ToArray();
        }

        #endregion

        #region Protected

        protected InstructionSet( Instruction[] instructions ) : this( instructions, null )
        {
        }

        protected InstructionSet( Instruction[] instructions, Instruction noOp )
        {
            m_Instructions = instructions;
        }

        protected InstructionSet( IEnumerable < InstructionData > data ) : this( data, null )
        {
        }

        protected InstructionSet( IEnumerable < InstructionData > data, Instruction noOp )
        {
            m_Instructions = new Instruction[byte.MaxValue];

            foreach ( InstructionData instructionData in data )
            {
                if ( m_Instructions[instructionData.OpCode] == null )
                {
                    m_Instructions[instructionData.OpCode] = instructionData.Instruction;
                }
                else
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new DuplicateInstructionOpCodesEvent(
                                                               m_Instructions[instructionData.OpCode],
                                                               instructionData.Instruction,
                                                               instructionData.OpCode
                                                              )
                                                         );
                }
            }

            if ( noOp != null )
            {
                for ( int i = 0; i < m_Instructions.Length; i++ )
                {
                    if ( m_Instructions[i] == null )
                    {
                        m_Instructions[i] = noOp;
                    }
                }
            }
        }

        #endregion

    }

}
