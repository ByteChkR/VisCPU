using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using VisCPU.Instructions;
using VisCPU.Utility;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;

namespace VisCPU
{

    public class Cpu
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

        public readonly CpuSymbolServer SymbolServer = new CpuSymbolServer();

        private readonly struct CpuState : IEquatable < CpuState >
        {

            public readonly Flags Flags;
            public readonly uint Pc;

            public CpuState( Flags flags, uint pc )
            {
                Flags = flags;
                Pc = pc;
            }

            public bool Equals( CpuState other )
            {
                return Flags == other.Flags && Pc == other.Pc;
            }

            public override bool Equals( object obj )
            {
                return obj is CpuState other && Equals( other );
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ( ( int ) Flags * 397 ) ^ ( int ) Pc;
                }
            }

        }

        private Action < Cpu, uint > m_InterruptHandler;

        private uint m_InternalInterruptHandler = 0;

        private readonly Stack < CpuState > m_CpuStack = new Stack < CpuState >();

        private readonly uint m_IntAddress;

        private readonly uint m_ResetAddress;
        private readonly Stack < uint > m_Stack = new Stack < uint >();
        private uint m_RemainingCycles;

        public int StackDepth => m_CpuStack.Count;

        public uint ProgramCounter { get; private set; }

        public Flags ProcessorFlags { get; private set; }

        public event Action < Cpu > OnBreak;

        #region Unity Event Functions

        public void Reset()
        {
            ProgramCounter = m_ResetAddress;
            m_CpuStack.Clear();
            MemoryBus.Reset();
        }

        #endregion

        #region Public

        public Cpu( MemoryBus bus, uint resetAddress, uint interruptAddress )
        {
            MemoryBus = bus;
            MemoryBus.SetCpu( this );
            m_IntAddress = interruptAddress;
            m_ResetAddress = resetAddress;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void ClearStackAndStates()
        {
            m_Stack.Clear();
            m_CpuStack.Clear();
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

            Instruction instruction = CpuSettings.InstructionSet.GetInstruction( op );

            if ( instruction == null && SettingsManager.GetSettings < CpuSettings >().DumpOnCrash )
            {
                Dump();
            }

            if ( Logger.s_Settings.DebugCore )
            {
                uint a0 = DecodeArgument( 0 );
                uint a1 = DecodeArgument( 1 );
                uint a2 = DecodeArgument( 2 );

                Logger.LogMessage(
                                  LoggerSystems.Debug,
                                  "Instruction: {0} {1}({4}) {2}({5}) {3}({6})",
                                  instruction.Key,
                                  a0,
                                  a1,
                                  a2,
                                  MemoryBus.Read( a0 ),
                                  MemoryBus.Read( a1 ),
                                  MemoryBus.Read( a2 )
                                 );
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

        public void FireInternalInterrupt( uint intCode )
        {
            if ( m_InternalInterruptHandler == 0 )
            {
                return;
            }

            Push( intCode );
            SetState( m_InternalInterruptHandler - CpuSettings.InstructionSize );
        }

        public void FireInterrupt( uint intCode )
        {
            if ( m_InterruptHandler != null )
            {
                Set( Flags.Interrupt );
                m_InterruptHandler( this, intCode );
            }
            else
            {
                Logger.LogMessage( LoggerSystems.All, "Interrupt Handler not Attached." );
            }

            if ( !HasSet( Flags.Halt ) )
            {
                FireInternalInterrupt( intCode );
            }
        }

        public IEnumerable < uint > GetCpuStates()
        {
            return m_CpuStack.Select( x => x.Pc );
        }

        public uint GetInternalInterruptHandler()
        {
            return m_InternalInterruptHandler;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool HasSet( Flags flag )
        {
            return ( ProcessorFlags & flag ) != 0;
        }

        public void LoadBinary( uint[] bios )
        {
            LoadBinary( bios, 0 );
        }

        public void LoadBinary( uint[] bios, uint start )
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
                CpuState state = m_CpuStack.Pop();
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
        public void PushState( uint pc, Flags flags )
        {
            m_CpuStack.Push(
                            new CpuState( ProcessorFlags, ProgramCounter )
                           );

            ProcessorFlags = flags;
            ProgramCounter = pc;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void PushState( uint pc )
        {
            PushState( pc, Flags.None );
        }

        public IEnumerable GetEnumerator()
        {
            while (!HasSet(Flags.Halt))
            {
                Cycle();

                if (HasSet(Flags.Break))
                {
                    if (OnBreak == null)
                    {
                        UnSet(Flags.Break);
                    }
                    else
                    {
                        OnBreak(this);
                    }
                }
                yield return true;
            }

            MemoryBus.Shutdown();
            yield return false;
        }

        public void Run()
        {
            while (!HasSet(Flags.Halt))
            {
                Cycle();

                if (HasSet(Flags.Break))
                {
                    if (OnBreak == null)
                    {
                        UnSet(Flags.Break);
                    }
                    else
                    {
                        OnBreak(this);
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

        public void SetInternalInterruptHandler( uint handler )
        {
            m_InternalInterruptHandler = handler;
        }

        public void SetInterruptHandler( Action < Cpu, uint > handler )
        {
            m_InterruptHandler = handler;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetState( uint pc, Flags flags )
        {
            ProcessorFlags = flags;
            ProgramCounter = pc;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetState( uint pc )
        {
            SetState( pc, Flags.None );
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
            List < CpuState > states = new List < CpuState >( m_CpuStack );
            tw.WriteLine( "Stack:" );

            for ( int i = 0; i < states.Count; i++ )
            {
                CpuState cpuState = states[i];
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
