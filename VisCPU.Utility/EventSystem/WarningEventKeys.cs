namespace VisCPU.Utility.EventSystem
{

    public static class WarningEventKeys
    {

        public const string MODULE_VERSION_EXISTS = "module-version-exists";

        public const string MEMORY_BUS_DEVICE_OVERLAP = "memory-bus-device-overlap";
        public const string MEMORY_BUS_READ_UNMAPPED = "memory-bus-read-unmapped";
        public const string MEMORY_BUS_WRITE_UNMAPPED = "memory-bus-write-unmapped";

        public const string PERIPHERAL_INVALID_READ = "peripheral-invalid-read";
        public const string PERIPHERAL_INVALID_WRITE = "peripheral-invalid-write";

        public const string PERIPHERAL_MEMORY_PATH_NOT_SET = "memory-path-not-set";

        public const string LINKER_DUPLICATE_ITEM = "linker-duplicate-item";

    }

}
