namespace OpenCL.NET.Interop.Programs
{

    /// <summary>
    ///     Represents an enumeration for the different types of information that can be queried from a program.
    /// </summary>
    public enum ProgramInformation : uint
    {
        /// <summary>
        ///     Retrieves the program reference count. The reference count returned should be considered immediately stale. It is
        ///     unsuitable for general use in applications. This feature is provided for identifying memory leaks.
        /// </summary>
        ReferenceCount = 0x1160,

        /// <summary>
        ///     Retrieves the context specified when the program object is created.
        /// </summary>
        Context = 0x1161,

        /// <summary>
        ///     Retrieves the number of devices associated with program.
        /// </summary>
        NumberOfDevices = 0x1162,

        /// <summary>
        ///     Retrieves the list of devices associated with the program object. This can be the devices associated with
        ///     <see cref="context" /> on which the program object has been created or can be a subset of devices that are
        ///     specified when a
        ///     progam object is created using <see cref="CreateProgramWithBinary" />.
        /// </summary>
        Devices = 0x1163,

        /// <summary>
        ///     Retrieves the program source code specified by <see cref="CreateProgramWithSource" />. The source string returned
        ///     is a concatenation of all source strings specified to <see cref="CreateProgramWithSource" /> with a null
        ///     terminator. The
        ///     concatenation strips any nulls in the original source strings.
        ///     If program is created using <see cref="CreateProgramWithBinary" />, <see cref="CreateProgramWithIl" />, or
        ///     <see cref="CreateProgramWithBuiltInKernels" />, a null string or the appropriate program source code is returned
        ///     depending on
        ///     whether or not the program source code is stored in the binary.
        /// </summary>
        Source = 0x1164,

        /// <summary>
        ///     Retrieves an array that contains the size in bytes of the program binary (could be an executable binary, compiled
        ///     binary or library binary) for each device associated with program. The size of the array is the number of devices
        ///     associated with program. If a binary is not available for a device(s), a size of zero is returned.
        ///     If program is created using <see cref="CreateProgramWithBuiltInKernels" />, the implementation may return zero in
        ///     any entries of the returned array.
        /// </summary>
        BinarySizes = 0x1165,

        /// <summary>
        ///     Retrieves the program binaries (could be an executable binary, compiled binary or library binary) for all devices
        ///     associated with <see cref="program" />. For each device in <see cref="program" />, the binary returned can be the
        ///     binary
        ///     specified for the device when program is created with <see cref="CreateProgramWithBinary" /> or it can be the
        ///     executable binary generated by <see cref="BuildProgram" /> or <see cref="LinkProgram" />. If program is created
        ///     with
        ///     <see cref="CreateProgramWithSource" /> or <see cref="CreateProgramWithIl" />, the binary returned is the binary
        ///     generated by <see cref="BuildProgram" />, <see cref="CompileProgram" />, or <see cref="LinkProgram" />. The bits
        ///     returned
        ///     can be an implementation-specific intermediate representation (a.k.a. IR) or device specific executable bits or
        ///     both. The decision on which information is returned in the binary is up to the OpenCL implementation.
        ///     <see cref="parameterValue" /> points to an array of n pointers allocated by the caller, where n is the number of
        ///     devices associated with <see cref="program" />. The buffer sizes needed to allocate the memory that these n
        ///     pointers
        ///     refer to can be queried using the <c>ProgramInformation.BinarySizes</c> query as described in this table.
        ///     Each entry in this array is used by the implementation as the location in memory where to copy the program binary
        ///     for a specific device, if there is a binary available. To find out which device the program binary in the array
        ///     refers to, use the <c>ProgramInformation.Devices</c> query to get the list of devices. There is a one-to-one
        ///     correspondence between the array of n pointers returned by <c>ProgramInformation.BinarySizes</c> and array of
        ///     devices
        ///     returned by <c>ProgramInformation.Devices</c>.
        ///     If an entry value in the array is <c>null</c>, the implementation skips copying the program binary for the specific
        ///     device identified by the array index.
        /// </summary>
        Binaries = 0x1166,

        /// <summary>
        ///     Retrieves the number of kernels declared in program that can be created with <see cref="CreateKernel" />. This
        ///     information is only available after a successful program executable has been built for at least one device in the
        ///     list of
        ///     devices associated with <see cref="program" />.
        /// </summary>
        NumberOfKernels = 0x1167,

        /// <summary>
        /// Retrieves a semi-colon separated list of kernel names in <see cref="program"/> that can be created with <see cref="CreateKernel"/>. This information is only available after a successful program executable has been built for at least
        // one device in the list of devices associated with <see cref="program"/>.
        /// </summary>
        KernelNames = 0x1168,

        /// <summary>
        ///     Retrieves the program IL for programs created with <see cref="CreateProgramWithIl" />. If program is created with
        ///     <see cref="CreateProgramWithSource" />, <see cref="CreateProgramWithBinary" /> or
        ///     <see cref="CreateProgramWithBuiltInKernels" /> the memory pointed to by <see cref="parameterValue" /> will be
        ///     unchanged and <see cref="parameterValueSizeRetrievesed" /> will be set to 0.
        /// </summary>
        Il = 0x1169
    }

}
