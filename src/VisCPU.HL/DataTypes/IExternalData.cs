namespace VisCPU.HL.DataTypes
{

    public interface IExternalData
    {

        ExternalDataType DataType { get; }

        string GetFinalName();

        string GetName();

        void SetUsed();

        int UseCount { get; }

    }

}
