using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public interface IHLTypeSystemInstance : IHLToken
    {

        uint GetSize();

        string Name { get; }

    }

}
