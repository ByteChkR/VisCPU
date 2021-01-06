namespace VisCPU.HL.Importer
{

    public interface IFileImporter : IImporter
    {

        string ProcessImport(string input);

    }

}