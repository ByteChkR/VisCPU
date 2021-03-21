namespace Utility.ADL.Configs
{
    public interface IProjectDebugConfig
    {

        string GetProjectName();

        int GetMinSeverity();

        int GetAcceptMask();

        PrefixLookupSettings GetPrefixLookupSettings();

        void SetProjectName(string projectName);

        void SetMinSeverity(int severity);

        void SetAcceptMask(int mask);

        void SetPrefixLookupSettings(PrefixLookupSettings settings);

    }
}