# HL System Library

## Device Drivers

### BENCHDRV
Device Driver for the Benchmark Peripheral

#### BENCHDRV(uint presentPin)
> Creates a Device Driver for the Benchmark Peripheral.


#### void ClearName()
> Clears the Name of the benchmark

#### void SetName(uint s, uint l)
> Sets the Name of the Benchmark(has to be set before starting the benchmark)

#### void StartRun()
> Starts the Benchmark Run.

#### void EndRun()
> Ends the Benchmark Run.

____

### MBUSDRV
Device Driver for the MemoryBus Peripheral

#### MBUSDRV()
> Creates a Memory Bus Driver Instance.

#### uint GetDeviceCount()
> Returns the Number of devices connected to the memory bus.

#### uint GetDeviceType(uint id)
> Returns the Device Type of the Device with the corresponding id.

#### uint GetDeviceAddress(uint id)
> Returns the Device Address of the Device with the corresponding id

#### uint GetDeviceNameLength(uint id)
> Returns the Device Name Length of the Device with the corresponding id

#### uint GetDeviceNameChar(uint id, uint index)
> Returns the character at `index` of the Device Name with the corresponding id

____

### STOREDRV
Device Driver for the FileDrive Peripheral

#### STOREDRV(uint presentPin)
> Creates a new Instance of the Driver.

#### uint GetSize()
> Returns the Size of the Drive.

#### void Write(uint addr, uint data)
> Writes a Single number to a specific address in the drive.

#### uint Read(uint addr)
> Reads a Single number from a specific address in the device.

#### void WriteBuffer(uint src, uint dst, uint start, uint len)
> Writes a Block of data from RAM to the Drive. Starting at `start` in ram.

#### void ReadBuffer(uint src, uint dst, uint start, uint len)
> Reads a block of data from the Drive into Ram.

____

### TIMEDRV
Device Driver for the Time Peripheral

#### TIMEDRV(uint presentPin)
> Creates new Driver Instance.

#### uint GetTime()
> Returns the Current System Time in Unix-Seconds.

