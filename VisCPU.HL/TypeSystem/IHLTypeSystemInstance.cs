using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{
    public interface IHLTypeSystemInstance : IHLToken
    {
        string Name { get; }

        uint GetSize();
    }
}