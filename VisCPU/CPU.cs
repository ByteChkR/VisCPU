using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using VisCPU.Instructions;
using VisCPU.Utility;

namespace VisCPU
{

    public class CPU
    {

        [Flags]
        public enum Flags
        {

            None = 0,
            Break = 1,
            Interrupt = 2,
            Halt = 4

        }

        public readonly MemoryBus MemoryBus;

        private readonly struct CPUState : IEquatable < CPUState >
        {

            public readonly Flags Flags;
            public readonly uint Pc;

            public CPUState( Flags flags, uint pc )
            {
                Flags = flags;
                Pc = pc;
            }

            public bool Equals( CPUState other )
            {
                return Flags == other.Flags && Pc == other.Pc;
            }

            public override bool Equals( object obj )
            {
                return obj is CPUState other && Equals( other );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( int ) Flags * 397 ) ^ ( int ) Pc;
                }
            }

        }

        private readonly Stack < CPUState > m_CpuStack = new Stack < CPUState >();

        private readonly uint m_IntAddress;

        private readonly uint m_ResetAddress;
        private readonly Stack < uint > m_Stack = new Stack < uint >();
        private uint m_RemainingCycles;

        public uint ProgramCounter { get; private set; }

        public Flags ProcessorFlags { get; private set; }

        public event Action < CPU > OnBreak;

        #region Unity Event Functions

        public void Reset()
        {
            ProgramCounter = m_ResetAddress;
            m_CpuStack.Clear();
            MemoryBus.Reset();
        }

        #endregion

        #region Public

        public CPU( MemoryBus bus, uint resetAddress, uint interruptAddress )
        {
            MemoryBus = bus;
            m_IntAddress = interruptAddress;
            m_ResetAddress = resetAddress;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool Cycle()
        {
            if ( m_RemainingCycles != 0 )
            {
                m_RemainingCycles--;

                return false;
            }

            if ( HasSet( Flags.Break | Flags.Halt ) )
            {
                m_RemainingCycles = 0;

                return true;
            }

            if ( HasSet( Flags.Interrupt ) )
            {
                PushState( m_IntAddress, ProcessorFlags & ~Flags.Interrupt );
                m_RemainingCycles = 0;

                return true;
            }

            uint op = MemoryBus.Read( ProgramCounter );

            Instruction instruction = CPUSettings.InstructionSet.GetInstruction( op );

            if ( instruction == null && CPUSettings.DumpOnCrash )
            {
                Dump();
            }

            m_RemainingCycles = instruction.Cycles - 1;
            instruction.Process( this );
            ProgramCounter += instruction.InstructionSize;

            return m_RemainingCycles == 0;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint DecodeArgument( int argNum )
        {
            return MemoryBus.Read( ProgramCounter + ( uint ) argNum + 1 );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool HasSet( Flags flag )
        {
            return ( ProcessorFlags & flag ) != 0;
        }

        public void LoadBinary( uint[] bios, uint start = 0 )
        {
            ProgramCounter = start;

            for ( uint i = start; i < start + bios.Length; i++ )
            {
                MemoryBus.Write( i, bios[i] );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Peek()
        {
            return m_Stack.Peek();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Pop()
        {
            return m_Stack.Pop();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void PopState()
        {
            if ( m_CpuStack.Count != 0 )
            {
                CPUState state = m_CpuStack.Pop();
                ProgramCounter = state.Pc;
                ProcessorFlags = state.Flags;
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Push( uint value )
        {
            m_Stack.Push( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void PushState( uint pc, Flags flags = Flags.None )
        {
            m_CpuStack.Push(
                            new CPUState( ProcessorFlags, ProgramCounter )
                           );

            ProcessorFlags = flags;
            ProgramCounter = pc;
        }

        public void Run()
        {
            while ( !HasSet( Flags.Halt ) )
            {
                Cycle();

                if ( HasSet( Flags.Break ) )
                {
                    if ( OnBreak == null )
                    {
                        UnSet( Flags.Break );
                    }
                    else
                    {
                        OnBreak( this );
                    }
                }
            }

            MemoryBus.Shutdown();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Set( Flags flag )
        {
            ProcessorFlags |= flag;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetState( uint pc, Flags flags = Flags.None )
        {
            ProcessorFlags = flags;
            ProgramCounter = pc;
        }

        public int Step()
        {
            int cycles = 0;

            while ( !Cycle() )
            {
                cycles++;
            }

            return cycles;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void UnSet( Flags flag )
        {
            ProcessorFlags &= ~flag;
        }

        #endregion

        #region Private

        private void Dump()
        {
            FileStream fs = File.Create( ".\\crash.dump.info" );
            TextWriter tw = new IndentedTextWriter( new StreamWriter( fs ) );
            List < CPUState > states = new List < CPUState >( m_CpuStack );
            tw.WriteLine( "Stack:" );

            for ( int i = 0; i < states.Count; i++ )
            {
                CPUState cpuState = states[i];
                tw.WriteLine( "Stack Pos: " + i + " PC: " + cpuState.Pc.ToHexString() + " FLAGS: " + cpuState.Flags );
            }

            tw.WriteLine();

            tw.Close();
            fs.Close();
            MemoryBus.Dump();
        }

        #endregion

    }

}
