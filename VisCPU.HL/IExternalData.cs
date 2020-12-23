namespace VisCPU.HL
{
    public interface IExternalData
    {
        ExternalDataType DataType { get; }

        string GetFinalName();

        string GetName();
    }
}