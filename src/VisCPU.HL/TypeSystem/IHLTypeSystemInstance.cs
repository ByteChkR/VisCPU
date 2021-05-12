using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public interface IHlTypeSystemInstance : IHlToken
    {

        uint GetSize();

        string Name { get; }

    }

}
