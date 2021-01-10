using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

using VisCPU.Instructions;
using VisCPU.Utility;

namespace VisCPU
{

    public class CPU
    {

        [Flags]
        public enum Flags
        {

            NONE = 0,
            BREAK = 1,
            INTERRUPT = 2,
            HALT = 4

        }

        public readonly MemoryBus MemoryBus;

        private struct CPUState
        {

            public Flags flags;
            public uint pc;

        }

        private readonly Stack < CPUState > cpuStack = new Stack < CPUState >();

        private readonly uint intAddress;

        private readonly uint resetAddress;
        private readonly Stack < uint > stack = new Stack < uint >();
        private uint remainingCycles;

        public uint ProgramCounter { get; private set; }

        public Flags ProcessorFlags { get; private set; }

        public event Action < CPU > OnBreak;

        #region Unity Event Functions

        public void Reset()
        {
            ProgramCounter = resetAddress;
            cpuStack.Clear();
            MemoryBus.Reset();
        }

        #endregion

        #region Public

        public CPU( MemoryBus bus, uint resetAddress, uint interruptAddress )
        {
            MemoryBus = bus;
            intAddress = interruptAddress;
            this.resetAddress = resetAddress;
        }

        public bool Cycle()
        {
            if ( remainingCycles != 0 )
            {
                remainingCycles--;

                return false;
            }

            if ( HasSet( Flags.BREAK | Flags.HALT ) )
            {
                remainingCycles = 0;

                return true;
            }

            if ( HasSet( Flags.INTERRUPT ) )
            {
                PushState( intAddress, ProcessorFlags & ~Flags.INTERRUPT );
                remainingCycles = 0;

                return true;
            }

            uint op = MemoryBus.Read( ProgramCounter );

            Instruction instruction = CPUSettings.InstructionSet.GetInstruction( op );

            if ( instruction == null && CPUSettings.DumpOnCrash )
            {
                Dump();
            }

            remainingCycles = instruction.Cycles;
            instruction.Process( this );
            ProgramCounter += instruction.InstructionSize;

            return remainingCycles == 0;
        }

        public uint DecodeArgument( int argNum )
        {
            return MemoryBus.Read( ProgramCounter + ( uint ) argNum + 1 );
        }

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

        public uint Peek()
        {
            return stack.Peek();
        }

        public uint Pop()
        {
            return stack.Pop();
        }

        public void PopState()
        {
            if ( cpuStack.Count != 0 )
            {
                CPUState state = cpuStack.Pop();
                ProgramCounter = state.pc;
                ProcessorFlags = state.flags;
            }
        }

        public void Push( uint value )
        {
            stack.Push( value );
        }

        public void PushState( uint pc, Flags flags = Flags.NONE )
        {
            cpuStack.Push(
                          new CPUState
                          {
                              pc = ProgramCounter,
                              flags = ProcessorFlags
                          }
                         );

            ProcessorFlags = flags;
            ProgramCounter = pc;
        }

        public void Run()
        {
            while ( !HasSet( Flags.HALT ) )
            {
                Cycle();

                if ( HasSet( Flags.BREAK ) )
                {
                    if ( OnBreak == null )
                    {
                        UnSet( Flags.BREAK );
                    }
                    else
                    {
                        OnBreak( this );
                    }
                }
            }

            MemoryBus.Shutdown();
        }

        public void Set( Flags flag )
        {
            ProcessorFlags |= flag;
        }

        public void SetState( uint pc, Flags flags = Flags.NONE )
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
            List < CPUState > states = new List < CPUState >( cpuStack );
            tw.WriteLine( "Stack:" );

            for ( int i = 0; i < states.Count; i++ )
            {
                CPUState cpuState = states[i];
                tw.WriteLine( "Stack Pos: " + i + " PC: " + cpuState.pc.ToHexString() + " FLAGS: " + cpuState.flags );
            }

            tw.WriteLine();

            tw.Close();
            fs.Close();
            MemoryBus.Dump();
        }

        #endregion

    }

}
