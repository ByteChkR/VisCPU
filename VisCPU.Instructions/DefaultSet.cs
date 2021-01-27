using VisCPU.Instructions.Bitwise;
using VisCPU.Instructions.Bitwise.Self;
using VisCPU.Instructions.Branch;
using VisCPU.Instructions.Branch.Conditional;
using VisCPU.Instructions.Math;
using VisCPU.Instructions.Math.Float;
using VisCPU.Instructions.Math.Self;
using VisCPU.Instructions.Math.Self.Float;
using VisCPU.Instructions.Memory;
using VisCPU.Instructions.Stack;

namespace VisCPU.Instructions
{

    public class DefaultSet : InstructionSet
    {
        public override string SetKey => "VisCPU-debug-set.v1";

        #region Public

        public DefaultSet() : base(
            new Instruction[]
            {
                new NoOpInstruction(),
                new InterruptInstruction(),
                new BreakInstruction(),
                new HaltInstruction(),
                new AddInstruction(),
                new AddSelfInstruction(),
                new SubInstruction(),
                new SubSelfInstruction(),
                new IncInstruction(),
                new DecInstruction(),
                new MulInstruction(),
                new MulSelfInstruction(),
                new DivInstruction(),
                new DivSelfInstruction(),
                new ModInstruction(),
                new ModSelfInstruction(),
                new ShiftLeftInstruction(),
                new ShiftRightInstruction(),
                new ShiftLeftSelfInstruction(),
                new ShiftRightSelfInstruction(),
                new BitwiseAndInstruction(),
                new BitwiseOrInstruction(),
                new BitwiseXOrInstruction(),
                new BitwiseAndSelfInstruction(),
                new BitwiseOrSelfInstruction(),
                new BitwiseXOrSelfInstruction(),
                new LoadInstruction(),
                new CopyInstruction(),
                new DeReferenceInstruction(),
                new CopyByReferenceInstruction(),
                new JumpToInstruction(),
                new JumpToAddrInstruction(),
                new BranchIfEqual(),
                new BranchIfNotEqual(),
                new BranchIfGreater(),
                new BranchIfGreaterEqual(),
                new BranchIfLess(),
                new BranchIfLessEqual(),
                new BranchIfNotEqual(),
                new BranchIfZero(),
                new BranchIfNotZero(),
                new JumpToSubroutineInstruction(),
                new JumpToSubroutineAddrInstruction(),
                new ReturnFromSubroutineInstruction(),
                new PushInstruction(),
                new PopInstruction(),
                new PeekInstruction(),
                new AddFInstruction(),
                new AddFSelfInstruction(),
                new MulFInstruction(),
                new MulFSelfInstruction(),
                new ModFInstruction(),
                new ModFSelfInstruction(),
                new DivFInstruction(),
                new DivFSelfInstruction(),
                new SubFInstruction(),
                new SubFSelfInstruction(),
                new IncFInstruction(),
                new DecFInstruction(),
                new InvertFInstruction(),
                new InvertFSelfInstruction()
            },
            new NoOpInstruction()
        )
        {
        }

        #endregion
    }

}
