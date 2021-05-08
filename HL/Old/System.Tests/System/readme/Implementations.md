# HL System Library

## Implemented Classes

### RawAllocator : Allocator
#### RawAllocator(uint memoryStart, uint memoryLength)
> Constructor for Allocator
#### void SetOptimizationLevel(uint level)
> Sets the Optimization Level
```
0 = No Optimizations
1 = Single Iteration Optimization
2 = Recursive Iteration Optimization
```
#### void WriteDebugData()
> Writes the Current Allocation Table to the Console Output

### RawAllocatorBlockData
Helper Class that represents the Meta-Data of an allocated/unallocated block
#### RawAllocatorBlockData(uint length, uint free)
> Constructor of the Block Data.
> Returns the Length of the Block
#### void SetData(uint length, uint free)
> Setter for Resizing or Setting the Free State.
#### void SetFree(uint free)
> Marks the Block as Free if `free` != 0
#### uint IsFree()
> Returns if the Block is Free or currently beeing used.

____

### BlockNotMappedException : Exception
Gets thrown if Allocator.Free is called with a pointer that was not Allocated by the Allocator Instance.

#### BlockNotMappedException()
> Empty Constructor.

### OutOfMemoryException : Exception
Gets thrown if the Allocator instance runs out of available memory.

#### OutOfMemoryException()
> Empty Constructor.

### DeviceNotFoundException : Exception
Gets thrown if the present pin in the driver does not correspond to the present pin of the device.

#### DeviceNotFoundException()
> Empty Constructor.
