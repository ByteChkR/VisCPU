namespace Utility.ADL.Configs
{

    public interface IProjectDebugConfig
    {

        int GetAcceptMask();

        int GetMinSeverity();

        PrefixLookupSettings GetPrefixLookupSettings();

        string GetProjectName();

        void SetAcceptMask( int mask );

        void SetMinSeverity( int severity );

        void SetPrefixLookupSettings( PrefixLookupSettings settings );

        void SetProjectName( string projectName );

    }

}
