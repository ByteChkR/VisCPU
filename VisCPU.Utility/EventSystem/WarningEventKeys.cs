namespace VisCPU.Utility.EventSystem
{

    public static class WarningEventKeys
    {
        public static readonly string s_ModuleVersionExists = "module-version-exists";

        public static readonly string s_MemoryBusDeviceOverlap = "memory-bus-device-overlap";
        public static readonly string s_MemoryBusReadUnmapped = "memory-bus-read-unmapped";
        public static readonly string s_MemoryBusWriteUnmapped = "memory-bus-write-unmapped";

        public static readonly string s_PeripheralInvalidRead = "peripheral-invalid-read";
        public static readonly string s_PeripheralInvalidWrite = "peripheral-invalid-write";

        public static readonly string s_PeripheralMemoryPathNotSet = "memory-path-not-set";

        public static readonly string s_LinkerDuplicateItem = "linker-duplicate-item";

        public static readonly string s_InvertInvalidEvent = "hl-compiler-invalid-unary-minus";
    }

}
