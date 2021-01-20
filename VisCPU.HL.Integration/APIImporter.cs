using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer;
using VisCPU.Utility.IO;

public class APIImporter: AImporter, IDataImporter, IFileImporter
{
    private Dictionary<uint, FunctionData> m_ExposedApis = new Dictionary<uint, FunctionData>();
    public readonly string DeviceDriverDirectory;

    public APIImporter()
    {
        DeviceDriverDirectory = Path.Combine(AImporter.CacheRoot, "api-devices");
        if (!Directory.Exists(DeviceDriverDirectory)) Directory.CreateDirectory(DeviceDriverDirectory);
    }
    public void AddApi(uint addr, FunctionData funcData)
    {
        m_ExposedApis.Add(addr, funcData);
        string driverDir = Path.Combine(DeviceDriverDirectory,
            funcData.GetFinalName() + ".vhl");
        File.WriteAllText(driverDir, GenerateAPIDriver(funcData, addr));
    }


    private string GenerateAPIDriver(FunctionData data, uint devAddr)
    {
        string header = $"public var {data.GetFinalName()}(";
        string args = "";
        for (int i = 0; i < data.ParameterCount; i++)
        {
            if (i != 0) args += $", var arg{i}";
            else args += $"var arg{i}";
        }
        header += $"{args})";
        string body = "{\n";

        string addrDecl = $"var addr = {devAddr};\n";

        body += addrDecl;
        body += $"return addr(";
        string invArgs = "";
        for (int i = 0; i < data.ParameterCount; i++)
        {
            if (i != 0) invArgs += $", arg{i}";
            else invArgs += $"arg{i}";
        }
        body += $"{invArgs});\n";
        body += "}";
        return header + body;
    }

    public override bool CanImport(string input)
    {
        return input.StartsWith("api-integration ");
    }

    string IFileImporter.ProcessImport(string input)
    {
        string name = input.Remove(0, "api-integration ".Length);
        KeyValuePair<uint, FunctionData> api = m_ExposedApis.First(x => x.Value.GetFinalName() == name);
        string target = Path.Combine(DeviceDriverDirectory,
            api.Value.GetFinalName() + ".vhl");
        Log("Including Device Driver: {0} :: {1}", name, target);
        return target;
    }

    IExternalData[] IDataImporter.ProcessImport(string input)
    {
        string name = input.Remove(0, "api-integration ".Length);
        KeyValuePair<uint, FunctionData> api = m_ExposedApis.First(x => x.Value.GetFinalName() == name);
        return new[] {api.Value};
    }
}