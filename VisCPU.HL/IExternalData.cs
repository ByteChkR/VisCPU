namespace VisCPU.HL
{
    public interface IExternalData
    {
        ExternalDataType DataType { get; }

        string GetName();
        string GetFinalName();

    }
}