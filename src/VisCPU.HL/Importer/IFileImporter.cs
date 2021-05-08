namespace VisCPU.HL.Importer
{

    public interface IFileImporter : IImporter
    {
        IncludedItem ProcessImport( string input );
    }

}
