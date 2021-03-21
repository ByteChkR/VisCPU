using System;

namespace Utility.ADL.Configs
{
    public class ProjectDebugConfig : IProjectDebugConfig
    {

        private int? minSeverity;

        public ProjectDebugConfig(
            string projectName, int acceptMask,
            PrefixLookupSettings lookupSettings)
        {
            ProjectName = projectName;
            AcceptMask = acceptMask;
            PrefixLookupSettings = lookupSettings;
            Debug.ConfigCreated(this);
        }

        public string ProjectName { get; set; }

        public int AcceptMask { get; set; }

        public int MinSeverity
        {
            get => minSeverity ?? Debug.DefaultSeverity;
            set => minSeverity = value;
        }

        public PrefixLookupSettings PrefixLookupSettings { get; set; }

        public virtual string GetProjectName()
        {
            return ProjectName;
        }

        public virtual int GetMinSeverity()
        {
            return MinSeverity;
        }

        public virtual int GetAcceptMask()
        {
            return AcceptMask;
        }

        public virtual PrefixLookupSettings GetPrefixLookupSettings()
        {
            return PrefixLookupSettings;
        }


        public virtual void SetProjectName(string projectName)
        {
            ProjectName = projectName;
        }

        public virtual void SetMinSeverity(int severity)
        {
            MinSeverity = severity;
        }

        public virtual void SetAcceptMask(int mask)
        {
            AcceptMask = mask;
        }

        public virtual void SetPrefixLookupSettings(PrefixLookupSettings settings)
        {
            PrefixLookupSettings = settings;
        }

        public override string ToString()
        {
            return ProjectName;
        }

    }

    public class ProjectDebugConfig<MaskType, SeverityType> : IProjectDebugConfig
        where MaskType : Enum
        where SeverityType : Enum
    {

        private int? minSeverity;

        public ProjectDebugConfig(
            string projectName, MaskType acceptMask,
            PrefixLookupSettings lookupSettings)
        {
            ProjectName = projectName;
            AcceptMask = acceptMask;
            PrefixLookupSettings = lookupSettings;
            Debug.ConfigCreated(this);
        }

        public string ProjectName { get; set; }

        public MaskType AcceptMask { get; set; }

        public SeverityType MinSeverity
        {
            get =>
                minSeverity != null
                    ? (SeverityType) Enum.ToObject(typeof(SeverityType), minSeverity)
                    : (SeverityType) Enum.ToObject(typeof(SeverityType), Debug.DefaultSeverity);
            set => minSeverity = Convert.ToInt32(value);
        }

        public PrefixLookupSettings PrefixLookupSettings { get; set; }

        public virtual string GetProjectName()
        {
            return ProjectName;
        }

        public virtual int GetMinSeverity()
        {
            return minSeverity ?? Debug.DefaultSeverity;
        }

        public virtual int GetAcceptMask()
        {
            return Convert.ToInt32(AcceptMask);
        }

        public virtual PrefixLookupSettings GetPrefixLookupSettings()
        {
            return PrefixLookupSettings;
        }

        public virtual void SetProjectName(string projectName)
        {
            ProjectName = projectName;
        }

        public virtual void SetMinSeverity(int severity)
        {
            minSeverity = severity;
        }

        public virtual void SetAcceptMask(int mask)
        {
            AcceptMask = (MaskType) Enum.ToObject(typeof(MaskType), mask);
        }

        public virtual void SetPrefixLookupSettings(PrefixLookupSettings settings)
        {
            PrefixLookupSettings = settings;
        }

        public override string ToString()
        {
            return ProjectName;
        }

    }
}